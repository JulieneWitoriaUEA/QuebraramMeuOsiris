using UnityEngine;
using UnityEngine.SceneManagement;

public class Flamethrower : MonoBehaviour
{
    [Header("Particle & Ray Settings")]
    [SerializeField] private ParticleSystem flamethrowerParticles;
    [SerializeField] private float rayDistance = 6f;
    [SerializeField] private LayerMask hitMask;

    [Header("Timing")]
    [SerializeField] private float activeTime = 1f;
    [SerializeField] private float interval = 4f;

    private void Start()
    {
        if (flamethrowerParticles == null)
            flamethrowerParticles = GetComponent<ParticleSystem>();
        StartCoroutine(FlamethrowerRoutine());
    }

    private System.Collections.IEnumerator FlamethrowerRoutine()
    {
        while (true)
        {
            flamethrowerParticles.Play();
            StartCoroutine(HandleRaycastWhileActive());

            yield return new WaitForSeconds(activeTime);

            flamethrowerParticles.Stop();

            yield return new WaitForSeconds(interval);
        }
    }

    private System.Collections.IEnumerator HandleRaycastWhileActive()
    {
        while (flamethrowerParticles.isPlaying)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, rayDistance, hitMask);

            // Ajusta o tempo de vida das partículas baseado na distância
            // lifetime = distância / velocidade da partícula

           if(hit.collider != null)
                if (hit.transform.CompareTag("Player"))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }

           yield return null;
        }
    }

#if UNITY_EDITOR
    // Gizmo opcional para ver o raio no editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * rayDistance);
    }
#endif
}
