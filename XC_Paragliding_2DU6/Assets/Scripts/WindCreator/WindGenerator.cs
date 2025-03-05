using UnityEngine;

public class WindGenerator : MonoBehaviour
{
    [SerializeField] private float gizmosPointSize = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ✅ Рисуем точку в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta; // Цвет точки
        Gizmos.DrawSphere(transform.position, gizmosPointSize); // Радиус 0.5 для наглядности
    }
}
