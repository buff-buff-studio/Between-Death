using System;
using System.Collections;
using Refactor.Data;
using Refactor.Misc;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class EnemyControllerEntityModule : EntityModule
    {
        [Header("SETTINGS")]
        public bool shouldWalk;
        public bool shouldAttack;
        public float attackDistance = 4f;
        public float attackDelay = 4f;
        public float attackRange = 1.25f;
        
        [Header("REFERENCES")]
        public Animator animator;
        public Transform body;
        public Transform attackTarget;
        public GameObject hitParticlesPrefab;
        
        [Header("STATE - AI")]
        public Vector3 target = Vector3.zero;
        private NavMeshPath _path;
        private int _pathIndex = 0;
        public float lastWalkingInput = 0;
        public bool goingToAttack = false;
        public bool isAttacking = false;
        public Vector3 wanderingCenterPoint;
        public float wanderingRadius = 6;

        [Header("SETTINGS - IK")]
        public float ikFootOffset = 0.005f;
        public float groundLeanWeight = 0.25f;
        
        [Header("STATE - IK")]
        public float pelvisOffset;
        public HumanFootState leftFootState;
        public HumanFootState rightFootState;
        public Quaternion groundRotation;
        public LayerMask groundMask;
        public float timeout = 10f;

        protected virtual void OnLineOfSightPlayer()
        {
            
        }
        
        public void NewTarget()
        {
            // saw player
            if (shouldAttack && IsSeeingPlayer())
            {
                target = attackTarget.position;
                RePath(true);
            }
            // wandering
            else
            {
                target = wanderingCenterPoint + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * new Vector3(0, 0, wanderingRadius);
                RePath(false);
            }
        }

        public void RePath(bool isEnemy)
        {
            goingToAttack = isEnemy;
            timeout = Random.Range(3f, 5f);
            _path ??= new NavMeshPath();
            
            NavMesh.CalculatePath(entity.transform.position, target, NavMesh.AllAreas, _path);
            _pathIndex = 0;
        }
        
        public override void OnEnable()
        {
            isAttacking = false;
            goingToAttack = false;
            entity.onChangeElement.AddListener(OnChangeElement);
            NewTarget();
        }

        public override void OnDisable()
        {
            entity.onChangeElement.RemoveListener(OnChangeElement);
        }

        public override void UpdateFrame(float deltaTime)
        {
            if (entity.isGrounded)
            {
                entity.velocity.x = math.lerp(entity.velocity.x, 0, deltaTime * 8f);
                entity.velocity.z = math.lerp(entity.velocity.z, 0, deltaTime * 8f);
            }

            if (isAttacking)
            {
                animator.SetFloat("turning", 0);
                animator.SetFloat("walking", 0);
                var dt = Utils.GetVectorXZ(attackTarget.position - entity.transform.position);
                var angle = Vector3.SignedAngle(Vector3.forward, dt.normalized, Vector3.up);

                body.eulerAngles = new Vector3(0, Mathf.LerpAngle(body.eulerAngles.y, angle, deltaTime * 8f), 0);
                return;
            }

            var inputMove = UpdateWalk(deltaTime, out bool isRunning);
            if (goingToAttack)
            {
                isRunning = Vector3.Distance(entity.transform.position, attackTarget.position) > attackDistance / 2f;
            }
            
            var isMoving = inputMove.magnitude > 0.15f;
            var deltaAngle = 0f;

            #region Rotation
            if (isMoving)
            {
                lastWalkingInput = Time.time;
                var angle = Vector3.SignedAngle(Vector3.forward, inputMove, Vector3.up);
                deltaAngle = Mathf.DeltaAngle(body.eulerAngles.y, angle);
                
                if (math.abs(deltaAngle) < 50f)
                {
                    body.eulerAngles = new Vector3(0,
                        Mathf.LerpAngle(body.eulerAngles.y, angle, deltaTime * 8f), 0);
                }
            }
            #endregion
            
            #region Animations
            var animWalking = animator.GetFloat("walking");
            animator.SetFloat("turning", math.lerp(animator.GetFloat("turning"), math.clamp(math.abs(deltaAngle) < 25f ? 0 : deltaAngle/30f, -1f, 1f), deltaTime * 8f));
            animator.SetFloat("walking", math.lerp(animWalking, isMoving ? (isRunning ? 1f : 0.5f) : 0, deltaTime * 2f));
            
            if((animWalking > 0.5f && Time.time > lastWalkingInput + 0.2f)) 
                animator.CrossFade("Stop", 0.2f);
            #endregion
        }

        protected virtual bool IsSeeingPlayer()
        {
            return Vector3.Distance(entity.transform.position, attackTarget.position) < attackDistance;
        }
        protected virtual bool DistanceToAttack()
        {
            return Vector3.Distance(entity.transform.position, attackTarget.position) < attackRange;
        }
        
        public Vector3 UpdateWalk(float deltaTime,out bool running)
        {
            running = false;
            if ((timeout -= Time.deltaTime) <= 0)
            {
                entity.StartCoroutine(_Next());
                _path = null;
                return Vector3.zero;
            }
            if (!goingToAttack && shouldAttack &&
                IsSeeingPlayer())
            {
                NewTarget();
            }
            
            if (shouldWalk && _path != null && _path.status is not NavMeshPathStatus.PathInvalid)
            {
                if (_pathIndex >= _path.corners.Length)
                {
                    entity.StartCoroutine(_Next());
                    _path = null;
                    return Vector3.zero;
                }
                
                if (goingToAttack && DistanceToAttack())
                {
                    Debug.Log("Attack");
                    entity.StartCoroutine(_Attack());
                    return Vector3.zero;
                }
                
                var waypoint = _path.corners[_pathIndex];
                var delta = Utils.GetVectorXZ(waypoint) - Utils.GetVectorXZ(entity.transform.position);
                var distance = delta.magnitude;
                var direction = delta / distance;
                
                Debug.DrawLine(entity.transform.position + Vector3.up, waypoint + Vector3.up, Color.green, 0.05f);
                running = distance > 3f;
                if (distance < 0.5f)
                    _pathIndex++;
                else
                {
                    return direction;
                }
            }

            return Vector3.zero;
        }

        protected virtual IEnumerator _Attack()
        {
            isAttacking = true;
            animator.CrossFade($"Attack {Random.Range(0, 3)}", 0.25f);
            yield return new WaitForSeconds(0.25f);
            ApplyDamageFor(1, 2);
            yield return new WaitForSeconds(2f);
            isAttacking = false;
            NewTarget();
        }
        
        private void ApplyDamageFor(float damage, float radius)
        {
            var p = entity.transform.position;
            var pos = p + Vector3.up;/* + _controllerEntity.body.rotation * attackOffset;*/
            var fw = body.forward;
            
            foreach (var damageTarget in HealthHelper.GetTargets(pos, radius))
            {
                if(damageTarget.GetGameObject().transform != attackTarget) continue;
                //if(target.GetGameObject() == entity.gameObject) continue;

                var delta = (damageTarget.GetGameObject().transform.position - p);
                var dir = delta.normalized;
                var dot = Vector3.Dot(fw, dir);
                if (dot < 0.1) continue;
                
                var hPos = (damageTarget.GetGameObject().transform.position + pos) / 2f;
                
                damageTarget.Damage(damage);
                
                if (damageTarget.health > 0 && damageTarget is HealthEntityModule module)
                {
                    var e = module.entity;
                    e.velocity = dir * 4f;
                }
                var go = Object.Instantiate(hitParticlesPrefab, hPos, Quaternion.identity);
                Object.Destroy(go, 2f);
            }
        }

        private IEnumerator _Next()
        {
            if (shouldAttack && IsSeeingPlayer())
            {
                NewTarget();
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(2f, 4f));
                NewTarget();
            }
        }

        public void OnChangeElement()
        {
            var elm = entity.element;
            if (elm == Element.None) return;
            
            entity.GetComponentInChildren<Renderer>().material.color = elm.GetColor();
        }
        
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
                if (!Physics.Raycast(pos + transform.forward * (i * off), Vector3.down, out var hit, 0.75f,
                        groundMask)) continue;
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