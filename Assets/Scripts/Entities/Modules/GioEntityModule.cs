using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class GioEntityModule : EntityModule
    {
        public const float TARGET_DISTANCE_THRESHOLD = 0.5f;
        
        public enum State
        {
            Idling, //Just waiting
            Wandering, //Just walking around
            Targeting, //Targeting someone
            Attacking //Attacking someone
        }

        [Header("SETTINGS")] 
        public float runningDistance = 3f;
        
        [Header("REFERENCES")]
        public Transform body;
        public Animator animator;
        public Vector3 wanderingOrigin;
        
        [Header("STATE")]
        public State state;
        public float lastWalkingInput;
        public Vector3 wanderingStart;
        public float stateTime = 0;
        
        [Header("STATE - PATH")]
        private NavMeshPath _path;
        private int _pathIndex = 0;
        public float pathTime = 0;
    
        public override void OnEnable()
        {
            base.OnEnable();
            wanderingStart = entity.transform.position;
        }

        public override void UpdateFrame(float deltaTime)
        {
            //Drag
            if (entity.isGrounded)
            {
                entity.velocity.x = math.lerp(entity.velocity.x, 0, deltaTime * 8f);
                entity.velocity.z = math.lerp(entity.velocity.z, 0, deltaTime * 8f);
            }
            
            #region Input
            stateTime += deltaTime;
            UpdatePathfinding(deltaTime);
            var inputMove = GetWalkInput(deltaTime, out bool isRunning);
            var isMoving = inputMove.magnitude > 0.15f;
            var deltaAngle = 0f;
            #endregion

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

        #region Setions
        public Vector3 GetWalkInput(float deltaTime, out bool running)
        {
            running = false;
            
            switch (state)
            {
                case State.Attacking:
                    break;
                
                case State.Idling:
                    return Vector3.zero;
                
                case State.Wandering:
                case State.Targeting:
                    if (_path == null || _path.status == NavMeshPathStatus.PathInvalid)
                    {
                        _NewWanderTarget();
                        return Vector3.zero;
                    }
                    
                    if (_pathIndex >= _path.corners.Length)
                    {
                        _OnReachTarget();
                        return Vector3.zero;
                    }
                    
                    //distance & state is State.Targeting
                    pathTime += deltaTime;
                    
                    var waypoint = _path.corners[_pathIndex];
                    var delta = Utils.GetVectorXZ(waypoint) - Utils.GetVectorXZ(entity.transform.position);
                    var distance = delta.magnitude;
                    var direction = delta / distance;
                    
                    running = distance > runningDistance;
                    if (distance < TARGET_DISTANCE_THRESHOLD) //Distance Threshold
                    {
                        _pathIndex++;
                        return Vector3.zero;
                    }
                    return direction;
            }
            
            return Vector3.zero;
        }
        
        public void UpdatePathfinding(float deltaTime)
        {
            switch (state)
            {
                case State.Idling:
                    if (stateTime > 3)
                    {
                        state = State.Wandering;
                        stateTime = 0;
                    }
                    return;
                
                case State.Wandering:
                    if(pathTime > 3)
                        _NewWanderTarget();

                    if (stateTime > 10)
                    {
                        state = State.Idling;
                        stateTime = 0;
                    }
                    return;
                
                case State.Targeting:
                    return;
                
                case State.Attacking:
                    return;
            }
        }
        #endregion

        #region Utils
        protected virtual void _DoPathTo(Vector3 target)
        {
            _path ??= new NavMeshPath();
            NavMesh.CalculatePath(entity.transform.position, target, NavMesh.AllAreas, _path);
            _pathIndex = 0;
            pathTime = 0;
        }
        
        protected virtual void _OnReachTarget()
        {
            _path = null;
            _NewWanderTarget();
        }

        protected virtual void _NewWanderTarget()
        {
            _DoPathTo(wanderingStart + new Vector3(Random.Range(-3, 3), 1, Random.Range(-3, 3)));
        }
        #endregion
    }
}