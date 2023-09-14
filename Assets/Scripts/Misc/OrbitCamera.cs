using Refactor.Data.Variables;
using Unity.Mathematics;
using UnityEngine;

namespace Refactor.Misc
{
    public class OrbitCamera : MonoBehaviour
    {
        public static float DeltaRot = 0;
        [Header("REFERENCES")]
        public Transform target;
        public LayerMask collisionMask;

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
        public float maxPivotDistance = 0.5f;
        public float turbulence = 10f;
        
        [Header("STATE")]
        public Vector2 rotation;
        private Vector3 _targetItself;

        public void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _targetItself = target.transform.position;
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
                var senX = (invertX ? 1f : -1f) * math.clamp(sensitivityX.value/10f, 0f, 1f); 
                var senY = (invertY ? -1f : 1f) * math.clamp(sensitivityY.value/10f, 0f, 1f);
                var mouseInput = IngameGameInput.InputCamera;

                DeltaRot += math.abs(mouseInput.x * senX * msModifier) + math.abs(mouseInput.y * senY * msModifier);
                rotation.y += mouseInput.x * senX * msModifier;
                rotation.x = math.clamp(rotation.x + mouseInput.y * senY * msModifier, -89f, 89f);
            }
            #endregion

            #region Point Calculation
            Vector3 newTarget = target.position;
            Vector3 delta = newTarget - _targetItself;
            float mag = delta.magnitude;
            _targetItself = newTarget - (mag > maxPivotDistance ? delta.normalized * maxPivotDistance : delta);
            _targetItself = Vector3.Lerp(_targetItself, newTarget, deltaTime * 2f);
            Vector3 point = _targetItself + offset;
            #endregion
            
            #region Rotation
            var breath = Quaternion.Euler(math.sin(math.radians(Time.time * 80f)) * breathWeight, 0,
                math.cos(math.radians(Time.time * -72f)) * breathWeight);

            float turbulenceStrength = mag / maxPivotDistance * turbulence;
            var tbb = Quaternion.Euler(0, 0, math.sin(Time.time * 24f) * turbulenceStrength);
            
            t.rotation = Quaternion.Lerp(t.rotation, Quaternion.Euler(rotation.x, rotation.y, 0) * breath * tbb, rotationLerpSpeed * deltaTime);
            #endregion

            #region Collision
            if (Physics.SphereCast(point, collisionRadius, -t.forward,  out RaycastHit hit, d, collisionMask))
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