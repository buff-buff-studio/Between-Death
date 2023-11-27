using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class ClownEntity : GioEntityModule
    {
        
        [SerializeField]
        private ProjectileController _controller;
        [SerializeField]
        private Transform shootPoint;
        protected override Vector3 WanderingPos()
        {
            Debug.Log("Wandering");
            return Vector3.zero;
        }

        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            shootPoint.LookAt(target);
        }
        protected override Vector3 TargetPos()
        {
            Vector3 runTo = (playerRef.position - entity.transform.position) * Random.Range(3f,6f);
            return runTo;
        }

        protected override void WanderingState()
        {
            Debug.LogWarning("WAndering");
            if (DistanceToAttack())
            {
                if (timeSinceLastAttack >= attackCollDown && _attackEnded)
                {
                    state = State.Attacking;
                    stateTime = 0;
                    Attack();
                }

            }
            if (IsSeeingPlayer())
            {
                state = State.Targeting;
                stateTime = 0;
            }
        }
        protected override void TargetingState()
        {
            Debug.Log(IsSeeingPlayer());
            if (!IsSeeingPlayer())
            {
                stateTime = 0;
                state = State.Wandering;
            }
            if (DistanceToAttack())
            {
                if (timeSinceLastAttack >= attackCollDown && _attackEnded)
                {
                    state = State.Attacking;
                    stateTime = 0;
                    Attack();
                }
            }  
            
        }
        
        
        
        protected override void Attack()
        {
            Debug.Log("Attack");
            _attackEnded = false;
            animator.CrossFade($"Attack {Random.Range(0, 3)}", 0.25f);
            timeSinceLastAttack = 0;
            _controller.CreateObject(shootPoint.position, playerRef,shootPoint.transform.rotation);
            
            entity.StartCoroutine(OnAnimationFinish(() =>
            {
                _attackEnded = true;
                stateTime = 0;

                if (!RandomBehaviour())
                    state = State.Targeting;
            }));
        }
    }
}