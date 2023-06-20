using Refactor.Entities.Modules;
using UnityEngine;

namespace Refactor.Entities
{
    public class EntityIKCaller : MonoBehaviour
    {
        [Header("REFERENCES")]
        public Entity entity;
        public Animator animator;
        private PlayerControllerEntityModule _module;

        private void OnEnable()
        {
            _module = entity.GetModule<PlayerControllerEntityModule>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            _module?.OnAnimatorIK(layerIndex);
        }
        
        private void OnAnimatorMove()
        {
            var y = transform.position.y;
            animator.ApplyBuiltinRootMotion();
            var lc = animator.transform.localPosition;
            lc.y = 0;
            entity.controller.Move(lc);
            animator.transform.localPosition = Vector3.zero;
        }
    }
}