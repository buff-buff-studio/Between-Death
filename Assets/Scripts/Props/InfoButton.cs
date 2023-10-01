using UnityEngine;

namespace Refactor.Props
{
    public class InfoButton : Interactable
    {
        public Transform text;
        private Transform _camera;
        public Transform[] meshes;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _camera = Camera.main!.transform;
        }

        public void Update()
        {
            var t = text.transform;
            var fw = _camera.transform.position - t.position;
            t.forward = -fw;

            foreach (var mesh in meshes)
            {
                mesh.right = -fw;
                mesh.eulerAngles = new Vector3(0, mesh.eulerAngles.y, 0);
            }
        }
    }
}