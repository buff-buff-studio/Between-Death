using System.Runtime.CompilerServices;
using UnityEngine;

namespace Refactor
{
    public static class Utils
    {
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