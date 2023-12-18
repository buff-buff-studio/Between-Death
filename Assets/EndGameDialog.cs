using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Refactor;
using Refactor.Data;
using Refactor.Interface;
using UnityEngine;

public class EndGameDialog : MonoBehaviour
{
    public IngameGameInput input;
    public CanvasGroup master;
    public CanvasGroup dialog;
    public Save save;

    public void BadEnd()
    {
        LoadingScreen.LoadScene("Menu");
        save.ResetData();
    }

    public void Open()
    {
        StartCoroutine(OpenDialog());
    }
    public IEnumerator OpenDialog()
    {
        yield return new WaitForSeconds(2f);
        master.DOFade(1, 1f);
        yield return new WaitForSeconds(1.1f);
        input.canInput = false;
        dialog.DOFade(1, 1f);
        yield return new WaitForSeconds(3.1f);
        dialog.DOFade(0, 1f);
        yield return new WaitForSeconds(1.3f);
        LoadingScreen.LoadScene("Menu");
    }
}
