using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Object = System.Object;

public class DocumentData : ScriptableObject
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
    
    [SerializeField] [Multiline] internal string _documentTranscript;
    
    public virtual object GetDocument => null;
}
