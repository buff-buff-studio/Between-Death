using System;
using System.Collections;
using DG.Tweening;
using Refactor.Data;
using Refactor.Data.Variables;
using Refactor.Entities.Modules;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Entities
{
    public class Entity : ModularBehaviour<EntityModule>
    {
        #region Fields
        [Header("REFERENCES")]
        public CharacterController controller;
        
        [Header("STATE")]
        public bool isGrounded;
        public Vector3 velocity;
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        private Value<Element> _element = Element.Order;

        public Element element
        {
            get => _element.value;
            set
            {
                if (value == _element.value) return;
                
                _element.value = value;
                onChangeElement.Invoke();
            }
        }

        public UnityEvent onChangeElement;
        #endregion
        
        #region Unity Callbacks
        public override void OnEnable()
        {
            controller = GetComponent<CharacterController>();
            base.OnEnable();
            
            StartCoroutine(_After());
        }

        private IEnumerator _After()
        {
            yield return new WaitForSeconds(0.05f);
            onChangeElement.Invoke();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            foreach (var module in GetModules())
            {
                if (!module.enabled) continue;
                (module as EntityModule)!.UpdatePhysics(deltaTime);
                (module as EntityModule)!.UpdateFrame(deltaTime);
            }

            var flags = controller.Move(velocity * deltaTime);
            if ((flags & CollisionFlags.Above) != 0)
                velocity.y = math.min(velocity.y, 0);
            
            isGrounded = controller.isGrounded;
        }

        public void Die()
        {
            var entityModule = GetModule<EnemyControllerEntityModule>();
            if (entityModule != null)
            {
                entityModule.animator.CrossFade("Die", 0.2f);
                velocity.x = velocity.z = 0;
                RemoveModule(entityModule);
            }
            
            var playerModule = GetModule<PlayerControllerEntityModule>();
            if (playerModule != null)
            {
                playerModule.animator.CrossFade("Die", 0.2f);
                velocity.x = velocity.z = 0;
                RemoveModule(playerModule);
            }
            
            Destroy(gameObject, 3f);
            //transform.DOScale(new Vector3(1, 0, 1), 3f).SetDelay(2f);
        }
        #endregion
        
        #region Module Methods
        protected override void OnAddModule(EntityModule module)
        {
            base.OnAddModule(module);
            module.entity = this;
        }
        #endregion
    }

    [Serializable]
    public abstract class EntityModule : Module
    {
        [HideInInspector]
        public Entity entity;

        public virtual void UpdatePhysics(float deltaTime)
        {
        }

        public virtual void UpdateFrame(float deltaTime)
        {
        }
    }
}