using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Text Document", menuName = "RPG/World/Text Document", order = 1)]
public class DocumentText : DocumentData
{
    public override DocumentType? documentType => DocumentType.Text;
    
    [SerializeField] internal DocumentTextData _documentText;

    public override object GetDocument => _documentText;
    public string text => _documentText.documentText;
    public TMP_FontAsset font => _documentText.documentFont;
    
    [Serializable]
    public struct DocumentTextData
    {
        public TMP_FontAsset documentFont;
        [Range(4,6)]
        public float documentFontSize;
        
        [TextArea(3,20)]
        public string documentText;
    }
}
