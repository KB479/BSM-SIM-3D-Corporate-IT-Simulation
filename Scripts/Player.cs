using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask layerMask;
    [SerializeField] private Transform playerInteractPoint;
    [SerializeField] private float interactDistance; 

    // Player ve player movement iþlev düzenlemesi gerekiyor

    private void Update()
    {
        HandleInteraction();
        EndDay(); 
    }



    public void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(Physics.Raycast(playerInteractPoint.position, transform.forward, out RaycastHit hit, interactDistance, layerMask))
            {

                if(hit.transform.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
                }
            }
        }
    }


    private void EndDay()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.TryEndDay(); 
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = playerInteractPoint.position;
        Gizmos.DrawRay(origin, playerInteractPoint.transform.forward * interactDistance);
    }
    


}