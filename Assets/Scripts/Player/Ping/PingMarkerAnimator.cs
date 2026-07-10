using UnityEngine;

public class PingMarkerAnimator : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f; // degrés / seconde
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Oscillation verticale")]
    [SerializeField] private float height = 0.5f;
    [SerializeField] private float speed = 2f;

    private Vector3 startLocalPos;

    private void Awake()
    {
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        startLocalPos = pos;
    }

    private void Update()
    {
        // Rotation continue
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);

        // Oscillation en hauteur
        float yOffset = Mathf.Sin(Time.time * speed) * height;
        transform.localPosition = startLocalPos + new Vector3(0f, yOffset, 0f);
    }
}