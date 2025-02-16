using UnityEngine;

public class ColliderGizmoDrawer : MonoBehaviour
{
    // Вызывается для отрисовки Gizmos в редакторе Unity
    private void OnDrawGizmos()
    {
        // Получаем компонент Collider2D, привязанный к объекту
        Collider2D collider = GetComponent<Collider2D>();

        if (collider != null)
        {
            
            // В зависимости от типа коллайдера отрисовываем его границы
            if (collider is BoxCollider2D boxCollider)
            {
                // Настройка цвета обводки
                Gizmos.color = Color.red;
                DrawBoxCollider(boxCollider);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                // Настройка цвета обводки
                Gizmos.color = Color.blue;
                DrawCircleCollider(circleCollider);
            }
            else if (collider is PolygonCollider2D polygonCollider)
            {
                // Настройка цвета обводки
                Gizmos.color = Color.yellow;
                DrawPolygonCollider(polygonCollider);
            }
        }
    }

    // Рисуем BoxCollider2D
    private void DrawBoxCollider(BoxCollider2D boxCollider)
    {
        Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
    }

    // Рисуем CircleCollider2D
    private void DrawCircleCollider(CircleCollider2D circleCollider)
    {
        Gizmos.DrawWireSphere(circleCollider.bounds.center, circleCollider.radius);
    }

    // Рисуем PolygonCollider2D
    private void DrawPolygonCollider(PolygonCollider2D polygonCollider)
    {
        Vector2[] points = polygonCollider.points;

        if (points.Length < 2) return;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 startPoint = polygonCollider.transform.TransformPoint(points[i]);
            Vector2 endPoint = polygonCollider.transform.TransformPoint(points[(i + 1) % points.Length]);
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}
