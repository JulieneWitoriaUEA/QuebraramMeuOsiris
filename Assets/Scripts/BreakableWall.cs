using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Box")
        {
            Debug.Log("AAAAAAAAAAAA");
            Destroy(gameObject);
        }
    }
}
