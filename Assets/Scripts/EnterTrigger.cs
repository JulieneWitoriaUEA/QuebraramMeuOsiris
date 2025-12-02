using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EnterTrigger : MonoBehaviour
{
    public UnityEvent onEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")||collision.CompareTag("ShadowPlayer"))
        {
            AudioSystem.Instance.PlaySFX("GoldenUrn");
            onEnter.Invoke();
        }
    }
}
