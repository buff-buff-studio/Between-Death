using System;
using System.Collections;
using Refactor.Data;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class EnemyControllerEntityModule : EntityModule
    {
        public bool shouldWalk;
        public Animator animator;
        
        public Vector3 target = Vector3.zero;
        private NavMeshPath _path;
        private int _pathIndex = 0;
        public float lastWalkingInput = 0;
        public Transform body;
        
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

        public void NewTarget()
        {
            var offset = Random.onUnitSphere;
            target = new Vector3(Random.Range(-7f, 7f), 0, Random.Range(-7f, 7f));
            RePath();
        }

        public void RePath()
        {
            timeout = Random.Range(3f, 5f);
            _path ??= new NavMeshPath();
            NavMesh.CalculatePath(entity.transform.position, target, NavMesh.AllAreas, _path);
            _pathIndex = 0;
        }
        
        public override void OnEnable()
        {
            entity.onChangeElement.AddListener(OnChangeElement);
            NewTarget();
        }

        public override void OnDisable()
        {
            entity.onChangeElement.RemoveListener(OnChangeElement);
        }

        public override void UpdateFrame(float deltaTime)
        {
            var inputMove = UpdateWalk(deltaTime, out bool isRunning);
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
        
        
        public Vector3 UpdateWalk(float deltaTime,out bool running)
        {
            running = false;
            if ((timeout -= Time.deltaTime) <= 0)
            {
                entity.StartCoroutine(_Next());
                _path = null;
                return Vector3.zero;
            }
            
            if (shouldWalk && _path != null && _path.status is not NavMeshPathStatus.PathInvalid)
            {
                if (_pathIndex >= _path.corners.Length)
                {
                    entity.StartCoroutine(_Next());
                    _path = null;
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

        private IEnumerator _Next()
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            NewTarget();
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