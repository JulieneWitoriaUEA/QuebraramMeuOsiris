using UnityEngine;
using System.Collections;
using System;

public class MapRotator : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode rotateCWKey = KeyCode.E;   // clockwise
    [SerializeField] private KeyCode rotateCCWKey = KeyCode.Q;  // counterclockwise

    [Header("Rotation")]
    [SerializeField] private float rotationDuration = 0.25f; // seconds per 90�
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private IntVariable totalRotationsSO;

    [Header("Player")]
    [SerializeField] private Rigidbody2D playerRb; // assign your player; we�ll freeze it while rotating

    [Header("Shadow Player")]
    [SerializeField] private Rigidbody2D shadowPlayerRb; // optional shadow player rotation

    private bool _isRotating;
    private bool _canRotate;
    private float _currentZ; // track world z rotation in degrees (multiples of 90)

    private void Awake()
    {
        // Initialize to nearest 90� if needed
        Vector3 e = transform.eulerAngles;
        totalRotationsSO.value = 0;
        _currentZ = RoundToRightAngle(e.z);
        transform.rotation = Quaternion.Euler(0, 0, _currentZ);
        EventManager.AddListener("PlayerHitGround", OnHitGround);
    }

    private void OnHitGround()
    {
        _canRotate = true;
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener("PlayerHitGround", OnHitGround);
    }

    private void Update()
    {
        if (_isRotating||!_canRotate) return;

        if (Input.GetKeyDown(rotateCWKey))
        {
            StartCoroutine(RotateBy(-90f)); // world rotates CW ? negative Z (screen coords)
        }
        else if (Input.GetKeyDown(rotateCCWKey))
        {
            StartCoroutine(RotateBy(90f));
        }
    }

    private IEnumerator RotateBy(float delta)
    {
        _isRotating = true;
        _canRotate = false;
        totalRotationsSO.value++;
        EventManager.TriggerEvent("StartRotation");

        // Freeze player so they don�t slide while the floor turns
        bool hadPlayer = playerRb != null;
        RigidbodyConstraints2D oldConstraints = RigidbodyConstraints2D.None;
        if (hadPlayer)
        {
            oldConstraints = playerRb.constraints;
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        bool hadShadowPlayer = shadowPlayerRb != null;
        RigidbodyConstraints2D shadowOldConstraints = RigidbodyConstraints2D.None;
        if (hadShadowPlayer)
        {
            shadowOldConstraints = shadowPlayerRb.constraints;
            shadowPlayerRb.linearVelocity = Vector2.zero;
            shadowPlayerRb.angularVelocity = 0f;
            shadowPlayerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }


        float start = _currentZ;
        float end = _currentZ + delta;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / rotationDuration;
            float z = Mathf.LerpAngle(start, end, ease.Evaluate(Mathf.Clamp01(t)));
            transform.rotation = Quaternion.Euler(0, 0, z);
            playerRb.transform.eulerAngles = Vector3.zero;
            if(hadShadowPlayer)
                shadowPlayerRb.transform.eulerAngles = Vector3.zero;
            yield return null;
        }

        _currentZ = RoundToRightAngle(end);
        transform.rotation = Quaternion.Euler(0, 0, _currentZ);

        // Unfreeze player
        if (hadPlayer)
            playerRb.constraints = oldConstraints;

        if (hadShadowPlayer)
            shadowPlayerRb.constraints = shadowOldConstraints;

        _isRotating = false;
        EventManager.TriggerEvent("EndRotation");
    }

    private static float RoundToRightAngle(float z)
    {
        // Snap to �,-270,-180,-90,0,90,180,270,�
        float snapped = Mathf.Round(z / 90f) * 90f;
        // Normalize to [0,360)
        snapped = Mathf.Repeat(snapped, 360f);
        return snapped;
    }
}
