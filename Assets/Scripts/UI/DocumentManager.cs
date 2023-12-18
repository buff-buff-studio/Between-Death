using System;
using System.Collections;
using System.Collections.Generic;
using Player.Documents.Data;
using Refactor;
using Refactor.Data.Variables;
using Refactor.Interface;
using Refactor.Interface.Windows;
using TMPro;
using UnityEngine;

public class DocumentManager : MonoBehaviour
{
    public static DocumentManager instance;
    
    [Header("INVENTORY")]
    [SerializeField] private Variable<InventoryData> inventory;

    private InventoryData inventoryData => inventory.Value;
    public DocumentList documents => inventoryData.GetDocumentList;
    
    [SerializeField] private DocumentSlot documentSlotPrefab;
    [SerializeField] private Transform socumentSlotParent;
    [SerializeField] private TextMeshProUGUI documentName;
    [SerializeField] private IngameGameInput canvasGameInput;
    
    private List<int> _documentsSlots = new List<int>();
    public bool InInventory(int id) => _documentsSlots.Contains(id);

    [Space]
    [Header("DOCUMENT VIEW")]
    [SerializeField] private Transform viewParent;
    [SerializeField] private GameObject objectView;
    [SerializeField] private GameObject textView;
    [SerializeField] private GameObject transcriptView;

    [SerializeField] private TextMeshPro textDocument;
    [SerializeField] private TextMeshProUGUI textTranscript;
    [SerializeField] [Range(0, 10)] private float dragSensitive;
    
    private int _currentDocument = -1;
    private bool transcriptActive => transcriptView.activeSelf;
    private WindowManager window => GetComponent<WindowManager>();
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void OnEnable()
    {
        UpdateInventory();
        IngameGameInput.CanInput = false;
    }

    private void Update()
    {
        if (!window._active) return;
        if(canvasGameInput.inputForth.triggered)
            SetTranscript(!transcriptActive);
        else if (canvasGameInput.inputCamera.inProgress && !transcriptActive)
            MoveDocument();
    }

    public void MoveDocument()
    {
        return;
        var xy = canvasGameInput.inputCamera.ReadValue<Vector2>();
        var x = -xy.x * dragSensitive;
        var y = xy.y * dragSensitive;
        viewParent.Rotate(new Vector3(y, x, 0), Space.Self);
    }

    public void UpdateInventory()
    {
        foreach (var docs in inventoryData.GetUnlockedDocuments)
        {
            if(_documentsSlots.Contains(docs)) continue;
            _documentsSlots.Add(docs);
            var slot = Instantiate(documentSlotPrefab, socumentSlotParent);
            slot.UpdateSlot(docs);
        }
        
        if(_currentDocument < 0) UpdateDocumentUI(inventoryData.GetUnlockedDocument(0));
    }
    
    public void UpdateDocumentUI(int id)
    {
        if (id == _currentDocument) return;
        
        if(objectView != null) Destroy(objectView);
        if (id < 0)
        {
            textView.SetActive(false);
            return;
        }
        
        SetTranscript(false, documents.GetTranscript(id));
        viewParent.rotation = Quaternion.identity;
        documentName.text = documents.GetName(id);

        switch (documents.GetDocumentType(id))
        {
            case DocumentData.DocumentType.Text:
                textView.SetActive(true);
                var text = (DocumentText) documents.Get(id);
                SetDocumentText(text.text, text.font);
                break;
            case DocumentData.DocumentType.Image:
                break;
            case DocumentData.DocumentType.Object:
                textView.SetActive(false);
                var obj = (DocumentObject) documents.Get(id);
                objectView = Instantiate(obj._documentPrefab, viewParent);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        _currentDocument = id;
    }

    public void SetTranscript(bool active, string transcript)
    {
        textTranscript.text = transcript;
        transcriptView.SetActive(active);
    }
    
    public void SetTranscript(bool active)
    {
        transcriptView.SetActive(active);
    }

    public void SetDocumentText(string text, TMP_FontAsset font)
    {
        textDocument.text = text;
        textDocument.font = font;
    }
}
