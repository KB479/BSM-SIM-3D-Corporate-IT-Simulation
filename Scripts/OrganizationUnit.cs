using UnityEngine;

public class OrganizationUnit : InteractionUnit
{
    protected override void ExecuteInteraction()
    {
        DialogueORG_UI.Instance.StartDialogue(this);
    }
}