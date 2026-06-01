using UnityEngine;

// Disco Elysium benzeri akýþ, ilerde dialogue tree altyapýsý kurulacak

// 1. Oyuncunun sorunu anlamak için soracaðý sorular (Görevi bitirmez)
[System.Serializable]
public struct InvestigationOption
{
    [Tooltip("Butonda yazacak kýsa soru (Örn: Neden sunucu çöktü?)")]
    public string buttonPreviewText;

    [TextArea(2, 3)]
    [Tooltip("Log'a düþecek uzun soru cümlesi")]
    public string playerFullQuestion;

    [TextArea(2, 4)]
    [Tooltip("Soruya karþýlýk NPC'nin vereceði cevap")]
    public string npcResponse;
}

// 2. Oyuncunun vereceði nihai kararlar (Görevi bitirir)
[System.Serializable]
public struct DialogueOption
{
    [Tooltip("Butonda yazacak kýsa karar (Örn: Projeyi iptal et)")]
    public string buttonPreviewText;

    [TextArea(2, 4)]
    [Tooltip("Karar verilince log'a düþecek oyuncu cümlesi")]
    public string fullLogText;

    // NPC'nin cevabý
    [TextArea(2, 4)]
    [Tooltip("Oyuncu bu kararý verince NPC'nin vereceði son tepki")]
    public string npcReaction;

    [Tooltip("Bu karar doðru mu?")]
    public bool isCorrectChoice;
}