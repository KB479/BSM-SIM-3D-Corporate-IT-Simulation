using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting.InputSystem;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance {  get; private set; }

    private PlayerInputActions playerInputActions;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();


        if (Instance != null)
        {
            Debug.LogError("There is more than one GameInput Instance!"); 
        }

        Instance = this;
    }


    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>(); 
        inputVector = inputVector.normalized;

        return inputVector;
    }

    public Vector2 GetLookVectorDelta()
    {
        Vector2 inputVector = playerInputActions.Player.Look.ReadValue<Vector2>();

        return inputVector;
    }


    public bool JumpPressed()
    {
        return playerInputActions.Player.Jump.triggered;
    }

}
