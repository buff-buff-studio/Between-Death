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
        private float chanceToSpecialAttack = 5;
        [SerializeField]
        private float timeOnSpecialAttack = 5;
        [SerializeField]
        private float distanceToSpecialAttack = 8;
        [SerializeField]
        private float damageRadius = 4;

        [SerializeField] private LayerMask triggerlayer;
        
        protected override float AttackAnimation()
        {
            return 1.5f;
        }
        
        protected override Vector3 OnAttackPos()
        {
            if (onSpecialAttack)
            {
                return entity.transform.forward * 500;
            }
            return Vector3.zero;
        }     
        
        protected override void Attack()
        {
            _attackEnded = false;
            timeSinceLastAttack = 0;
            stateTime = 0;
            NormalAttack();
        }
        private void SpecialAttack()
        {
            _attackEnded = false;
            timeSinceLastAttack = 0;
            stateTime = 0;
            onSpecialAttack = true;
            animator.CrossFade($"Attack {Random.Range(0, 3)}", 0.25f);
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
            }

            if (DistanceToSpecialAttack())
            {
                if (Random.Range(0, 11) < chanceToSpecialAttack)
                {
                    state = State.Attacking;
                    stateTime = 0;
                    onSpecialAttack = true;
                    SpecialAttack(); 
                }
            }
            
            if (DistanceToAttack())
            {
                state = State.Attacking;
                stateTime = 0;
                if(timeSinceLastAttack >= attackCollDown)
                    Attack();
            }
        }

        protected override void AttackState()
        {
            /*_path.ClearCorners();
            _pathIndex = 0;*/
            Debug.Log("AttackState");
            
            if (onSpecialAttack)
            {
                if (Physics.SphereCast(entity.transform.position, 20,entity.transform.forward, out RaycastHit hit, 1f, triggerlayer,
                        QueryTriggerInteraction.UseGlobal))
                {
                    // set dizzy
                    if (hit.transform.CompareTag("Player"))
                    {
                        ApplyDamageFor(1, 3);
                    }
                    state = State.Dizzy;
                    DizzyState();
                    stateTime = 0;
                    onSpecialAttack = false;
                 
                }
                else if (stateTime >= timeOnSpecialAttack)
                {
                    Debug.Log("Finish");
                    state = State.Dizzy;
                    DizzyState();
                    stateTime = 0;
                    onSpecialAttack = false;
                    return;
                    if (!DistanceToSpecialAttack())
                        state = State.Targeting;
                    
                    onSpecialAttack = false;
                    if (DistanceToAttack())
                    {
                        _attackEnded = true;
                        Attack();
                    }
                    else
                    {
                        stateTime = 0;
                        onSpecialAttack = true;
                        SpecialAttack();
                    }
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
                    stateTime = 0;
                    Attack();
                }
                else
                {
                    stateTime = 0;
                    state = State.Targeting;
                }
                state = State.Targeting;
                dizzyBarCurrentValue = dizzyBarMax;
                animator.CrossFade("Stop", 0.2f);
            }
        }

        private void NormalAttack()
        {
            animator.CrossFade($"Attack {Random.Range(0, 3)}", 0.25f);

            ApplyDamageFor(1, damageRadius);
            Debug.Log("Attack");
            entity.StartCoroutine(OnAnimationFinish(() =>
            {
                _attackEnded = true;    
                Debug.Log("Attack finished");
                RandomBehaviour();
                /*if (!RandomBehaviour())
                    state = State.Attacking;*/
            }));
        }
    }
}