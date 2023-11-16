using System;
using System.Collections;
using Refactor.Audio;
using Refactor.Data;
using Refactor.Misc;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

//Cool idea:
//https://www.youtube.com/watch?v=JdT4ZSJ6rkA
/*
 * 1 - targeting (by camera)
 * 2 - snaps to the entity (turns around if needed)
 * 3 - slow effects
 * 4 - combo
 * 5 - skills
 * 6 - swords
 */

namespace Refactor.Entities.Modules
{
    public enum PlayerState
    {
        Default,
        Jumping,
        Dashing,
        Attacking,
        UsingSkill,
        Casting,
        Dead
    }
    
    [Serializable]
    public struct HumanFootState
    {
        // ReSharper disable once InconsistentNaming
        [Header("STATE")]
        public RaycastHit hitCache;
        public float weight;
        
        public GameObject groundObject;
        public Vector3 groundPos;
        public Quaternion groundRotation;
        public float groundDistance;
        public bool wasOnGround;
        public float footstepTime;
        
        public Vector3 offset;
        public Quaternion rotation;
        public float heightChange;
    }
    
    [Serializable]
    public class PlayerControllerEntityModule : EntityModule
    {
        [Header("REFERENCES")]
        public Animator animator;
        public Transform body;
        public Camera camera;
        public Material swordMaterial;

        [Header("REFERENCES - TARGETS")]
        public Transform riggingLookTarget;
        public Transform riggingLeanRotation;

        [Header("REFERENCES - RIGS")] 
        public LerpRig rigIdle;
        public LerpRig rigLean;
        
        [Header("SETTINGS")] 
        public bool useCameraView;
        public float ikFootOffset = 0.005f;
        public float groundLeanWeight = 0.25f;
        public float dashDuration = 1f;
        public float dashSpeed = 10f;
        public float airMoveSpeed = 3f;
        public float dashCooldown = 1f;
        public float footThreshold = 0.236f;
        
        [Header("STATE")] 
        public PlayerState state;
        public Vector3 lookDirection;
        public float posY;
        public float lastGrounded;
        public float lastWalkingInput;
        public float timeSinceLastAttack = 1000f;

        [Header("STATE - IK")]
        public float pelvisOffset;
        public HumanFootState leftFootState;
        public HumanFootState rightFootState;
        public Quaternion groundRotation;
        public LayerMask groundMask;
        
        //private PlayerAttackEntityModule _playerAttack;
        private PlayerNewAttackEntityModule _playerAttack;

        public UnityEvent onPlayerJump;
        public UnityEvent onPlayerDash;
        public UnityEvent onEnterAttackState;
        public UnityEvent onLeaveAttackState;

        #region Callbacks
        public override void OnEnable()
        {
            //Solve reload bug
            state = PlayerState.Default;
            
            if (camera == null)
                camera = Camera.main;
            
            posY = body.transform.position.y;
            _playerAttack = entity.GetModule<PlayerNewAttackEntityModule>();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            var color = new Color(1f, 1f, 1f, 1f) * 3f;
            swordMaterial.SetColor("_EmissionColor", color);
        }

        public override void UpdateFrame(float deltaTime)
        {
            UpdateAttackTimer(timeSinceLastAttack + deltaTime);
        
            switch (state)
            {
                case PlayerState.Dead:
                    animator.SetFloat("walking", 0);
                    animator.SetFloat("turning", 0);
                    entity.velocity.x = entity.velocity.z = 0;
                    return;

                case PlayerState.Default:
                    State__Default(deltaTime);
                    Handle__Dash();
                    break;
                
                case PlayerState.Jumping:
                    State__Jumping(deltaTime);
                    break;
                
                case PlayerState.Dashing:
                    entity.velocity.y = 0;
                    break;
                
                case PlayerState.UsingSkill:
                    break;
                
                case PlayerState.Attacking:
                    //_playerAttack.HandleAttack();
                    break;
            }

            if (state is not PlayerState.Attacking and not PlayerState.Casting)
            {
                Handle__ChangeElement();
            }
            
            if(state is not PlayerState.Casting and not PlayerState.Dashing && _playerAttack != null)
                _playerAttack.HandleAttacks(state, deltaTime);
        }
        #endregion

        #region Handlers
        public void Handle__ChangeElement()
        {
            if (IngameGameInput.InputChangeElement.trigger && entity.isGrounded)
            {
                entity.velocity = Vector3.zero;
                
                state = PlayerState.Casting;
                animator.CrossFade("Casting", 0.1f);

                IEnumerator LerpSword(float timeIn, float timeOut)
                {
                    var color = new Color(1f, 1f, 1f, 1f) * 3f;
                    var strength = 500f;
                    for(float f = 0; f < timeIn;)
                    {
                        swordMaterial.SetColor("_EmissionColor", color * strength * f/timeIn);
                        
                        f += Time.deltaTime;
                        yield return null;
                    }
                    
                    for(float f = 0; f < timeOut;)
                    {
                        swordMaterial.SetColor("_EmissionColor", color * strength * (1f - f/timeOut));
                        
                        f += Time.deltaTime;
                        yield return null;
                    }

                    swordMaterial.SetColor("_EmissionColor", color);
                }
                
                IEnumerator Coroutine()
                {
                    yield return new WaitForSeconds(0.5f);
                    entity.element = (entity.element is Element.Chaos) ? Element.Order : Element.Chaos;
                    yield return new WaitForSeconds(0.75f);
                    state = PlayerState.Default;
                }

                entity.StartCoroutine(Coroutine());
                entity.StartCoroutine(LerpSword(0.5f, 0.75f));
            }
        }
        
        public void Handle__Dash()
        {
            dashCooldown -= Time.deltaTime;
            
            if (dashCooldown < 0 && IngameGameInput.InputDash.trigger && entity.element == Element.Chaos)
            {
                dashCooldown = 0.5f;
                state = PlayerState.Dashing;
                entity.StartCoroutine(_Handle__Dash_Coroutine());
                onPlayerDash.Invoke();
            }
        }

        private IEnumerator _Handle__Dash_Coroutine()
        {
            #region Inputs

            var inputMove2 = IngameGameInput.InputMove;
            var inputMove = new Vector3(inputMove2.x, 0, inputMove2.y).normalized;
            if(useCameraView)
                inputMove = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * inputMove;
            if (inputMove.magnitude < 0.15f)
                inputMove = body.forward;
            #endregion

            #region Rotate
            var angle = Vector3.SignedAngle(Vector3.forward, inputMove, Vector3.up);
            body.eulerAngles = new Vector3(0,angle, 0);
            #endregion
            
            animator.CrossFade("Dash", 0.25f);
            const float delay = 0.2f;
            yield return new WaitForSeconds(delay);
            entity.velocity.x = inputMove.x * dashSpeed;
            entity.velocity.z = inputMove.z * dashSpeed;
                        
            yield return new WaitForSeconds(dashDuration - delay);

            
            /*
            const int count = 4;

            for (var i = 1; i < count; i++)
            {
                yield return new WaitForSeconds(dashDuration / count);
                entity.GetModule<CloneEntityModule>()?.Clone(0.25f);
            }
            
            yield return new WaitForSeconds(dashDuration / count);
            */
            
            
            animator.CrossFade("MainMovement", 0.5f);
            lastGrounded = Time.time;
            state = PlayerState.Default;

            entity.velocity = inputMove;
        }
        #endregion

        #region State
        public void State__Jumping(float deltaTime)
        {
            rigIdle.value = rigLean.value = 0;
            
            #region Inputs
            var inputMove2 = IngameGameInput.InputMove;
            var inputMove = new Vector3(inputMove2.x, 0, inputMove2.y).normalized;
            if(useCameraView)
                inputMove = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * inputMove;
            #endregion

            var isMoving = inputMove.magnitude > 0.15f;
            
            #region Rotation
            if (isMoving)
            {
                var angle = Vector3.SignedAngle(Vector3.forward, inputMove, Vector3.up);
                body.eulerAngles = new Vector3(0,
                    Mathf.LerpAngle(body.eulerAngles.y, angle, deltaTime * 2f), 0);

                entity.velocity.x = math.lerp(entity.velocity.x, inputMove.x * airMoveSpeed, deltaTime * 12f);
                entity.velocity.z = math.lerp(entity.velocity.z, inputMove.z * airMoveSpeed, deltaTime * 12f);
            }
            #endregion

            if (entity.isGrounded)
            {
                entity.velocity.x /= 2;
                entity.velocity.z /= 2;
                state = PlayerState.Default;
            }

            animator.SetBool("grounded", entity.isGrounded);
        }
        

        public void State__Default(float deltaTime)
        {
            #region Velocity Handling
            if (entity.isGrounded)
            {
                entity.velocity.x = math.lerp(entity.velocity.x, 0, deltaTime * 8);
                entity.velocity.z = math.lerp(entity.velocity.z, 0, deltaTime * 8);
                lastGrounded = Time.time;
            }
            else
            {
                if (entity.velocity.y > 0.5f)
                {
                    animator.CrossFade("JumpingUp", 0.1f);
                    var v = animator.velocity;
                    entity.velocity = new Vector3(0, entity.velocity.y, 0);
                    state = PlayerState.Jumping;
                }
                else if (lastGrounded < Time.time - 0.25f)
                {
                    animator.CrossFade("Jumping", 0.1f);
                    state = PlayerState.Jumping;
                }
            }
            #endregion

            #region Inputs
            var inputMove2 = IngameGameInput.InputMove;
            var inputMove = new Vector3(inputMove2.x, 0, inputMove2.y).normalized;
            if(useCameraView)
                inputMove = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * inputMove;
            var inputRunning = IngameGameInput.InputRunning;
            var inputJumping = IngameGameInput.InputJump;
            #endregion

            #region Jump
            if (inputJumping.trigger && entity.isGrounded)
            {
                entity.velocity.y = 8f;
                onPlayerJump.Invoke();
            }
            #endregion
            
            var isMoving = inputMove.magnitude > 0.15f;
            var deltaAngle = 0f;
            
            #region Rotation
            if (isMoving)
            {
                lastWalkingInput = Time.time;
                var angle = Vector3.SignedAngle(Vector3.forward, inputMove, Vector3.up);
                deltaAngle = Mathf.DeltaAngle(body.eulerAngles.y, angle);
                
                //if (math.abs(deltaAngle) < 50f)
                {
                    body.eulerAngles = new Vector3(0,
                        Mathf.LerpAngle(body.eulerAngles.y, angle, deltaTime * 8f), 0);
                }
            }
            else
            {
                lookDirection = camera.transform.forward;

                var lookPos = /*Quaternion.Euler(0, body.eulerAngles.y, 0) **/ (new Vector3(0, 1.5f, 0) + lookDirection + body.forward);

                riggingLookTarget.localPosition = Vector3.Lerp(
                    riggingLookTarget.localPosition, lookPos, deltaTime * 2f);
            }
            #endregion
            
            
            #region Animations
            var animWalking = animator.GetFloat("walking");
            animator.SetFloat("turning", math.lerp(animator.GetFloat("turning"), math.clamp(math.abs(deltaAngle) < 25f ? 0 : deltaAngle/30f, -1f, 1f), deltaTime * 8f));
            animator.SetFloat("walking", math.lerp(animWalking, isMoving ? (inputRunning.value ? 1 : 0.5f) : 0, deltaTime * 2f));
            
            if((animWalking > 0.5f && Time.time > lastWalkingInput + 0.2f)) 
                animator.CrossFade("Stop", 0.2f);
            #endregion

            #region Rigs
            rigIdle.value = isMoving ? 0 : 1;
            rigLean.value = animWalking;
                
            var leanAngle = math.clamp(-deltaAngle, -10f, 10f);
            riggingLeanRotation.localEulerAngles = new Vector3(0, body.transform.eulerAngles.y, Mathf.LerpAngle(riggingLeanRotation.localEulerAngles.z, leanAngle, deltaTime * 4f));
            #endregion
        }
        
        public void UpdateAttackTimer(float newTime)
        {
            if (newTime < 4 && timeSinceLastAttack >= 4)
                onEnterAttackState.Invoke();
            
            if (newTime >= 4 && timeSinceLastAttack < 4)
                onLeaveAttackState.Invoke();

            timeSinceLastAttack = newTime;
        }
        #endregion

        #region IK
        public void OnAnimatorIK(int layer)
        {
            var deltaTime = Time.deltaTime;

            UpdateGroundRotation();

            CalculateFootIK(HumanBodyBones.LeftFoot, ref leftFootState, deltaTime);
            CalculateFootIK(HumanBodyBones.RightFoot, ref rightFootState, deltaTime);
            
            #region Pelvis IK
            var tg = LerpMinMax(leftFootState.heightChange, rightFootState.heightChange, 0.25f);
            pelvisOffset = Mathf.Lerp(pelvisOffset, tg, deltaTime * 2f);
            #endregion

            #region Lean IK
            var pos = animator.bodyPosition + Vector3.up * pelvisOffset;
            //posY = pos.y = math.min(math.lerp(posY, pos.y, deltaTime * 4f), pos.y);
            animator.bodyPosition = pos;
            #endregion
            
            ApplyFootIK(AvatarIKGoal.LeftFoot,ref leftFootState);
            ApplyFootIK(AvatarIKGoal.RightFoot,ref rightFootState);

            HandleFootSound(ref leftFootState);
            HandleFootSound(ref rightFootState);
        }

        public void HandleFootSound(ref HumanFootState footState)
        {
            var grounded = footState.groundDistance < footThreshold;
            if (grounded && !footState.wasOnGround)
            {
                if (Time.time > footState.footstepTime)
                {
                    footState.footstepTime = Time.time + 0.5f;
                    AudioSystemController.instance.PlaySound(AudioSystem.HashString("step")).At(footState.groundPos);
                }
            }

            footState.wasOnGround = grounded;
        }

        private void CalculateFootIK(HumanBodyBones bone, ref HumanFootState footState,float deltaTime)
        {
            var transform = entity.transform;
            var footPos = animator.GetBoneTransform(bone).position;
            var position = transform.position;
            
            footPos = position + (footState.groundRotation * (footPos - position));

            if (FootCast(footPos + Vector3.up * 0.2f,ref footState))
            {
                footState.weight = Mathf.Lerp(footState.weight, 1f, deltaTime * 4f);

                //Calculate position
                footState.groundObject = footState.hitCache.collider.gameObject;
                footState.groundPos = footState.hitCache.point + Vector3.up * ikFootOffset;
                
                var off = Vector3.up * (footState.groundPos.y - position.y);
                
                //Position            
                footState.offset = Vector3.Lerp(footState.offset, off, deltaTime * 10f);
                
                //Calculate rotation
                var rotAxis = Vector3.Cross(Vector3.up, footState.hitCache.normal);
                var angle = Vector3.Angle(Vector3.up, footState.hitCache.normal);
                var rot = Quaternion.AngleAxis(angle * 1f, rotAxis);

                //Rotation
                footState.rotation = Quaternion.Lerp(footState.rotation, rot, deltaTime * 5f); 
                footState.heightChange = footState.groundPos.y - position.y;
            }
            else
            {
                footState.weight = Mathf.Lerp(footState.weight, 0f, deltaTime * 2f);
                footState.offset = Vector3.Lerp(footState.offset, Vector3.zero, deltaTime * 2f);
                footState.rotation = Quaternion.Lerp(footState.rotation, Quaternion.identity, deltaTime * 2f);

                footState.heightChange = 0;
            }   
        }

        private void ApplyFootIK(AvatarIKGoal goal,ref HumanFootState footState)
        {
            var transform = entity.transform;
            
            //Position
            animator.SetIKPositionWeight(goal, footState.weight);
            animator.SetIKPosition(goal, animator.GetIKPosition(goal) + footState.offset);

            animator.SetIKRotationWeight(goal, footState.weight);
            animator.SetIKRotation(goal, footState.rotation * animator.GetIKRotation(goal));
        
            #if UNITY_EDITOR
            Debug.DrawRay(footState.groundPos, footState.rotation * transform.forward,Color.green);
            #endif
        }

        private bool FootCast(Vector3 pos,ref HumanFootState footState)
        {
            var transform = entity.transform;
            
            var b = false;
            const float off = 0.05f;

            for(var i = 0; i < 1; i ++)
            {
                footState.groundDistance = 100;
                
                if (!Physics.Raycast(pos + transform.forward * (i * off), Vector3.down, out var hit, 0.75f,
                        groundMask)) continue;

                footState.groundDistance = hit.distance;
                
                if(i == 0)
                    footState.hitCache = hit;
                else
                {
                    if(footState.hitCache.point.y < hit.point.y)
                        footState.hitCache = hit;
                }

                b = true;
            }

            return b;
        } 

        private void UpdateGroundRotation()
        {
            var transform = entity.transform;
            
            groundRotation = Quaternion.identity;

            if(Physics.Raycast(transform.position + Vector3.up * 0.5f,Vector3.down,out RaycastHit hitCache,2f,groundMask))
                groundRotation = Quaternion.FromToRotation(body.up, hitCache.normal);

            groundRotation = Quaternion.Slerp(groundRotation, Quaternion.identity, 1f - groundLeanWeight);
        }

        public static float LerpMinMax(float a,float b,float lerp)
        {
            return Mathf.Lerp(b, a, lerp);     
        }
        #endregion
    }
}