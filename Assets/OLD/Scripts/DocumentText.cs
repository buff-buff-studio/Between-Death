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
    
    [Serializable]
    public struct DocumentTextData
    {
        public TMP_FontAsset documentFont;
        
        [TextArea(3,20)]
        public string documentText;
    }
}
