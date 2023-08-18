using System;
using System.Collections;
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

        [Header("STATE - RETREAT")] 
        [SerializeField]
        [Range(0,10)]
        private float chanceToRetreat = 5;
        private bool _canTurn = true;
        private float _retreatTime = 4f;
        private float _distanceBehind = 1;
        
        [FormerlySerializedAs("chanceToDogde")]
        [Header("STATE - DODGE")] 
        [SerializeField]
        [Range(0,10)]
        private float chanceToDodge;
        private float dodgeSpeed = 2f;
        private float _dodgeTime = 3f;

        private Vector3 inputMove;
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
            inputMove = GetWalkInput(deltaTime, out bool isRunning);
            var isMoving = inputMove.magnitude > 0.15f;
            var deltaAngle = 0f;
            #endregion

            #region Rotation

            if (_canTurn)
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
            if(_canTurn)
                animator.SetFloat("turning", math.lerp(animator.GetFloat("turning"), math.clamp(math.abs(deltaAngle) < 25f ? 0 : deltaAngle/30f, -1f, 1f), deltaTime * 8f));
            else
                animator.SetFloat("turning", 0);


            var speed = state == State.Dodging ? 4f : 2f;
            animator.SetFloat("walking", math.lerp(animWalking, isMoving ? state == State.Retreating || state == State.Dodging  ? -0.5f : (isRunning ? 1f : 0.5f) : 0, deltaTime * speed));
            
            if((animWalking > 0.5f && Time.time > lastWalkingInput + 0.2f)) 
                animator.CrossFade("Stop", 0.2f);
            #endregion
        }

        private void SetDodge()
        {
            state = State.Dodging;
            entity.velocity.x = inputMove.x * dodgeSpeed;
            entity.velocity.y = 0.2f;
            entity.velocity.z = inputMove.z * -dodgeSpeed;
            _canTurn = false;
            
        }

        private IEnumerator HandleDodgeCoroutine()
        {
            const int count = 4;

            for (var i = 1; i < count; i++)
            {
                yield return new WaitForSeconds(_dodgeTime / count);
                entity.GetModule<CloneEntityModule>()?.Clone(0.25f);
            }
        }

        private IEnumerator OnAttackAnimationFinish()
        {
            yield return new WaitForSeconds(0.2f);

            yield return new WaitUntil(() =>animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
            Debug.Log("Animtion finished");
            
            if (Random.Range(0, 11) <= chanceToRetreat)
                state = State.Retreating;

            if (Random.Range(0, 11) <= chanceToDodge)
            {
                entity.StartCoroutine(HandleDodgeCoroutine());
                SetDodge();
            }
         
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
                        _lastAttack = Time.time;
                        /*if (Random.Range(0, 11) < chanceToRetreat)
                            state = State.Retreating;

                        if (Random.Range(0, 11) < chanceToDodge)
                        {
                            entity.StartCoroutine(HandleDodgeCoroutine());
                            SetDodge();
                        }*/

                        entity.StartCoroutine(OnAttackAnimationFinish());

                    }

                    break;
                
                case State.Idling:
                    if (IsSeeingPlayer())
                        state = State.Targeting;
                    return Vector3.zero;
             
                case State.Targeting or State.Wandering or State.Retreating or State.Dodging:
                    
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

                    switch (state)
                    {
                        case State.Targeting:
                        {
                            if (!IsSeeingPlayer())
                                state = State.Wandering;
                            if (DistanceToAttack())
                                state = State.Attacking;
                            break;
                        }
                        case State.Wandering:
                        {
                            if (IsSeeingPlayer())
                                state = State.Targeting;
                            break;
                        }
                        case State.Retreating:
                            _canTurn = false;
                            break;
                        case State.Dodging:
                            SetDodge();
                            break;
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
                
                case State.Retreating:
                    if (stateTime > _retreatTime)
                    {
                        state = State.Targeting;
                        stateTime = 0;
                        _canTurn = true;
                    }
                    return;
                
                case State.Dodging:
                    if (stateTime > _dodgeTime)
                    {
                        state = State.Targeting;
                        stateTime = 0;
                        _canTurn = true;
                    }
                    break;
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
            else if(state == State.Retreating || state == State.Dodging)
                target = entity.transform.position - (entity.transform.forward * _distanceBehind);
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