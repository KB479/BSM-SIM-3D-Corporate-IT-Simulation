using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopSelectionUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referanslar")]
    [SerializeField] private RectTransform selectionBox;

    private RectTransform desktopRect;
    private Vector2 startLocalPos;

    private void Awake()
    {
        desktopRect = GetComponent<RectTransform>();

        if (selectionBox != null)
        {
            selectionBox.gameObject.SetActive(false); 
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectionBox == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            desktopRect, eventData.position, eventData.pressEventCamera, out startLocalPos);

        selectionBox.gameObject.SetActive(true);
        UpdateSelectionBox(startLocalPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (selectionBox == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            desktopRect, eventData.position, eventData.pressEventCamera, out Vector2 currentLocalPos))
        {
            UpdateSelectionBox(currentLocalPos);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectionBox != null)
        {
            selectionBox.gameObject.SetActive(false);
        }
    }

    private void UpdateSelectionBox(Vector2 currentLocalPos)
    {
        float width = Mathf.Abs(currentLocalPos.x - startLocalPos.x);
        float height = Mathf.Abs(currentLocalPos.y - startLocalPos.y);

        float minX = Mathf.Min(startLocalPos.x, currentLocalPos.x);
        float minY = Mathf.Min(startLocalPos.y, currentLocalPos.y);

        selectionBox.anchoredPosition = new Vector2(minX, minY);
        selectionBox.sizeDelta = new Vector2(width, height);
    }
}