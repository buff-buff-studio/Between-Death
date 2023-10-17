using System;
using UnityEngine;

namespace Refactor.Entities.Modules
{
    public class RayCastDebug : MonoBehaviour
    {
        private void OnDrawGizmos()
        {

            Debug.DrawLine(transform.position,new Vector3(transform.position.x,transform.position.y, transform.position.z + 0.8f));
            Gizmos.DrawSphere(new Vector3(transform.position.x,transform.position.y, transform.position.z + 0.8f),2);
        }
    }
}