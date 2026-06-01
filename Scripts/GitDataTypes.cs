using UnityEngine;


[System.Serializable]
public struct CommitCardData
{
    public string author;      
    public string message;     
    public string description; 
    public string timeAgo;     
}


[System.Serializable]
public struct ConflictZone
{
    public string zoneID;
    [TextArea(3, 8)] public string commitACode;
    [TextArea(3, 8)] public string commitBCode;
}
