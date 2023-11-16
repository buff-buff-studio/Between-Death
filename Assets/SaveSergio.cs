using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class SaveSergio : MonoBehaviour
{
    public Transform[] transforms;
    public Vector3[] eulers;

    public bool saving = true;

    private void OnEnable()
    {
        if (saving)
        {
            eulers = new Vector3[transforms.Length];

            for (int i = 0; i < eulers.Length; i++)
                eulers[i] = transforms[i].position;
        }
        else
        {
            for (int i = 0; i < eulers.Length; i++)
                transforms[i].position = eulers[i];
        }
    }
}