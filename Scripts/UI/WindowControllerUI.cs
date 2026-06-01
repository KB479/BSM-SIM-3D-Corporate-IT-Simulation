using UnityEngine;
using UnityEngine.UI;

public class WindowControllerUI : MonoBehaviour
{
    [Header("Pencere Referanslarý")]
    [SerializeField] private RectTransform windowRoot;

    [Header("Butonlar")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button maximizeButton;

    public bool IsFullscreen { get; private set; } = false;

    private Vector2 originalSizeDelta;
    private Vector2 originalAnchoredPosition;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;

    private void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(CloseWindow);
        if (maximizeButton != null) maximizeButton.onClick.AddListener(ToggleFullscreen);
    }

    private void Start()
    {
        SaveOriginalState();
    }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
        windowRoot.SetAsLastSibling();
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    public void ToggleFullscreen()
    {
        if (!IsFullscreen)
        {
            SaveOriginalState();
            windowRoot.anchorMin = new Vector2(0, 0);
            windowRoot.anchorMax = new Vector2(1, 1);
            windowRoot.offsetMin = Vector2.zero;
            windowRoot.offsetMax = Vector2.zero;
            IsFullscreen = true;
        }
        else
        {
            RestoreOriginalState();
        }
    }

    public void RestoreForDrag()
    {
        if (!IsFullscreen) return;

        windowRoot.anchorMin = originalAnchorMin;
        windowRoot.anchorMax = originalAnchorMax;
        windowRoot.sizeDelta = originalSizeDelta;

        IsFullscreen = false;
    }

    private void RestoreOriginalState()
    {
        windowRoot.anchorMin = originalAnchorMin;
        windowRoot.anchorMax = originalAnchorMax;
        windowRoot.sizeDelta = originalSizeDelta;
        windowRoot.anchoredPosition = originalAnchoredPosition;
        IsFullscreen = false;
    }

    private void SaveOriginalState()
    {
        originalSizeDelta = windowRoot.sizeDelta;
        originalAnchoredPosition = windowRoot.anchoredPosition;
        originalAnchorMin = windowRoot.anchorMin;
        originalAnchorMax = windowRoot.anchorMax;
    }
}