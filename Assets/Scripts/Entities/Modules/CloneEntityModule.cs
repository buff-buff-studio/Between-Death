using System;
using Refactor.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class CloneEntityModule : EntityModule
    {
        [Header("REFERENCES")]
        public Transform transformReference;
        public GameObject prefab;
        public Material cloneMaterial;
        
        [Header("SETTINGS")]
        public int meshCount = 2;
        
        public void Clone(float duration)
        {
            var go = Object.Instantiate(prefab, transformReference.position, transformReference.rotation);

            var color = entity.element.GetColor();
            color.a = 0.25f;
            cloneMaterial.color = color;
            
            _Apply(go.transform, transformReference);
            _ApplyMaterial(go.transform, meshCount);
            
            Object.Destroy(go, duration);
        }

        private void _Apply(Transform clone, Transform original)
        {
            for (var i = 0; i < clone.childCount; i++)
            {
                var c = clone.GetChild(i);
                var o = original.GetChild(i);
                c.rotation = o.rotation;
                _Apply(c, o);
            }
        }

        private void _ApplyMaterial(Transform clone, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var renderer = clone.GetChild(i).GetComponent<Renderer>();
                renderer.material = cloneMaterial;

                //for (var m = 0; m < renderer.materials.Length; m++)
                //renderer.materials[m] = cloneMaterial;
            }
        }
    }
}