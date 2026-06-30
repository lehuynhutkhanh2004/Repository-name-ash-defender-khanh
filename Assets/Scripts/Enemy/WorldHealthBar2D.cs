using UnityEngine;

public class WorldHealthBar2D : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth targetHealth;
    public Transform backgroundBar;
    public Transform fillBar;

    [Header("Bar Size")]
    public float barWidth = 0.55f;
    public float barHeight = 0.06f;
    public float backgroundPadding = 0.02f;

    private void Awake()
    {
        if (targetHealth == null)
        {
            targetHealth = GetComponentInParent<EnemyHealth>();
        }

        UpdateBackgroundSize();
    }

    private void Update()
    {
        UpdateBackgroundSize();
        UpdateFillBar();
    }

    private void UpdateBackgroundSize()
    {
        if (backgroundBar == null)
        {
            return;
        }

        backgroundBar.localScale = new Vector3(
            barWidth + backgroundPadding,
            barHeight + backgroundPadding,
            1f
        );

        backgroundBar.localPosition = Vector3.zero;
    }

    private void UpdateFillBar()
    {
        if (targetHealth == null || fillBar == null)
        {
            return;
        }

        float hpPercent = (float)targetHealth.currentHP / targetHealth.maxHP;
        hpPercent = Mathf.Clamp01(hpPercent);

        float currentWidth = barWidth * hpPercent;

        fillBar.localScale = new Vector3(
            currentWidth,
            barHeight,
            1f
        );

        fillBar.localPosition = new Vector3(
            -barWidth / 2f + currentWidth / 2f,
            0f,
            0f
        );
    }
}