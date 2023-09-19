using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DocumentManager : MonoBehaviour
{
    public static DocumentManager instance;
    
    [Header("INVENTORY")]
    [SerializeField] private InventoryData inventoryData;
    public DocumentList documents => inventoryData.GetDocumentList;
    
    [SerializeField] private DocumentSlot documentSlotPrefab;
    [SerializeField] private Transform socumentSlotParent;
    [SerializeField] private TextMeshProUGUI documentName;
    
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
    
    private int _currentDocument = -1;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        inventoryData ??= Resources.Load<InventoryData>("Inventory");
    }

    public void OnEnable()
    {
        UpdateInventory();
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
        if(objectView != null) Destroy(objectView);
        if (id < 0)
        {
            textView.SetActive(false);
            return;
        }
        
        SetTranscript(false, documents.GetTranscript(id));
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
