using System.Linq;
using UnityEngine;

public class ParagliderComputer : MonoBehaviour
{
    public PolygonCollider2D terrainCollider;

    public float GetAGL()
    {
        if (terrainCollider == null) return float.MaxValue;

        float paragliderX = transform.position.x;
        float paragliderY = transform.position.y;

        var closestPoint = terrainCollider.points
            .Select(p => terrainCollider.transform.TransformPoint(p))
            .OrderBy(p => Mathf.Abs(p.x - paragliderX))
            .FirstOrDefault();
        return Mathf.RoundToInt(paragliderY - closestPoint.y)*10f;
    }
}
