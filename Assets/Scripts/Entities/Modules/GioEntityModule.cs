using System;
using Refactor.Misc;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
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
            Attacking, //Attacking someone
            Retreating,
            Dodging
        }

        [Header("SETTINGS")] 
        public float runningDistance = 3f;
        
        [Header("REFERENCES")]
        public Transform body;
        public Animator animator;
        public Vector3 wanderingOrigin;
        [SerializeField]private Transform playerRef;
        
        
        [Header("STATE")]
        public State state;
        public float lastWalkingInput;
        public Vector3 wanderingStart;
        public float stateTime = 0;
        
        [Header("STATE - PATH")]
        private NavMeshPath _path;
        private int _pathIndex = 0;
        public float pathTime = 0;


        [Header("STATE - ATTACKING")] 
        [SerializeField] private float attackCollDown = 2f;
        private float _lastAttack = 0;
        [SerializeField]private float distanceToAttack = 1.25f;
        [SerializeField]private float distanceToChasePlayer = 6f;
        private bool _isAttacking;

        [Header("STATE - RETREAT")] [SerializeField]
        private float chanceToRetreat = 5;
        private bool canTurn;
        private float retreatTime = 3f;
        private float _lastRetreat = 0;
        private float distanceBehind = 4;
        
        [Header("STATE - DODGE")] 
        [SerializeField]
        private float chanceToDogde;

        
        public override void OnEnable()
        {
            base.OnEnable();
            wanderingStart = entity.transform.position;
            _pathTime = PathTime();
            _wanderingTime = WanderingTime();
            playerRef =playerRef ? GameObject.FindWithTag("Player").transform : playerRef;
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

            if (canTurn)
            {
                  
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
            }
          
            #endregion
            
            #region Animations
            var animWalking = animator.GetFloat("walking");
            if(canTurn)
                animator.SetFloat("turning", math.lerp(animator.GetFloat("turning"), math.clamp(math.abs(deltaAngle) < 25f ? 0 : deltaAngle/30f, -1f, 1f), deltaTime * 8f));
            else
                animator.SetFloat("turning", 0);
            
            animator.SetFloat("walking", math.lerp(animWalking, isMoving ? state == State.Retreating ? -0.5f : (isRunning ? 1f : 0.5f) : 0, deltaTime * 2f));
            
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
                    
                    if (!DistanceToAttack())
                        state = State.Targeting;
                    
                    if (Time.time > _lastAttack + attackCollDown)
                    {
                        Attack();

                        if (Random.Range(0, 11) < chanceToRetreat)
                            state = State.Retreating;
                        _lastAttack = Time.time;
                    }
                    break;
                
                case State.Idling:
                    if (IsSeeingPlayer())
                        state = State.Targeting;
                    return Vector3.zero;
             
                case State.Targeting or State.Wandering or State.Retreating:
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
                    
                    if(state == State.Targeting)
                    {
                        if (!IsSeeingPlayer())
                            state = State.Wandering;
                        if (DistanceToAttack())
                            state = State.Attacking;
                    }
                    else if(state == State.Wandering)
                    { 
                        if (IsSeeingPlayer())
                            state = State.Targeting;
                    }else if (state == State.Retreating)
                    {
                        canTurn = false;
                        if (Time.time > _lastRetreat + retreatTime)
                        {
                            _lastRetreat = Time.time;
                            state = State.Targeting;
                            canTurn = true;
                        }
                    }
                    
                    
                    pathTime += deltaTime;
                    
                    var waypoint = _path.corners[_pathIndex];
                    var delta = Utils.GetVectorXZ(waypoint) - Utils.GetVectorXZ(entity.transform.position);
                    var distance = delta.magnitude;
                    var direction = delta / distance;
                    
                    running = distance > runningDistance || state == State.Targeting;
                    if (distance < TARGET_DISTANCE_THRESHOLD) //Distance Threshold
                    {
                        _pathIndex++;
                        return Vector3.zero;
                    }
                    
                    return direction;
                
                case State.Dodging:
                    break;
            }
            
            return Vector3.zero;
        }

        private int GetRandomWanderingTime()
        {
            return Random.Range(2, 5);
        }
        
        private int WanderingTime()
        {
            return Random.Range(8, 15);
        }
        
        private int PathTime()
        {
            return Random.Range(2, 4);
        }

        private int _wanderingTime;
        private int _pathTime;
        public void UpdatePathfinding(float deltaTime)
        {
            switch (state)
            {
                case State.Idling:
                    if (stateTime > _pathTime)
                    {
                        state = State.Wandering;
                        stateTime = 0;
                        _pathTime = PathTime();
                    }
                    return;
                
                case State.Wandering:
                    if (pathTime > _pathTime)
                    {
                        _NewWanderTarget();
                        _wanderingTime = WanderingTime();
                    }
                    
                    if (stateTime > _wanderingTime)
                    {
                        state = State.Idling;
                        stateTime = 0;
                        _pathTime = PathTime();
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
            Vector3 target;
            if (IsSeeingPlayer())
                target = playerRef.position;
            else if(state == State.Retreating)
                target = entity.transform.position - (entity.transform.forward * distanceBehind);
            else
                target = wanderingStart + new Vector3(Random.Range(-3, 3), 1, Random.Range(-3, 3));

            
            
            _DoPathTo(target);
        }
        
        protected virtual bool IsSeeingPlayer()
        {
            return Vector3.Distance(playerRef.transform.position, entity.transform.position) < distanceToChasePlayer;
        }

        protected virtual bool DistanceToAttack()
        {
            return Vector3.Distance(playerRef.transform.position, entity.transform.position) < distanceToAttack;
        }
        
        protected virtual void Attack()
        {
            _isAttacking = true;
            animator.CrossFade($"Attack {Random.Range(0, 3)}", 0.25f);
            ApplyDamageFor(1, 2);
        }
        
        private void ApplyDamageFor(float damage, float radius)
        {
            var p = entity.transform.position;
            var pos = p + Vector3.up;/* + _controllerEntity.body.rotation * attackOffset;*/
            var fw = body.forward;
            
            foreach (var damageTarget in HealthHelper.GetTargets(pos, radius))
            {
                if(damageTarget.GetGameObject().transform != playerRef) continue;
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
                /*var go = Object.Instantiate(hitParticlesPrefab, hPos, Quaternion.identity);
                Object.Destroy(go, 2f);*/
            }
            _isAttacking = false;
        }

        #endregion
    }
}