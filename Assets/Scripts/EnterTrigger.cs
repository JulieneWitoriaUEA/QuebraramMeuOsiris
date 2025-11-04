using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EnterTrigger : MonoBehaviour
{
    public UnityEvent onEnter;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            onEnter.Invoke();
        }
    }
}
