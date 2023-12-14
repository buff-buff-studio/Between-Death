using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class TankEntity : GioEntityModule
    {

        [SerializeField]
        private bool onSpecialAttack;
        [Range(0, 10)]
        [SerializeField]
        private float chanceToSpecialAttack = 3;
        [SerializeField]
        private float timeOnSpecialAttack = 5;
        [SerializeField]
        private float distanceToSpecialAttack = 8;
        [SerializeField]
        private float damageRadius = 4;

        [SerializeField] private LayerMask triggerlayer;
        [SerializeField] private Transform rayCastPlace;

        private GameObject playerRefPos = null;
        private static readonly int RushAttackOut = Animator.StringToHash("rushAttackOut");

        /*protected override float AttackAnimation()
        {
            return 1.5f;
        }*/
        
        private void MoveObject(Vector3 pos)
        {
            if (playerRefPos == null)
                playerRefPos = new GameObject();

            playerRefPos.transform.position = pos;
        }
        protected override Vector3 OnAttackPos()
        {
            if (onSpecialAttack)
            {
                return (playerRefPos.transform.position - entity.transform.position) * 500;
            }
            Debug.Log("onAttackPos");
            return Vector3.zero;
        }

        protected override void OnReachTarget()
        {
            if (state == State.Attacking && onSpecialAttack)
            {
                SetDizzy();
            }
        }
        
        protected override void Attack()
        {
            _attackEnded = false;
            onSpecialAttack = false;
            timeSinceLastAttack = 0;
            stateTime = 0;
            Debug.Log("NORMAL ATTACK");
            _NewWanderTarget();
            NormalAttack();
        }
        private void SpecialAttack()
        {
            _attackEnded = false;
            MoveObject(playerRef.position);
            timeSinceLastAttack = 0;
            stateTime = 0;
            onSpecialAttack = true;
            animator.CrossFade("RushAttackIN", 0.1f);
      
          //  animator.CrossFade($"Attack {Random.Range(0, 3)}", 0.25f);
        }
        protected virtual bool DistanceToSpecialAttack()    
        {
            return Vector3.Distance(playerRef.transform.position, entity.transform.position) <= distanceToSpecialAttack;
        }

        protected override void TargetingState()
        {
            if (!IsSeeingPlayer())
            {
                stateTime = 0;
                state = State.Wandering;
                if (_path != null)
                {
                    _path.ClearCorners();
                    _pathIndex = 0;
                }
            }
     
            if (DistanceToAttack())
            {
                _NewWanderTarget();
                onSpecialAttack = true;
               state = State.Attacking;
                stateTime = 0;
                Attack();
                if (_path != null)
                {
                    _path.ClearCorners();
                    _pathIndex = 0;
                }
                return;
                //  if(timeSinceLastAttack >= attackCollDown)
             
            }
            
            if (DistanceToSpecialAttack())
            {
                if (Random.Range(0, 11) < chanceToSpecialAttack)
                {
                    _NewWanderTarget();
                    onSpecialAttack = true;
                    Debug.Log("Special attack");
                    if (_path != null)
                    {
                        _path.ClearCorners();
                        _pathIndex = 0;
                    }
                    state = State.Attacking;
                    stateTime = 0;
                    
                    SpecialAttack(); 
                }
            }
       
        }

        private void SetDizzy()
        {
            
            state = State.Dizzy;
            
            DizzyState();
            stateTime = 0;
            onSpecialAttack = false;
            /*if (onSpecialAttack)
            {
                animator.SetTrigger("rushAttackOut");
                onSpecialAttack = false;
            }*/
        }
        
        
        protected override void AttackState()
        {
            /*_path.ClearCorners();
            _pathIndex = 0;*/
            Debug.Log("AttackState");
            
            if (onSpecialAttack)
            {
                if (Physics.SphereCast(rayCastPlace.transform.position, 2,entity.transform.forward, out RaycastHit hit, 0.8f, triggerlayer,
                        QueryTriggerInteraction.UseGlobal))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        ApplyDamageFor(attackDamage * 2, 3);
                        SetDizzy();
                    }
                    
                }
                else if (stateTime >= timeOnSpecialAttack)
                {
                    ApplyDamageFor(attackDamage * 2, 3);
                    SetDizzy();
                }
            }
            else
            {
                if (!DistanceToAttack())
                    state = State.Targeting;

                if (timeSinceLastAttack >= attackCollDown && _attackEnded)
                    Attack();
                
            }
        }

        protected override void OnDizzyState()
        {
            
            if (stateTime > dizzyTime)
            {
                stateTime = 0;
                if (DistanceToAttack())
                {
                    Debug.Log("Distance to attack");
                    onSpecialAttack = false;
                    stateTime = 0;
                    Attack();
                }
                else
                {
                    stateTime = 0;
                    state = State.Targeting;
                    if (_path != null)
                    {
                        _path.ClearCorners();
                        _pathIndex = 0;
                    }
                }
                //  state = State.Targeting;
                dizzyBarCurrentValue = dizzyBarMax;
                animator.CrossFade("Stop", 0.2f);
                if (_path != null)
                {
                    _path.ClearCorners();
                    _pathIndex = 0;
                }
            }
        }

        private void NormalAttack()
        {
            animator.CrossFade($"Attack {Random.Range(0, 3)}", 0.25f);

            ApplyDamageFor(attackDamage, damageRadius);
            Debug.Log("Attack");
            entity.StartCoroutine(OnAnimationFinish(() =>
            {
                _attackEnded = true;    
                Debug.Log("Attack finished");
                RandomBehaviour();
                /*if (!RandomBehaviour())
                    state = State.Attacking;*/
            },0.5f));
        }
    }
}