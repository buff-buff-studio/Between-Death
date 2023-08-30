using System;
using System.Collections;
using Refactor.Audio;
using Refactor.Data;
using Refactor.Misc;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class Attack
    {
        /// <summary>
        /// Defines the animation clip name
        /// </summary>
        public string clipName;

        /// <summary>
        /// Damage count
        /// </summary>
        public float damage = 1f;
        
        /// <summary>
        /// Defines the transition time from the last state to the attacking state
        /// </summary>
        [Header("TIMINGS")]
        public float transitionTime = 0;

        /// <summary>
        /// Defines when the damage should be applied
        /// </summary>
        public float damageTime = 0.5f;
        
        /// <summary>
        /// Defines after how many time player can chain the next attack
        /// </summary>
        [Header("AFTER TIMING")]
        public float nextAttackWindow = 0.5f;
        
        /// <summary>
        /// Defines how much time after this attack the player can keep the combo
        /// </summary>
        public float keepStreakTime = 1f;
        
        /// <summary>
        /// Defines when the attacking animation should start when played chained withing last attack
        /// </summary>
        public float chainedStartTime = 0f;
    }
    
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
                    var e = module.entity;
                    e.velocity = dir * 4f;
                }
                
                //onPlayerDamageVictim.Invoke(target, attack.damageCount);
                
                var go = Object.Instantiate(hitParticlesPrefab, hPos, Quaternion.identity);
                Object.Destroy(go, 2f);
            }
        }
        
        public void PerformAttack(bool resetStreak, bool chained)
        {
            AudioSystem.PlaySound("attack").At(entity.transform.position);

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
                        _controllerEntity.state = PlayerState.Default;
                        currentAttackState.timeSinceLastAttack = 0;
                        currentAttackState.canKeepStreak = true;
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
    }
}