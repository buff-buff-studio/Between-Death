using TMPro;
using UnityEngine;

namespace Refactor.Misc
{
    public class DamageIndicator : MonoBehaviour
    {
        [Header("REFERENCES")]
        public TMP_Text labelValue;
        
        [Header("SETTINGS")]
        public float lifeTime = 2f;
        public float speed = 1f;
        
        [Header("COLORS")]
        public Color colorHeal = Color.green;
        public Color colorDamage = Color.red;

        private Transform _camera;
        private void OnEnable()
        {
            Destroy(gameObject, lifeTime);
            _camera = Camera.main!.transform;
        }

        public void Update()
        {
            var t = transform;
            var fw = _camera.transform.position - t.position;
            t.forward = -fw;
            // ReSharper disable once Unity.InefficientPropertyAccess
            t.position += Vector3.up * (speed * Time.deltaTime);
        }

        public void SetValue(bool isDamage, float amount)
        {
            labelValue.color = isDamage ? colorDamage : colorHeal;
            labelValue.text = $"{(int) amount}";
        }
    }
}