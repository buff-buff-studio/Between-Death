using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DocumentData;

[CreateAssetMenu(fileName = "DocumentList", menuName = "RPG/DocumentList")]
public class DocumentList : ScriptableObject
{
    public List<DocumentData> documents = new List<DocumentData>();
    
    public DocumentData Get(int i) => documents[i];
    
    public int GetID(DocumentData item) => documents.IndexOf(item);
    public string GetName(int i) => documents[i].documentName;
    public Sprite GetIcon(int i) => documents[i].documentIcon;
    public string GetTranscript(int i) => documents[i]._documentTranscript;
    public DocumentType GetDocumentType(int i) => (DocumentType)documents[i].documentType;
    public object GetDocument(int i) => documents[i].GetDocument;
}
