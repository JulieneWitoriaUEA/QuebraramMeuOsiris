using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelDoor : MonoBehaviour
{
    public UnityEvent onEnter;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.transform.rotation.z == transform.rotation.z)
                onEnter.Invoke();
        }
    }
}
