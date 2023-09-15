using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Refactor;
using Refactor.Interface;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class InspectDoc : MonoBehaviour
{
    [Header("Data")]
    public DocumentText _documentData;
    
    [Space]
    [Header("Objects")]
    [SerializeField] private IngameCanvas _ingameCanvas;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private GameObject _transcriptButton;
    [SerializeField] private GameObject _transcriptPanel;
    [SerializeField] private VolumeProfile _volumeProfile;
    [SerializeField] [Range(0,10)] private float _dragSensitive = 1f;

    [Space] [Header("Prefabs")] [SerializeField]
    private GameObject _documentPrefab;
    
    private Vector2 _mousePos;
    private bool _mouseDrag;
    private PlayerInput _playerInput;
    private bool _resetRotation = false;

    private void Awake()
    {
        _ingameCanvas ??= FindObjectOfType<IngameCanvas>();
    }

    private void Update()
    {
        if(IngameGameInput.InputAttack1.trigger) SetTranscriptActive(!_transcriptPanel.activeSelf);
        else if(IngameGameInput.InputRunning.trigger) StartCoroutine(ResetRotation());
        else if(IngameGameInput.InputInteract.trigger) DocExit();
    }

    private void OnEnable()
    {
        if(_nameText != null) _nameText.text = _documentData.documentName;
        if(_descriptionText != null) _descriptionText.text = _documentData._documentTranscript;
        
        _documentPrefab.GetComponentInChildren<TextMeshPro>().text = _documentData._documentText.documentText;
        _documentPrefab.GetComponentInChildren<TextMeshPro>().font = _documentData._documentText.documentFont;
        
        SetTranscriptActive(false);
    }

    public void DocExit()
    {
        Debug.Log("DocExit");
        if (_transcriptPanel.activeSelf) SetTranscriptActive(false);
        else _ingameCanvas.CloseDocumentWindow();
    }

    public void SetTranscriptActive(bool active)
    {
        _volumeProfile.TryGet(out DepthOfField dof);
        dof.active = active;
        _transcriptPanel.SetActive(active);
        _transcriptButton.SetActive(!active);
    }

    public void DocumentMove(Vector2 _mousePos)
    {
        this._mousePos = _mousePos;
        if (_playerInput.currentControlScheme != "KeyboardAndMouse") OnMouseDrag();
        else if(_mouseDrag) OnMouseDrag();
    }

    public void OnMouseDrag()
    {
        var x = -_mousePos.x * _dragSensitive;
        var y = _mousePos.y * _dragSensitive;
        this.transform.Rotate(new Vector3(y, x, 0));
        
        _resetRotation = false;
    }

    private IEnumerator ResetRotation()
    {
        _resetRotation = true;
        while (transform.GetChild(0).rotation != Quaternion.identity && _resetRotation)
        {
            transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, Quaternion.identity, Time.deltaTime);
            yield return null;
        }
    }
}
