using UnityEngine;
using UnityEngine.EventSystems;

public class HeroDragIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public Canvas canvas;
    public GameObject heroPrefab;
    public Projectile projectilePrefab;
    public Transform projectileContainer;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startAnchoredPosition;
    private Camera mainCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag Hero Icon");

        startAnchoredPosition = rectTransform.anchoredPosition;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging Hero Icon");

        if (canvas == null)
        {
            return;
        }

        RectTransform canvasRect = canvas.transform as RectTransform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        rectTransform.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag Hero Icon");

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        HeroBuildSlot slot = FindBuildSlotUnderMouse(eventData);

        if (slot != null)
        {
            slot.TryPlaceHero(heroPrefab, projectilePrefab, projectileContainer);
        }
        else
        {
            Debug.Log("No hero slot here.");
        }

        rectTransform.anchoredPosition = startAnchoredPosition;
    }

    private HeroBuildSlot FindBuildSlotUnderMouse(PointerEventData eventData)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return null;
        }

        Vector3 screenPosition = new Vector3(
            eventData.position.x,
            eventData.position.y,
            -mainCamera.transform.position.z
        );

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        Vector2 worldPoint = new Vector2(worldPosition.x, worldPosition.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint);

        foreach (Collider2D hit in hits)
        {
            HeroBuildSlot slot = hit.GetComponent<HeroBuildSlot>();

            if (slot != null)
            {
                return slot;
            }
        }

        return null;
    }
}