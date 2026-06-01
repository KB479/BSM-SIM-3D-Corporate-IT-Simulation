using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "TaskScriptableObject", menuName = "ScriptableObjects/TaskScriptableObject")]
public class TaskSO : ScriptableObject
{
    // Görev tipine özgün alanlar TaskSO kalýtýmlarý yapýlmalý, refactor gerek

    [Header("Görev Kimliði (Mevcut)")]
    public string taskID;
    public string taskName;

    [TextArea(3, 5)]
    public string description;

    [Header("Görev Özellikleri (Mevcut)")]
    public TaskType type;
    public TaskDifficulty difficulty;

    [TextArea(3, 5)]
    [Tooltip("Bu görevin gerçek hayattaki BSM ders/konu karþýlýðý.")]
    public string curriculumReference;

    [Header("Sonuç Etkileri (Mevcut)")]
    public int successCreditReward = 10;
    public int failCreditPenalty = 15;

    [Header("MailApp Bilgileri")]
    public string emailSender;
    public string emailSubject;
    [TextArea(3, 5)]
    public string emailContent;


    //  SOFTWARE GÖREVLERÝ ÝÇÝN VERÝLER

    [Header("IDE Setup (Kod Görevleri Ýçin)")]
    public string fileName; 

    [TextArea(10, 20)]
    [Tooltip("Ýçinde {CONFLICT_1} vb. yer tutucular barýndýran þablon ana kod.")]
    public string baseCodeTemplate;

    [Header("Çakýþma Verileri")]
    public List<ConflictZone> conflictZones;

    [Header("Commit UI Kartlarý")]
    public CommitCardData commitA_Card;
    public CommitCardData commitB_Card;
    public int correctCommitIndex;

    // ORGANIZATION (DÝYALOG) GÖREVLERÝ ÝÇÝN VERÝLER

    [Header("Organization Setup (Diyalog Görevleri Ýçin)")]
    public string npcName;

    [TextArea(3, 5)]
    public string initialCrisisDialogue;

    [Header("1. Araþtýrma Sorularý (Ýsteðe Baðlý)")]
    public List<InvestigationOption> investigationOptions;

    [Header("2. Nihai Karar Seçenekleri (Görevi Sonlandýrýr)")]
    public List<DialogueOption> dialogueOptions;


}