using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Lever : MonoBehaviour
{
    public UnityEvent onSwitchOn;
    public UnityEvent onSwitchOff;

    public bool on;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Mathf.Abs(collision.transform.rotation.z-transform.rotation.z)<0.1f)
        {
            if (on && collision.GetComponentInChildren<SpriteRenderer>().flipX)
            {
                onSwitchOff.Invoke();
                on = false;
            }
            else if (!collision.GetComponentInChildren<SpriteRenderer>().flipX)
            {
                onSwitchOn.Invoke();
                on = true;
            }
        }
    }
}
