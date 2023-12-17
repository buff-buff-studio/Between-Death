using System;
using UnityEngine;

namespace Refactor.Entities.Modules
{
    public class SetLayer : MonoBehaviour
    {
        [SerializeField]
        private Animator anim;
        [SerializeField]
        private int layer = 0;
        private void Start()
        {
            anim.SetLayerWeight(layer, 1);
        }
    }
}