using UnityEngine;
using UnityEngine.Events;

public class LevelDoor : MonoBehaviour
{
    public UnityEvent onEnter;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("ShadowPlayer"))
        {
            Vector2 doorDir = transform.up;
            Vector2 playerDir = collision.transform.up;

            float angle = Vector2.Angle(playerDir, doorDir);

            if (angle > 179f)
                angle = 0f;

            if (angle < 5f)
                onEnter.Invoke();
        }
    }
}
