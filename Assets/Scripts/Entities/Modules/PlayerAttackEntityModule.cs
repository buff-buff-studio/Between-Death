using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using Refactor.Misc;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Refactor.Entities.Modules
{
    
    [Serializable]
    public class PlayerAttackEntityModule : EntityModule
    {
        [Header("SETTINGS")] 
        public Vector3 attackOffset;
        public float attackRadius = 1f;
        public float snapMinDot = 0.5f;
        
        [Header("REFERENCES")] 
        public Animator animator;
        public float angle;
        public TrailRenderer[] attackTrails;
        public GameObject hitParticlesPrefab;
        
        [Header("ATTACKS")] 
        public AttackCombo[] combos;
        public bool canAttack = true;

        [Header("STATE")] 
        public List<AttackCombo> possibleCombos = new();
        public List<AttackCombo.AttackType> inputOrder = new();
        public bool hasDamaged;
        
        [Header("EVENTS")] 
        public UnityEvent<AttackCombo, int> onPlayerPerformAttack;
        public UnityEvent onPlayerFailCombo;
        public UnityEvent<IHealth, float> onPlayerDamageVictim;
        
        private AttackCombo _currentCombo;
        private AttackCombo.Attack _currentAttack;
        private PlayerControllerEntityModule _controllerEntity;

        public override void OnEnable()
        {
            base.OnEnable();
            _controllerEntity = entity.GetModule<PlayerControllerEntityModule>();
        }

        public void DoSnap()
        {
            var closest = 10f;
            IHealth closestTarget = null;
            
            var pos = entity.transform.position;
            var fw = _controllerEntity.body.forward;
            foreach (var possibleTarget in HealthHelper.GetTargets(entity.transform.position, 3f))
            {
                if(possibleTarget.GetGameObject() == entity.gameObject) continue;
                //if (possibleTargets is not HealthEntityModule) continue;

                var delta = (possibleTarget.GetGameObject().transform.position - pos);
                var distance = delta.magnitude;
                var dir = delta.normalized;
                //delta = new Vector3(delta.x, 0, delta.z).normalized;
                var dot = Vector3.Dot(fw, dir);

                if (dot > snapMinDot && distance < closest)
                {
                    closest = distance;
                    closestTarget = possibleTarget;
                }
            }
            
            if (closestTarget == null) return;

            var target = (closestTarget.GetGameObject().transform.position - pos);
            target = new Vector3(target.x, 0, target.z).normalized;
            var angleToEnemy = Vector3.SignedAngle(Vector3.forward, target, Vector3.up);
            _controllerEntity.body.eulerAngles = new Vector3(0, angleToEnemy, 0);
        }

        public void PerformAttacks(PlayerState state)
        {
            if (!canAttack)
                return;

            var attackLeft = IngameGameInput.InputAttack0.trigger;
            var attackRight = IngameGameInput.InputAttack1.trigger;
            var isAttacking = attackLeft || attackRight;
            
            if (!isAttacking) return;

            if (state != PlayerState.Attacking)
            {
                //Start attack
                inputOrder.Clear();
                possibleCombos.Clear();
                
                foreach (var combo in combos)
                {
                    if(CheckRules(combo))
                        possibleCombos.Add(combo);
                }
            }
            
            switch (attackLeft)
            {
                case true when !attackRight:
                    inputOrder.Add(AttackCombo.AttackType.Left);
                    ProcessAttack();
                    break;
                case false:
                    inputOrder.Add(AttackCombo.AttackType.Right);
                    ProcessAttack();
                    break;
                default:
                    OnFailCombo();
                    break;
            }
        }

        public void HandleAttack()
        {
            var attack = _currentAttack;
            var state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName(attack.clipName))
            {
                if (state.normalizedTime > 0.9f)
                {
                    //Out of attack
                    entity.StartCoroutine(_EndAttacks());
                    foreach (var v in attackTrails)
                        v.emitting = false;
                }
                else if (!hasDamaged && state.normalizedTime >= attack.damageTime)
                {
                    hasDamaged = true;

                    ApplyDamageFor(attack);
                }
            }
        }

        public IEnumerator _EndAttacks()
        {
            canAttack = false;
            yield return new WaitForSeconds(0.1f);
            
            _currentCombo.OnEnd(entity, _currentAttack);
            canAttack = true;
            entity.GetModule<PlayerControllerEntityModule>().state = PlayerState.Default;
            _currentAttack = null;
        }

        public void ProcessAttack()
        {
            foreach (var combo in possibleCombos)
            {
                var canDo = true;
                for (var i = 0; i < math.min(combo.attacks.Length, inputOrder.Count); i++)
                {
                    if (combo.attacks[i].attackType == inputOrder[i]) continue;
                    canDo = false;
                    break;
                }
                
                if(!canDo) continue; //Wrong inputs
                if(inputOrder.Count > combo.attacks.Length) return; //Combo ended
                if (inputOrder.Count <= 1)
                {
                    combo.OnStart(entity);
      
                    var inputMove2 = IngameGameInput.InputMove;
                    var inputMove = new Vector3(inputMove2.x, 0, inputMove2.y).normalized;
                    if(_controllerEntity.useCameraView)
                        inputMove = Quaternion.Euler(0, _controllerEntity.camera.transform.eulerAngles.y, 0) * inputMove;
                    
                    if(inputMove.magnitude > 0.15f) 
                        _controllerEntity.body.eulerAngles = new Vector3(0, Vector3.SignedAngle(Vector3.forward, inputMove, Vector3.up), 0);
                    angle = _controllerEntity.body.eulerAngles.y;
                }
                else
                {
                    var state = animator.GetCurrentAnimatorStateInfo(0);
                    var stateTime = state.normalizedTime;
                    
                    var lastAttack = combo.attacks[inputOrder.Count - 2];
                    if (stateTime < lastAttack.nextAttackWindowStart || stateTime > lastAttack.nextAttackWindowEnd) return;
                }

                OnPerformAttack(combo, combo.attacks[inputOrder.Count - 1]);
                return;
            }
            
            OnFailCombo();
        }

        public void OnFailCombo()
        {
            onPlayerFailCombo.Invoke();
            if (inputOrder.Count <= 1) return;
            canAttack = false;
        }
        
        public void ApplyDamageFor(AttackCombo.Attack attack)
        {
            var p = entity.transform.position;
            var pos = p + Vector3.up;/* + _controllerEntity.body.rotation * attackOffset;*/
            var fw = _controllerEntity.body.forward;
            
            foreach (var target in HealthHelper.GetTargets(pos, attackRadius))
            {
                if(!entity.element.CanDamage(target.GetElement())) continue;
                //if(target.GetGameObject() == entity.gameObject) continue;

                var delta = (target.GetGameObject().transform.position - p);
                var dir = delta.normalized;
                var dot = Vector3.Dot(fw, dir);
                if (dot < snapMinDot) continue;
                
                var hPos = (target.GetGameObject().transform.position + pos) / 2f;
                
                target.Damage(attack.damageCount);
                
                if (target.health > 0 && target is HealthEntityModule module)
                {
                    var e = module.entity;
                    e.velocity = dir * 4f;
                }
                
                onPlayerDamageVictim.Invoke(target, attack.damageCount);
                
                var go = Object.Instantiate(hitParticlesPrefab, hPos, Quaternion.identity);
                Object.Destroy(go, 2f);
            }
        }
        
        public void OnPerformAttack(AttackCombo combo, AttackCombo.Attack attack)
        {
           
            var index = Array.IndexOf(combo.attacks, attack);
            if (index > 0)
                ApplyDamageFor(combo.attacks[index - 1]);
            
            onPlayerPerformAttack.Invoke(combo, index - 1);

            animator.CrossFade(attack.clipName, attack.transitionTime);
            entity.GetModule<PlayerControllerEntityModule>().state = PlayerState.Attacking;
            _controllerEntity.body.eulerAngles = new Vector3(0, angle, 0);
            
            DoSnap();

            _currentCombo = combo;
            _currentAttack = attack;
            hasDamaged = false;
            combo.OnDoAttack(entity, attack);

            foreach (var v in attackTrails)
            {
                v.emitting = true;
                v.material.SetColor("_EmissionColor", entity.element.GetColor() * 10);
            }
        }

        public bool CheckRules(AttackCombo combo)
        {
            if (combo.needToBeGrounded)
                return entity.isGrounded;

            return true;
        }
    }
}