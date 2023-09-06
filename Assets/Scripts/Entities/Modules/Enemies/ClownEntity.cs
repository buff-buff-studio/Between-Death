using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class ClownEntity : GioEntityModule
    {
        protected virtual Vector3 WanderingPos()
        {
            return Vector3.zero;
        }

        protected virtual Vector3 TargetPos()
        {
            return playerRef.position;
        }

        protected override void WanderingState()
        {
            Debug.LogWarning("WAndering");
            if (DistanceToAttack())
            {
                state = State.Attacking;
                stateTime = 0;
                Attack();
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
        
        protected override void Attack()
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
    }
}