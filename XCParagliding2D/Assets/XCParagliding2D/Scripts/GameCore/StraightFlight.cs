using UnityEngine;

public class StraightFlight : MonoBehaviour
{
    [Header("Flight Parameters")]
    public float forwardSpeed = 10f; // �������� ������ ������
    public float descentRate = 1.42f; // �������� �������� (����� 7:1)
    public float turnTime = 2f; // �����, ����������� ��� ����� �����������

    private float turnTimer = 0f;
    private int turnDirection = 0; // -1 (�����), 1 (������), 0 (�� ������������)
    private int currentDirection = 1; // 1 - ����� ������, -1 - ����� �����

    void Update()
    {
        // ������������ ���� � ����������
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turnDirection = -1; // �����
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            turnDirection = 1; // ������
        }
        else
        {
            turnDirection = 0; // ���� �� ���������� �� ���� ������, �� ������������
            turnTimer = 0f; // ���������� ������, ����� ��� ��������� ������� ����� ����� turnTime
        }

        // ���� ������ ������, �������� ������ �������
        if (turnDirection != 0)
        {
            turnTimer += Time.deltaTime;
            if (turnTimer >= turnTime)
            {
                currentDirection *= -1; // ������ ����������� �� ���������������
                turnTimer = 0f; // ���������� ������, ����� ����� ����� ��������� �������
            }
        }

        // ������� �������� � ��������� �����������
        transform.position += new Vector3(currentDirection * forwardSpeed * Time.deltaTime, -descentRate * Time.deltaTime, 0);
    }
}

