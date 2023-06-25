using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Image Document", menuName = "RPG/World/Image Document", order = 1)]
public class DocumentImage : DocumentData
{
    public override DocumentType? documentType => DocumentType.Image;
    
    [SerializeField] internal Sprite _documentSprite;
    public override object GetDocument => _documentSprite;
}
