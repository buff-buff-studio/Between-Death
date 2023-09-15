using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Props;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Object = System.Object;

public class DocumentData : ChestItem
{
    public enum DocumentType
    {
        Text,
        Image,
        Object
    }
    
    public virtual DocumentType? documentType => null;
    
    public string documentName = "New Document";
    public string documentDescription = "Document Description";
    
    [SerializeField] [TextArea(3,20)] internal string _documentTranscript;
    
    public virtual object GetDocument => null;
    
    public override void OpenItem()
    {
        InGameHUD.instance.OpenDocument(this);
    }
}
