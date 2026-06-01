using UnityEngine;

public class SoftwareUnit : InteractionUnit
{

    protected override void ExecuteInteraction()
    {
        VirtualPC_UI.Instance.TurnOnOS(this);
    }




}
