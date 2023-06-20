using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Refactor.Misc
{
    public class LerpRig : Rig
    {
        [Range(0f, 1f)]
        public float value = 0f;
        public float lerpSpeed = 2f;

        private void OnEnable()
        {
            weight = value;
        }

        private void Update()
        {
            weight = math.lerp(weight, value, lerpSpeed * Time.deltaTime);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(LerpRig))]
    public class LerpRigEditor : Editor
    {
        private static readonly string[] _Exclude = {"m_Weight","m_Script", "m_Effectors"};

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, _Exclude);
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}