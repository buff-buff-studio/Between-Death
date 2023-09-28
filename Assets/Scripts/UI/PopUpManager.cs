using System;
using System.Collections;
using Refactor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup parent;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image icon;
    [SerializeField] private int autoHideTime = 5;

    public bool isOpen = false;
    
    private Action _onClickEvent;
    private CanvasGroup canvasGroup => GetComponent<CanvasGroup>();
    private Animator animator => GetComponent<Animator>();
    
    public void Show(string title, string description, Sprite icon, Action onClick = null)
    {
        this.title.text = title;
        this.description.text = description;
        this.icon.sprite = icon;
        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;        
        
        parent.alpha = 1;
        parent.interactable = true;
        parent.blocksRaycasts = true;
        
        isOpen = true;
        _onClickEvent = onClick;
        animator.Play("Open");
        StartCoroutine(OpenFade());
        StartCoroutine(AutoHide());
    }
    
    public void Hide(bool canInput = true)
    {
        StopAllCoroutines();
        animator.Play("Close");
        
        isOpen = false;

        StartCoroutine(CloseFade());
        
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;        
        
        parent.interactable = false;
        parent.blocksRaycasts = false;
        
        
        IngameGameInput.CanInput = canInput;
    }
    
    public void OnClick()
    {
        _onClickEvent?.Invoke();
        Hide(false);
    }
    
    public IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(autoHideTime);
        Hide();
    }

    private IEnumerator OpenFade()
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 3;
            canvasGroup.alpha = time;
            parent.alpha = time;
            yield return null;
        }
    }

    private IEnumerator CloseFade()
    {
        float time = 1;
        while (time > 0)
        {
            time -= Time.deltaTime * 3;
            canvasGroup.alpha = time;
            parent.alpha = time;
            yield return null;
        }
    }
}
