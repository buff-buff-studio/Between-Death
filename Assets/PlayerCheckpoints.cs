using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Data.Variables;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCheckpoints : MonoBehaviour
{
    public static PlayerCheckpoints instance;

    public Vector3 checkpoint => checkpoints[index.Value].position;

    public List<Transform> checkpoints;
    public IntVariable index;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        checkpoints.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            checkpoints.Add(transform.GetChild(i));
            checkpoints[i].AddComponent<CheckpointTrigger>();
        }
    }

    public void SetCheckpoint(Transform position)
    {
        index.Value = checkpoints.IndexOf(position);
    }

    public void ForceCheckpoint(Vector3 position)
    {
        checkpoints[index.Value].position = position;
    }
}
