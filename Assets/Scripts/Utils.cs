using System.Runtime.CompilerServices;
using UnityEngine;

namespace Refactor
{
    public static class Utils
    {
        public static T GetOrAddComponent<T>(this GameObject o, out bool wasAdded) where T : Component
        {
            var c = o.GetComponent<T>();
            wasAdded = c == null;
            return c == null ? o.AddComponent<T>() : c;
        }
        
        public static T GetOrAddComponent<T>(this Component o, out bool wasAdded) where T : Component
        {
            var c = o.GetComponent<T>();
            wasAdded = c == null;
            return c == null ? o.gameObject.AddComponent<T>() : c;
        }
        
        
        public static T GetOrAddComponent<T>(this GameObject o) where T : Component
        {
            var c = o.GetComponent<T>();
            return c == null ? o.AddComponent<T>() : c;
        }
        
        public static T GetOrAddComponent<T>(this Component o) where T : Component
        {
            var c = o.GetComponent<T>();
            return c == null ? o.gameObject.AddComponent<T>() : c;
        }

        public static RectTransform GetRectTransform(this Component o)
        {
            return o.transform as RectTransform;
        }
        
        public static RectTransform GetRectTransform(this GameObject o)
        {
            return o.transform as RectTransform;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OnInterval(float f, float min, float max)
        {
            return f > min && f < max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OutInterval(float f, float min, float max)
        {
            return f < min || f > max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetVectorXZ(Vector3 v)
        {
            v.y = 0;
            return v;
        }
    }
}