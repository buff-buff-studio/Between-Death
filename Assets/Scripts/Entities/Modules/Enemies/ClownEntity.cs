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
            Vector3 runTo;
            if (Random.Range(0, 11) < 1)
            {
                runTo = (playerRef.position - entity.transform.position) * Random.Range(1f,6f);
            }
            else
            {
                runTo = Vector3.zero;
            }
            return runTo;
        }
        [SerializeField]
        private float runTime = 3;

        protected override void WanderingState()
        {
            _canTurn = true;
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

            if (stateTime <= runTime) return;
            if (IsSeeingPlayer())
            {
                state = State.Targeting;
                stateTime = 0;
            }
        }
        protected override void TargetingState()
        {
            /*if (stateTime >= runTime)
            {
                stateTime = 0;
                state = State.Wandering;
                if (_path != null)
                {
                    _path.ClearCorners();
                    _pathIndex = 0;
                }
            }*/
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
            
            entity.StartCoroutine(OnAnimationFinish(() =>
            {
                _controller.CreateObject(shootPoint.position, playerRef,shootPoint.transform.rotation);
                _attackEnded = true;
                stateTime = 0;

                if (!RandomBehaviour())
                    state = State.Targeting;
            }));
        }
    }
}