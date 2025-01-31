using UnityEngine;
public class AutoScroll : MonoBehaviour
{
    public float moveSpeed = 5f; // �������� ��������

    void Update()
    {
        // �������� ���� �� ������
        float moveX = Input.GetAxis("Horizontal"); // A/D ��� ������� �����/������
        float moveY = Input.GetAxis("Vertical");   // W/S ��� ������� �����/����

        // ������� ������ ��������
        Vector2 movement = new Vector2(moveX, moveY);

        // ����������� ������, ����� ������������ �������� �� ���� �������
        movement = movement.normalized;

        // ���������� ������
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
