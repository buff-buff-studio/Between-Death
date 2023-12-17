using System;
using System.Collections.Generic;
using Refactor.Audio;
using Refactor.Data;
using Refactor.Data.Variables;
using Refactor.Misc;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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
        public InventoryData inventoryData;
        public SkillList skillList;
        public FloatVariable lifeDrainPercentage;

        [Header("STATE")] 
        public AttackState currentAttackState;
        private PlayerControllerEntityModule _controllerEntity;
        private IHealth _healthEntity;
        private float _targetAngle = 0;

        public UnityEvent<int> onUseSkill;

        public override void OnEnable()
        {
            base.OnEnable();
            _controllerEntity = entity.GetModule<PlayerControllerEntityModule>();
            _healthEntity = entity.GetModule<HealthEntityModule>();
        }

        public override void UpdatePhysics(float deltaTime)
        {
            foreach (var skill in skillList.skills)
            {
                skill.actualCooldown -= deltaTime;
                if (skill.actualCooldown < 0)
                    skill.actualCooldown = 0;
            }
        }
        
        public void ApplyDamage()
        {
            var p = entity.transform.position;
            var fw = _controllerEntity.body.forward;
            var pos = p + Vector3.up + fw;
            
            foreach (var target in HealthHelper.GetTargets(pos, attackRadius))
            {
                if(!entity.element.CanDamage(target.GetElement())) continue;
                if (target.health == 0) continue;
              
                var delta = (target.GetGameObject().transform.position - p);
                var dir = delta.normalized;
                var dot = Vector3.Dot(fw, dir);
                if (dot < 0) continue;
                
                var hPos = (target.GetGameObject().transform.position + pos) / 2f;
                
                target.Damage(currentAttackState.currentAttack.damage);
                AudioSystem.PlaySound("impact").At(hPos);
                _healthEntity.Heal((currentAttackState.currentAttack.damage * lifeDrainPercentage.FinalValue)/100);
                
                var go = Object.Instantiate(hitParticlesPrefab, hPos, Quaternion.identity);
                Object.Destroy(go, 2f);
            }
        }

        public void PerformSkill(SkillData skill, bool chained)
        {
            if (skill == null) 
                return;
            if (skill.actualCooldown > 0)
                return;
            
            entity.velocity.x = entity.velocity.z = 0;
            PerformAttack(chained, skill.GetAttack());
            skill.actualCooldown = skill.cooldown;
        }
        
        public void PerformAttack(bool resetStreak, bool chained)
        {
            if (resetStreak)
                currentAttackState.currentAttackStreak = 0;

            PerformAttack(chained, attacks[currentAttackState.currentAttackStreak % attacks.Length]);
        }

        public void PerformAttack(bool chained, Attack attack)
        {
            AudioSystem.PlaySound("attack").At(entity.transform.position);
            entity.velocity.x = entity.velocity.z = 0;
            
            currentAttackState.currentAttack = attack;
            currentAttackState.currentAttackStreak++;
                
            animator.CrossFade(currentAttackState.currentAttack.clipName, currentAttackState.currentAttack.transitionTime, -1, chained ? currentAttackState.currentAttack.chainedStartTime : 0);
            _controllerEntity.state = PlayerState.Attacking;
            
            currentAttackState.timeSinceLastAttack = 0;
            currentAttackState.appliedDamage = false;

            _controllerEntity.UpdateAttackTimer(0);

            DoSnap();
        }
        
        public void DoSnap()
        {
            var p = entity.transform.position;
            var fw = _controllerEntity.body.forward;
            
            var pos = p + Vector3.up + fw;
            var body = _controllerEntity.body;

            Camera cam = Camera.main!;
            _targetAngle = cam.transform.eulerAngles.y;

            IHealth snapTarget = null;
            float snapGreatest = 0;
                
            foreach (var target in HealthHelper.GetTargets(pos, attackRadius))
            {
                if(!entity.element.CanDamage(target.GetElement())) continue;
                if (target.health == 0) continue;

                if (target is HealthEntityModule hem)
                {
                    var gem = hem.entity.GetModule<GioEntityModule>();
                    gem.RandomBehaviour();
                }
                
                var delta = (target.GetGameObject().transform.position - p);
                var dir = delta.normalized;
                var dot = Vector3.Dot(fw, dir);
                
                if(dot < 0)
                    continue;
                
                if(snapTarget == null || dot > snapGreatest)
                {
                    snapTarget = target;
                    snapGreatest = dot;
                }
            }

            if (snapTarget != null)
            {
                var delta = (snapTarget.GetGameObject().transform.position - p);
                var dir = delta.normalized;
   
                float angle = Vector3.SignedAngle(Vector3.forward, new Vector3(dir.x, 0, dir.z), Vector3.up);
                body.eulerAngles = new Vector3(0, angle, 0);
            }
        }
        
        public void HandleAttacks(PlayerState state, float deltaTime)
        {
            var attack = IngameGameInput.InputAttack0.trigger;
            currentAttackState.timeSinceLastAttack += deltaTime;
            
            if (state == PlayerState.Attacking)
            {
                var body = _controllerEntity.body;
                body.eulerAngles = new Vector3(0, Mathf.LerpAngle(body.eulerAngles.y, _targetAngle, deltaTime * 8f), 0);
            
                var animState = animator.GetCurrentAnimatorStateInfo(0);

                if (animState.IsName(currentAttackState.currentAttack.clipName))
                {
                    if (!currentAttackState.appliedDamage && !currentAttackState.appliedDamage && animState.normalizedTime > currentAttackState.currentAttack.damageTime)
                    {
                        currentAttackState.appliedDamage = true;
                        ApplyDamage();
                    }

                    bool rightWindow = animState.normalizedTime >= currentAttackState.currentAttack.nextAttackWindow;

                    if (rightWindow)
                    {
                        if (IngameGameInput.InputSkill0.trigger)
                        {
                            SkillData data = skillList.Get(inventoryData.GetEquippedSkill(0));
                            if (data.element != entity.element) return;

                            PerformSkill(data, true);
                            onUseSkill.Invoke(0);
                        }
                        else if (IngameGameInput.InputSkill1.trigger)
                        {
                            SkillData data = skillList.Get(inventoryData.GetEquippedSkill(1));
                            if (data.element != entity.element) return;

                            PerformSkill(data, true);
                            onUseSkill.Invoke(1);
                        }
                        else if (IngameGameInput.InputSkill2.trigger)
                        {
                            SkillData data = skillList.Get(inventoryData.GetEquippedSkill(2));
                            if (data.element != entity.element) return;

                            PerformSkill(data, true);
                            onUseSkill.Invoke(2);
                        }
                        else if (attack)
                        {
                            PerformAttack(false, true);
                        }
                    }
                    

                    if (animState.normalizedTime > 0.9f)
                    {
                        LeaveAttack();
                    }
                }
            }
            else
            {
                if (IngameGameInput.InputSkill0.trigger)
                {
                    SkillData data = skillList.Get(inventoryData.GetEquippedSkill(0));
                    PerformSkill(data, false);
                    onUseSkill.Invoke(0);
                }
                else if (IngameGameInput.InputSkill1.trigger)
                {
                    SkillData data = skillList.Get(inventoryData.GetEquippedSkill(1));
                    PerformSkill(data, false);
                    onUseSkill.Invoke(1);
                }
                else if (IngameGameInput.InputSkill2.trigger)
                {
                    SkillData data = skillList.Get(inventoryData.GetEquippedSkill(2));
                    PerformSkill(data, false);
                    onUseSkill.Invoke(2);
                }
                else if (attack)
                {
                    PerformAttack(!currentAttackState.canKeepStreak, false);
                }
 
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