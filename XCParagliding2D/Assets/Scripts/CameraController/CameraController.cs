using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (_player != null)
        {
            Vector3 moveToNewPosition = transform.position;
            moveToNewPosition.x = _player.position.x;
            moveToNewPosition.y = _player.position.y;
            transform.position = moveToNewPosition;
        }
    }
}
