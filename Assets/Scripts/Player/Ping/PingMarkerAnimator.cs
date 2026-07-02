using UnityEngine;

public class PingMarkerAnimator : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f; // degrés / seconde
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Oscillation verticale")]
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startLocalPos;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        // Rotation continue
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);

        // Oscillation en hauteur
        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPos + new Vector3(0f, yOffset, 0f);
    }
}