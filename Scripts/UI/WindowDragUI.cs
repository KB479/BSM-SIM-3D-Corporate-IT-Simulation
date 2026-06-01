using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerClickHandler
{
    [Tooltip("Sürüklenecek ana pencere objesi (OS_WindowFrame)")]
    [SerializeField] private RectTransform windowRoot;

    private WindowControllerUI windowController;
    private Vector2 pointerOffset;
    private RectTransform parentRect;

    private void Awake()
    {
        if (windowRoot != null)
        {
            parentRect = windowRoot.parent as RectTransform;
            windowController = windowRoot.GetComponent<WindowControllerUI>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2 && windowController != null)
        {
            windowController.ToggleFullscreen();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (windowRoot != null)
        {
            windowRoot.SetAsLastSibling();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (windowRoot == null || windowController == null) return;

        if (windowController.IsFullscreen)
        {
            windowController.RestoreForDrag();

            pointerOffset = new Vector2(0, (windowRoot.rect.height / 2f) - 15f);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                windowRoot, eventData.position, eventData.pressEventCamera, out pointerOffset);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (windowRoot == null || parentRect == null) return;

        if (windowController != null && windowController.IsFullscreen) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition))
        {
            Vector2 targetPosition = localPointerPosition - pointerOffset;

            float parentHalfHeight = parentRect.rect.height / 2f;
            float windowHalfHeight = windowRoot.rect.height / 2f;

            float titleBarHeight = 40f;
            float minY = -parentHalfHeight - windowHalfHeight + titleBarHeight;

            targetPosition.y = Mathf.Max(targetPosition.y, minY);

            windowRoot.localPosition = targetPosition;
        }
    }
}