using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Portal : MonoBehaviour
{
    [Header("Portal destino")]
    public Portal targetPortal;

    [Header("Delay anti-loop (s)")]
    public float cooldown = 0.15f;

    private bool canTeleport = true;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!canTeleport) return;
        if (targetPortal == null) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        // Calcula velocidade atual (inércia)
        Vector2 velocity = rb.linearVelocity;

        // Teletransporta para a posição do portal destino
        Vector3 pos = targetPortal.transform.position;
        other.transform.position = pos;

        // Mantém a velocidade
        rb.linearVelocity = velocity;

        // Liga o cooldown no portal destino para impedir teleporte imediato
        targetPortal.DisableTeleportFor(cooldown);
    }

    public void DisableTeleportFor(float t)
    {
        StartCoroutine(TeleportCooldown(t));
    }

    private System.Collections.IEnumerator TeleportCooldown(float t)
    {
        canTeleport = false;
        yield return new WaitForSeconds(t);
        canTeleport = true;
    }
}
