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
        private bool isDead;
        
        public enum State
        {  
            Idling, //Just waiting
            Wandering, //Just walking around
            Targeting, //Targeting someone
            Attacking, //Attacking someone
            Retreating,
            Dodging,
            Dizzy,
            TakingDamage,
            Dead,
            WaitingToAttack
        }

        [Header("SETTINGS")] 
        public float runningDistance = 3f;
        
        [Header("REFERENCES")]
        public Transform body;
        public Animator animator;
        public Vector3 wanderingOrigin;
        [SerializeField] protected Transform playerRef;

        [Header("STATE")] 
        [SerializeField]
        private State _state;
        public State state
        {
            get => _state;
            set
            {
                _state = value;
                entity.StopCoroutine(OnAnimationFinish());
            }
        }

        public float lastWalkingInput;
        public Vector3 wanderingStart;
        public float stateTime = 0;
        
        [Header("STATE - PATH")]
        private NavMeshPath _path;
        private int _pathIndex = 0;
        public float pathTime = 0;
        private int _wanderingTime;
        private int _pathTime;

        [Header("STATE - ATTACKING")] 
        [SerializeField] private float attackCollDown = 2f;
        [SerializeField]private float distanceToAttack = 1.25f;
        [SerializeField]private float distanceToChasePlayer = 6f;
        private bool _attackEnded = true;   

        [Header("STATE - RETREAT")] 
        [SerializeField]
        [Range(0,10)]
        private float chanceToRetreat = 5;
        private bool _canTurn = true;
        [SerializeField]
        private float _retreatTime = 4f;
        private float _distanceBehind = 8;
        
        [Header("STATE - DODGE")] 
        [SerializeField]
        [Range(0,10)]
        private float chanceToDodge;
        private float dodgeSpeed = 2f;
        [SerializeField]
        private float _dodgeTime = 2f;

        private Vector3 inputMove;

        [Header("STATE - DIZZY")]
        [SerializeField]
        private float dizzyBarMax;
        [SerializeField]
        private float dizzyBarCurrentValue;
        [SerializeField]
        private float dizzyBarAmountWhenDamage;
        [SerializeField]
        private float dizzyBarAmountRecover;
        [SerializeField]
        private float dizzyTime = 3f;

        [Header("STATE - WAITING TO ATTACK")] 
        [SerializeField]
        [Tooltip("Less than the distance to target player and more than the distance to attack (zero if will not wait for attack")]
        private float distanceToWaitForAttack;
        public bool isGoingToAttack;
   
        [Header("Controller")] 
        public EnemiesInSceneController controller;

   

        public override void OnEnable()
        {
            base.OnEnable();
            wanderingStart = entity.transform.position;
            _pathTime = PathTime();
            _wanderingTime = WanderingTime();
            playerRef =playerRef ? GameObject.FindWithTag("Player").transform : playerRef;
            entity.GetModule<HealthEntityModule>().onHealthChange.AddListener(OnEnemyTakeDamage);
            entity.GetModule<HealthEntityModule>().onDie.AddListener(() =>
            {
                isDead = true;
                state = State.Dead;
                animator.CrossFade("Die", 0.2f);

            });

            controller = controller ? controller : GameObject.FindWithTag("EnemiesController").GetComponent<EnemiesInSceneController>();
            controller.AddEnemy(this);
        }

        public override void UpdateFrame(float deltaTime)
        {
            
            if(isDead) return;
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

            float y = 0;

            if (isMoving)
            {
                switch (state)
                {
                    case State.Retreating:
                        y = -0.5f;
                        break;
                    case State.Dodging:
                        y = -1;
                        break;
                    default:
                        if (isRunning)
                            y = 1;
                        else
                            y = 0.5f;
                        break;
                }
            }
            else
                y = 0;
            

            animator.SetFloat("walking", math.lerp(animWalking, y, deltaTime * speed));
            
            if((animWalking > 0.5f && Time.time > lastWalkingInput + 0.2f)) 
                animator.CrossFade("Stop", 0.2f);
            #endregion

            dizzyBarCurrentValue += deltaTime * dizzyBarAmountRecover;
            dizzyBarCurrentValue = Mathf.Min(dizzyBarMax, dizzyBarCurrentValue);
        }

        #region OnStateUpdate

        protected virtual void DodgeState()
        {
            state = State.Dodging;
            /*entity.velocity.x = inputMove.x * dodgeSpeed;
            entity.velocity.y = 0.2f;
            entity.velocity.z = inputMove.z * -dodgeSpeed;*/
            _canTurn = false;
            //stateTime = 0;
        }
        protected virtual void AttackState()
        {
            _path.ClearCorners();
            _pathIndex = 0;
            if (stateTime >= attackCollDown && _attackEnded)
                Attack();

        }
        protected virtual void IdleState()
        {
            if (IsSeeingPlayer())
            {
                state = State.Targeting;
                stateTime = 0;
            }
        }
        protected virtual void TargetingState()
        {
            Debug.Log(IsSeeingPlayer());
            if (!IsSeeingPlayer())
            {
                stateTime = 0;
                state = State.Wandering;
            }
            if (DistanceToAttack())
            {
                state = State.Attacking;
                stateTime = 0;
                Attack();
            }  
           
            if(distanceToWaitForAttack <= 0) return;
            if(isGoingToAttack) return;
            if(!controller.HasMoreThanOne()) return;
            if (DistanceToWaitToAttack())
            {
                state = State.WaitingToAttack;
                _NewWanderTarget();
                stateTime = 0;
            }
        }
        protected virtual void WanderingState()
        {
            Debug.LogWarning("WAndering");
            if (IsSeeingPlayer())
            {
                state = State.Targeting;
                stateTime = 0;
            }
        }
        protected virtual void RetreatState()
        {
            _canTurn = false;
        }
        protected virtual void DizzyState()
        {
            animator.CrossFade("Dizzy", 0.2f);
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
        
        protected virtual void WaitingToAttack()
        {
            controller.routineAttacking = true;

            if (isGoingToAttack)
                state = State.Targeting;
            
        }
        #endregion
        
        
        #region Setions

        public virtual void OnEnemyTakeDamage(float amount)
        {
            
            // damage animatio
            //state = State.TakingDamage;

            animator.CrossFade("Reaction", 0.25f);
            
            if(state == State.Dizzy) return;
            DecreaseDizzyBar();
            
         
        }
        
        private void DecreaseDizzyBar()
        {
            dizzyBarCurrentValue -= dizzyBarAmountWhenDamage;

            if (dizzyBarCurrentValue <= 0)
                DizzyState();
            
        }
        public Vector3 GetWalkInput(float deltaTime, out bool running)
        {
         
            running = false;
            
            if(isDead) return Vector3.zero;
            
            pathTime += deltaTime;
            switch (state)
            {
                case State.Attacking:
                    AttackState();
                    break;
                
                case State.Idling:
                    IdleState();
                    return Vector3.zero;
                      
                case State.Dizzy:
                    if (stateTime > dizzyTime)
                    {
                        stateTime = 0;
                        state = State.Targeting;
                        dizzyBarCurrentValue = dizzyBarMax;
                        animator.CrossFade("Stop", 0.2f);
                    }
                    return Vector3.zero;
                
                case State.Targeting or State.Wandering or State.Retreating or State.Dodging or State.WaitingToAttack:
                    
                    if (_path == null|| _path.status == NavMeshPathStatus.PathInvalid)
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
                            TargetingState();
                            break;
                        }
                        case State.Wandering:
                        {
                            WanderingState();
                            controller.RemoveEnemy(this);
                            break;
                        }
                        case State.Retreating:
                            RetreatState();
                            break;
                        case State.Dodging:
                            DodgeState();
                            break;
                        case State.WaitingToAttack:
                            WaitingToAttack();
                            break;
                    }

                    
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

        protected virtual Vector3 WaitingToAttackNavMesh()
        {
            return Vector3.zero;
        }

        protected virtual void _NewWanderTarget()
        {
            Debug.Log("New wander target");
            Vector3 target = Vector3.zero;
            if (state == State.Retreating || state == State.Dodging)
            {
                target = entity.transform.position - (entity.transform.forward * _distanceBehind);
                
            }else if (state == State.WaitingToAttack)
            {
                target = WaitingToAttackNavMesh();
            }
            else if (IsSeeingPlayer())
            {
                Debug.Log("Player");
                target = playerRef.position;
            }
            else
                target = wanderingStart + new Vector3(Random.Range(-3, 3), 1, Random.Range(-3, 3));
            
            _DoPathTo(target);
        }
 
        
        protected virtual bool IsSeeingPlayer()
        {
            return Vector3.Distance(playerRef.transform.position, entity.transform.position) <= distanceToChasePlayer;
        }

        protected virtual bool DistanceToAttack()    
        {
            return Vector3.Distance(playerRef.transform.position, entity.transform.position) <= distanceToAttack;
        }
        
        protected virtual bool DistanceToWaitToAttack()    
        {
            return Vector3.Distance(playerRef.transform.position, entity.transform.position) <= distanceToWaitForAttack;
        }
        
        protected virtual void Attack()
        {
            _attackEnded = false;
            animator.CrossFade($"Attack {Random.Range(0, 3)}", 0.25f);
            ApplyDamageFor(1, 2);
            
            entity.StartCoroutine(OnAnimationFinish(() =>
            {
                _attackEnded = true;
                stateTime = 0;

                if (!RandomBehaviour())
                    state = State.Targeting;
                
            }));
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
        }


        private float RandomNumber()
        {
            return Random.Range(1f, 11f);
        }
        
        public virtual bool RandomBehaviour()
        {
            if (RandomNumber() <= chanceToRetreat)
            {
                state = State.Retreating;
                stateTime = 0;
                return true;
            }
            
            if (RandomNumber() <= chanceToDodge)
            {
                entity.StartCoroutine(HandleDodgeCoroutine());
                DodgeState();
                return true;
            }

            return false;
        }
        
        
        private IEnumerator OnAnimationFinish(Action callback = null)
        {
            yield return new WaitForSeconds(0.2f);

            yield return new WaitUntil(() =>animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f);
            
            callback?.Invoke();
        }


        #endregion
    }
}