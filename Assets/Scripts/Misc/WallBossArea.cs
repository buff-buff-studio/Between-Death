using System;
using Refactor.Entities;
using Refactor.Misc;
using UnityEngine;

namespace Misc
{
    public class WallBossArea : MonoBehaviour
    {
        [SerializeField]private Transform plane;
        [SerializeField] private OrbitCamera cam;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player");
                other.transform.GetComponent<Entity>().enabled = false;
                other.transform.GetComponent<CharacterController>().enabled = false;
                cam.enabled = false;
                
                var newPos = other.transform.position;
                newPos.x *= -0.9f;
                newPos.z *= -0.9f;
                other.transform.position = newPos;
                  
                newPos = cam.transform.position;
                newPos.x *= -0.9f;
                newPos.z *= -0.9f;
                cam.transform.position = newPos;
                 other.transform.GetComponent<Entity>().enabled = true;
                 other.transform.GetComponent<CharacterController>().enabled = true;
                 cam.enabled = true;
            }
        }
    }
}
