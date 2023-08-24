using System;
using System.Collections;
using Refactor.Entities;
using Refactor.Entities.Modules;
using UnityEngine;

namespace Refactor.Data
{
    [CreateAssetMenu(fileName = "AttackCombo", menuName = "Refactor/AttackCombo", order = 100)]
    public class AttackCombo : ScriptableObject
    {
        public enum AttackType
        {
            Left,
            Right
        }
        
        [Serializable]
        public class Attack
        {
            [Header("SETTINGS")]
            public AttackType attackType;
            public float damageTime = 0.4f;
            public float damageCount = 1f;
            
            [Header("ANIMATION")]
            public string clipName;
            public float transitionTime = 0.1f;
            public float nextAttackWindowStart = 0.2f;
            public float nextAttackWindowEnd = 0.8f;
            
        }
        
        [Header("ATTACKS")]
        public Attack[] attacks;
        
        [Header("SETTINGS")]
        public bool needToBeGrounded;

        public void OnStart(Entity entity)
        {
            //entity.GetModule<GravityEntityModule>().enabled = false;
        }

        public void OnEnd(Entity entity, Attack lastAttack)
        {
            //entity.GetModule<GravityEntityModule>().enabled = true;
            entity.velocity = Vector3.zero;
        }

        public void OnDoAttack(Entity entity, Attack attack)
        {
            //entity.StartCoroutine(_SmallDash(entity, 0.1f, 10f));
        }

        private IEnumerator _SmallDash(Entity entity, float time, float dashSpeed)
        {
            var module = entity.GetModule<PlayerControllerEntityModule>();
            var fw = module.body.forward;
            entity.velocity.x = fw.x * dashSpeed;
            entity.velocity.z = fw.z * dashSpeed;
            
            entity.GetModule<CloneEntityModule>().Clone(0.2f);
            
            yield return new WaitForSeconds(time/2f);
            
            entity.GetModule<CloneEntityModule>().Clone(0.2f);

            yield return new WaitForSeconds(time/2f);
            
            entity.velocity.x = 0;
            entity.velocity.z = 0;
        }
    }
}