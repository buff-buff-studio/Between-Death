using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object Document", menuName = "RPG/World/Object Document", order = 1)]
public class DocumentObject : DocumentData
{
    public override DocumentType? documentType => DocumentType.Object;
    
    [SerializeField] internal GameObject _documentPrefab;
    public override object GetDocument => _documentPrefab;
}
