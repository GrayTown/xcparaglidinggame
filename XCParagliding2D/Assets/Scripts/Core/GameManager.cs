using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {

        EventManager.Instance.Publish("GameStarted", " � ����������� c������ ������ ����");
    }

    public void EndGame()
    {
        EventManager.Instance.Publish("GameEnded", "���� ���������");
    }

    private void OnApplicationQuit()
    {
        EndGame(); // ���������� ������� ����� �������
    }
}
