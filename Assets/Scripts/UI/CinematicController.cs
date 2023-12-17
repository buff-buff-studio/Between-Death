using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Data.Variables;
using Refactor.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class CinematicController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    public CanvasGroup canvasGroup;
    public OrbitCamera camera;
    public IngameGameInput input;

    public AudioSource audioSource;
    public float timeToPlayAudio = 60f;

    public BoolVariable haveCinematic;

    private void Awake()
    {
        audioSource ??= GetComponent<AudioSource>();
        videoPlayer ??= GetComponent<VideoPlayer>();

        if (haveCinematic.Value)
        {
            camera.enabled = false;
            canvasGroup.alpha = 1;
        }
        else
        {
            rawImage.enabled = false;
            camera.enabled = false;
            canvasGroup.alpha = 1;
        }
    }

    private void Start()
    {
        if (haveCinematic.Value)
        {
            StartCoroutine(PlayLastMusic());
            input.DisableAllInput();
            input.inputConfirm.started += OnInputConfirmOnperformed;
            input.inputInteract.started += OnInputCancelOnperformed;
            videoPlayer.Play();
            videoPlayer.loopPointReached += source => { StartCoroutine(FadeOut()); };
        }
        else
        {
            canvasGroup.alpha = 0;
            input.DisableAllInput();
            StartCoroutine(FadeOut());
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

    public IEnumerator PlayLastMusic()
    {
        while (videoPlayer.time < timeToPlayAudio) { yield return null; }

        audioSource.Play();
        if (videoPlayer.time > timeToPlayAudio)
        {
            audioSource.time = (float)(videoPlayer.time - timeToPlayAudio);
        }
    }

    public IEnumerator FadeOut()
    {
        input.inputJump.started -= OnInputConfirmOnperformed;
        input.inputInteract.started -= OnInputCancelOnperformed;

        camera.LookAtCinematic();
        camera.hudCanvas.alpha = 0;

        if(haveCinematic.Value)
        {
            var time = 0f;
            var timeEnd = 2f;
            while (time < timeEnd)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = 1 - (time / timeEnd);
                yield return null;
            }
        }else canvasGroup.alpha = 0;

        haveCinematic.Value = false;
        camera.haveCinematic = true;
        gameObject.SetActive(false);
        camera.enabled = true;
    }
}
