using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Entities.Modules;
using Refactor.Misc;
using UnityEngine;

public class ElevatorInteract : MonoBehaviour
{
    [SerializeField]
    private Transform topPosition, bottomPosition;
    [SerializeField] [Tooltip("false = bottom, true = top")]
    private bool floor = false;

    [SerializeField] [Range(.1f, 100f)]
    private float timeToMove = 1f;
    [SerializeField] [Range(.1f,3)]
    private float timeToWait = 1f;

    private float distance => Vector3.Distance(topPosition.position, bottomPosition.position);
    private GravityEntityModule _gravityEntityModule;
    private OrbitCamera _camera;

    public bool Floor
    {
        set
        {
            _gravityEntityModule ??= GameController.instance.player.GetModule<GravityEntityModule>();
            _camera ??= GameController.instance.camera;
            PlayerMode(false);

            StopAllCoroutines();
            floor = value;
            StartCoroutine(MoveElevator());
        }
    }

    public IEnumerator MoveElevator()
    {
        var start = transform.position;
        var end = floor ? topPosition.position : bottomPosition.position;
        var time = 0f;
        var timeEnd = (Vector3.Distance(start, end) * timeToMove) / distance;

        yield return new WaitForSeconds(timeToWait);

        PlayerMode(true);
        while (time < timeEnd)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, time/timeEnd);
            yield return null;
        }
        PlayerMode(false);
    }

    private void PlayerMode(bool enter)
    {
        _camera.maxPivotDistance = enter ? .1f : 1f;
        _gravityEntityModule.gravityForce = enter ? 100 : 20;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(!Application.isPlaying)
            transform.position = floor ? topPosition.position : bottomPosition.position;
    }
#endif
}
