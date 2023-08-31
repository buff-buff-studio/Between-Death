using UnityEngine;

namespace Refactor.Entities
{
    [CreateAssetMenu(fileName = "Attack", menuName = "RPG/Attack", order = 100)]
    public class Attack : ScriptableObject
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
        public float transitionTime = 0.1f;

        /// <summary>
        /// Defines when the damage should be applied
        /// </summary>
        public float damageTime = 0.5f;
        
        /// <summary>
        /// Defines after how many time player can chain the next attack
        /// </summary>
        [Header("AFTER TIMING")]
        public float nextAttackWindow = 0.8f;
        
        /// <summary>
        /// Defines how much time after this attack the player can keep the combo
        /// </summary>
        public float keepStreakTime = 1f;
        
        /// <summary>
        /// Defines when the attacking animation should start when played chained withing last attack
        /// </summary>
        public float chainedStartTime = 0f;
    }
}