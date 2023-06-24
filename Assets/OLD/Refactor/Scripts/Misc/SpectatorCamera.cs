using Unity.Mathematics;
using UnityEngine;

namespace Refactor.Misc
{
    public class SpectatorCamera : MonoBehaviour
    {
        [Header("SETTINGS")]
        public float moveSpeed = 5F;
        public float mouseSensibility = 0.1f;
        
        [Header("STATE")]
        public Vector2 rotation;
        public Vector2 inputMouse;
        public Vector2 inputMoveXZ;
        public bool inputMoveUp;
        public bool inputMoveDown;
        
        private Rigidbody _rigidbody;
        public void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        public void Update()
        {
            var deltaTime = Time.deltaTime;
            rotation.y += inputMouse.x * mouseSensibility;
            rotation.x = math.clamp(rotation.x - inputMouse.y * mouseSensibility, -89f, 89f);

            transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0);

            var moveY = (inputMoveUp ? 1f : 0f) + (inputMoveDown ? -1f : 0f);

            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Quaternion.Euler(0, rotation.y, 0) * new Vector3(inputMoveXZ.x, moveY, inputMoveXZ.y) * moveSpeed, deltaTime * 5f);
        }
    }
}