using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class PlurosEntity : GioEntityModule
    {
        [SerializeField]
        private float circleRadius = 4;
        
        
        protected override Vector3 WaitingToAttackNavMesh()
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(GetPointInCircle(), out hit, 1, NavMesh.AllAreas))
            {
                Debug.Log(hit.position);
                return hit.position;
            }

            return WaitingToAttackNavMesh();
        }
        
               
        private Vector3 GetPointInCircle()
        {
            var center = playerRef.position;
            var radius = circleRadius;
            var angle = Random.Range(0f, 100f) * Math.PI * 2;
            var x = center.x + Math.Cos(angle) * radius;
            var z = center.y + Math.Sin(angle) * radius;
            Debug.Log("Circle pos " + new Vector3((float)x, 1, (float)z));
            return new Vector3((float)x, 1, (float)z);
        }
    }
}