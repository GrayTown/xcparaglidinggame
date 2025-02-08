
using UnityEngine;

public class TakeCollideEvent : MonoBehaviour
{
    [Header("Events")]
    public GameEvent collideWithTermikEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            collideWithTermikEvent.Raise(this, this.name);
        }
    }
}
