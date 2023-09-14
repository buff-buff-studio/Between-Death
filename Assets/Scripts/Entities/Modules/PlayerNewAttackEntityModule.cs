using System;
using Refactor.Audio;
using Refactor.Data;
using Refactor.Misc;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class AttackState
    {
        public Attack currentAttack;
        public int currentAttackStreak = 0;
        public bool canKeepStreak = true;
        public float timeSinceLastAttack = 0;
        public bool appliedDamage;
    }
    
    [Serializable]
    public class PlayerNewAttackEntityModule : EntityModule
    {
        [Header("SETTINGS")] 
        public float attackRadius = 2f;
        
        [Header("REFERENCES")] 
        public Animator animator;
        public Attack[] attacks;
        public GameObject hitParticlesPrefab;

        [Header("STATE")] 
        public AttackState currentAttackState;
        private PlayerControllerEntityModule _controllerEntity;
        
        public override void OnEnable()
        {
            base.OnEnable();
            _controllerEntity = entity.GetModule<PlayerControllerEntityModule>();
        }
        
        public void ApplyDamage()
        {
            var p = entity.transform.position;
            var fw = _controllerEntity.body.forward;
            
            var pos = p + Vector3.up + fw;/* + _controllerEntity.body.rotation * attackOffset;*/
            
            foreach (var target in HealthHelper.GetTargets(pos, attackRadius))
            {
                if(!entity.element.CanDamage(target.GetElement())) continue;
                //if(target.GetGameObject() == entity.gameObject) continue;

                var delta = (target.GetGameObject().transform.position - p);
                var dir = delta.normalized;
                var dot = Vector3.Dot(fw, dir);
                //if (dot < snapMinDot) continue;
                
                var hPos = (target.GetGameObject().transform.position + pos) / 2f;
                
                target.Damage(currentAttackState.currentAttack.damage);
                AudioSystem.PlaySound("impact").At(hPos);
                
                if (target.health > 0 && target is HealthEntityModule module)
                {
                    //var e = module.entity;
                    //e.velocity = dir * 4f;
                }
                
                //onPlayerDamageVictim.Invoke(target, attack.damageCount);
                
                var go = Object.Instantiate(hitParticlesPrefab, hPos, Quaternion.identity);
                Object.Destroy(go, 2f);
            }
        }
        
        public void PerformAttack(bool resetStreak, bool chained)
        {
            AudioSystem.PlaySound("attack").At(entity.transform.position);
            entity.velocity.x = entity.velocity.z = 0;

            if (resetStreak)
                currentAttackState.currentAttackStreak = 0;
                
            currentAttackState.currentAttack = attacks[currentAttackState.currentAttackStreak%attacks.Length];
            currentAttackState.currentAttackStreak++;
                
            animator.CrossFade(currentAttackState.currentAttack.clipName, currentAttackState.currentAttack.transitionTime, -1, chained ? currentAttackState.currentAttack.chainedStartTime : 0);
            _controllerEntity.state = PlayerState.Attacking;
            
            currentAttackState.timeSinceLastAttack = 0;
            currentAttackState.appliedDamage = false;
        }
        
        public void HandleAttacks(PlayerState state, float deltaTime)
        {
            var attack = IngameGameInput.InputAttack0.trigger;
            currentAttackState.timeSinceLastAttack += deltaTime;
            
            if (state == PlayerState.Attacking)
            {
                var animState = animator.GetCurrentAnimatorStateInfo(0);

                if (animState.IsName(currentAttackState.currentAttack.clipName))
                {
                    if (!currentAttackState.appliedDamage && !currentAttackState.appliedDamage && animState.normalizedTime > currentAttackState.currentAttack.damageTime)
                    {
                        currentAttackState.appliedDamage = true;
                        ApplyDamage();
                    }
                    
                    if (attack && animState.normalizedTime >= currentAttackState.currentAttack.nextAttackWindow)
                    {
                        PerformAttack(false, true);
                    }
                    else if (animState.normalizedTime > 0.9f)
                    {
                        LeaveAttack();
                    }
                }
            }
            else
            {
                if (attack)
                    PerformAttack(!currentAttackState.canKeepStreak, false);

                if (currentAttackState.currentAttack != null && currentAttackState.timeSinceLastAttack > currentAttackState.currentAttack.keepStreakTime)
                    currentAttackState.canKeepStreak = false;
            }
        }

        public void LeaveAttack()
        {
            _controllerEntity.state = PlayerState.Default;
            currentAttackState.timeSinceLastAttack = 0;
            currentAttackState.canKeepStreak = true;
        }
    }
}