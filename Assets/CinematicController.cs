using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class CinematicController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public CanvasGroup canvasGroup;
    public OrbitCamera camera;
    public IngameGameInput input;

    public bool haveCinematic = true;

    private void Start()
    {
        if (haveCinematic)
        {
            camera.enabled = false;
            canvasGroup.alpha = 1;
            input.DisableAllInput();
            input.inputConfirm.started += OnInputConfirmOnperformed;
            input.inputInteract.started += OnInputCancelOnperformed;
            videoPlayer.loopPointReached += source => { StartCoroutine(FadeOut()); };
        }
        else
        {
            gameObject.SetActive(false);

            input.EnableAllInput();
            camera.haveCinematic = false;
            camera.enabled = true;
        }
    }

    private void OnInputConfirmOnperformed(InputAction.CallbackContext context)
    {
        StartCoroutine(FadeOut());
    }
    private void OnInputCancelOnperformed(InputAction.CallbackContext context)
    {
        videoPlayer.time += 10;
    }

    public IEnumerator FadeOut()
    {
        input.inputJump.started -= OnInputConfirmOnperformed;
        input.inputInteract.started -= OnInputCancelOnperformed;

        haveCinematic = false;
        camera.LookAtCinematic();
        camera.hudCanvas.alpha = 0;

        var time = 0f;
        var timeEnd = 2f;
        while (time < timeEnd)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = 1-(time / timeEnd);
            yield return null;
        }

        camera.haveCinematic = true;
        gameObject.SetActive(false);
        camera.enabled = true;
    }
}
