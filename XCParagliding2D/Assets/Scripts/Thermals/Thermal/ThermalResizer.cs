using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ThermalResizer : MonoBehaviour
{
    public Vector2 size = new Vector2(1, 1); // Размер в мировых координатах
    public float alpha = 1.0f; // Прозрачность (0 - полностью прозрачный, 1 - полностью непрозрачный)
    
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSpriteSize();
        UpdateAlpha();
    }

    public void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSpriteSize();
        UpdateAlpha();
    }

    private void UpdateSpriteSize()
    {
        if (spriteRenderer.sprite == null) return;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        transform.localScale = new Vector3(size.x / spriteSize.x, size.y / spriteSize.y, 1);
    }

    private void UpdateAlpha()
    {
        if (spriteRenderer == null) return;

        Color color = spriteRenderer.color;
        color.a = Mathf.Clamp01(alpha);
        spriteRenderer.color = color;
    }
}

