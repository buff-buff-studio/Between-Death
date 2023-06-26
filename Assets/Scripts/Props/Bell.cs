using System;
using UnityEngine;

namespace Refactor.Props
{
    public class Bell : MonoBehaviour
    {
        public Vector3 topPosition = new(0, 10, 40);
        public Vector3 bottomPosition = new Vector3(0, 0, 40);

        public bool state = false;

        public void ClimbTheBell()
        {
            state = true;
        }
        
        public void Update()
        {
            var deltaTime = Time.deltaTime;
            var targetPos = state ? topPosition : bottomPosition;
            transform.position = Vector3.Lerp(transform.position, targetPos, deltaTime * 12f);
        }
    }
}