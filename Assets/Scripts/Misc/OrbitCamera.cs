using Refactor.Data.Variables;
using Unity.Mathematics;
using UnityEngine;

namespace Refactor.Misc
{
    public class OrbitCamera : MonoBehaviour
    {
        [Header("REFERENCES")]
        public Transform target;

        public Value<float> sensitivityX = 50;
        public Value<float> sensitivityY = 50;
        public Value<bool> invertX = false;
        public Value<bool> invertY = true;

        [Header("SETTINGS")]
        public Vector3 offset;
        public float distance = 5f;
        public float rotationLerpSpeed = 5f;
        public float collisionRadius = 3f;
        public float minDistance = 1f;
        public float breathWeight = 5f;
        
        [Header("STATE")]
        public Vector2 rotation;

        private Vector3 _lastTarget;

        public void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _lastTarget = target.transform.position;
        }

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(transform.position, collisionRadius);
        }

        public void Update()
        {
            var deltaTime = Time.deltaTime;
            var t = transform;
            var d = distance;
            var msModifier = 1f;
            if (GameInput.CurrentControlScheme is GameInput.ControlScheme.Desktop)
                msModifier *= 0.25f;

            #region Input
            
            if (Cursor.lockState == CursorLockMode.Locked || GameInput.CurrentControlScheme != GameInput.ControlScheme.Desktop)
            {
                var senX = (invertX ? -1f : 1f) * math.clamp(sensitivityX.value/10f, 0f, 1f); 
                var senY = (invertY ? -1f : 1f) * math.clamp(sensitivityY.value/10f, 0f, 1f);
                var mouseInput = IngameGameInput.InputCamera;
                rotation.y += mouseInput.x * senX * msModifier;
                rotation.x = math.clamp(rotation.x + mouseInput.y * senY * msModifier, -89f, 89f);
            }
            #endregion

            #region Rotation

            var breath = Quaternion.Euler(math.sin(math.radians(Time.time * 80f)) * breathWeight, 0,
                math.cos(math.radians(Time.time * -72f)) * breathWeight);
            
            t.rotation = Quaternion.Lerp(t.rotation, Quaternion.Euler(rotation.x, rotation.y, 0) * breath, rotationLerpSpeed * deltaTime);
            var targetPosition = target.position;
            var point = new Vector3(targetPosition.x,
                _lastTarget.y = math.lerp(_lastTarget.y, targetPosition.y, deltaTime * 2f)
                , targetPosition.z) + offset; /*+ target.TransformVector(offset);*/
            _lastTarget = targetPosition;
            #endregion

            #region Collision
            if (Physics.SphereCast(point, collisionRadius, -t.forward, out RaycastHit hit, d))
            {
                d = math.max(minDistance, hit.distance);
            }
            #endregion

            #region Position
            t.position = point - d * t.forward;
            #endregion
        }
    }
}