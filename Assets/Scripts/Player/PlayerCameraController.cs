using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

/// <summary>
/// Gère le switch entre la caméra TPS normale et la caméra FPS (zoom/visée).
/// Doit être placé sur le même GameObject que PlayerControllerTPS.
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera vcamNormal;
    [SerializeField] private CinemachineCamera vcamAim;

    [Header("Mesh Visibility")]
    [SerializeField] private GameObject characterModel;
    public float cameraHideDistance = 1.0f; 

    // focusDuration = 0.2f;


    private Coroutine focusCoroutine;

    private CinemachineOrbitalFollow normalOrbit;
    private CinemachinePanTilt fpsAim;
    private Transform cameraTransform;

    public bool IsZooming { get; private set; }

    public CinemachineCamera GetVcamNormal() => vcamNormal;
    public CinemachineCamera GetVcamAim() => vcamAim;

    void Awake()
    {
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (vcamNormal != null)
            normalOrbit = vcamNormal.GetComponent<CinemachineOrbitalFollow>();

        if (vcamAim != null)
            fpsAim = vcamAim.GetComponent<CinemachinePanTilt>();
    }

    void Update()
    {
        HandleMeshVisibility();

        if (IsZooming && cameraTransform != null && !characterModel.activeSelf)
        {
            float targetRotationY = cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetRotationY, 0f);
        }

        
    }

    /// <summary>
    /// Bascule entre mode TPS et FPS. Appelé par PlayerControllerTPS.
    /// </summary>
    public void ToggleZoom(bool isCrouching)
    {
        if (isCrouching || vcamAim == null) return;

        if (!IsZooming)
        { 
            if (fpsAim != null)
            {
                fpsAim.PanAxis.Value = cameraTransform.eulerAngles.y;

                float camTilt = cameraTransform.eulerAngles.x;
                if (camTilt > 180f) camTilt -= 360f;
                fpsAim.TiltAxis.Value = camTilt;
            }

            vcamAim.Priority = 20;
            IsZooming = true;
        }
        else
        {
            if (normalOrbit != null)
                normalOrbit.HorizontalAxis.Value = cameraTransform.eulerAngles.y;

            vcamAim.Priority = 0;
            IsZooming = false;
        }
    }

    private void HandleMeshVisibility()
    {
        if (characterModel == null || cameraTransform == null) return;

        CharacterController cc = GetComponent<CharacterController>();
        Vector3 center = cc != null ? transform.position + cc.center : transform.position;
        float distance = Vector3.Distance(cameraTransform.position, center);

        characterModel.SetActive(distance > cameraHideDistance);
    }

    public void FocusBehindPlayer(float focusTime)
    {
        if (focusCoroutine != null)
            StopCoroutine(focusCoroutine);

        focusCoroutine = StartCoroutine(SmoothFocus(focusTime));
    }

    private IEnumerator SmoothFocus(float focus)
    {
        float elapsed = 0f;

        if (IsZooming && fpsAim != null)
        {
            float startAngle = fpsAim.PanAxis.Value;
            float targetAngle = transform.eulerAngles.y;

            while (elapsed < focus)
            {
                elapsed += Time.deltaTime;
                fpsAim.PanAxis.Value = Mathf.LerpAngle(startAngle, targetAngle, elapsed / focus);
                yield return null;
            }

            fpsAim.PanAxis.Value = targetAngle;
        }
        else if (!IsZooming && normalOrbit != null)
        {
            float startAngle = normalOrbit.HorizontalAxis.Value;
            float targetAngle = transform.eulerAngles.y;

            while (elapsed < focus)
            {
                elapsed += Time.deltaTime;
                normalOrbit.HorizontalAxis.Value = Mathf.LerpAngle(startAngle, targetAngle, elapsed / focus);
                yield return null;
            }

            normalOrbit.HorizontalAxis.Value = targetAngle;
        }

        focusCoroutine = null;
    }

    public void SnapFocusBehindPlayer()
    {
        if (focusCoroutine != null)
        {
            StopCoroutine(focusCoroutine);
            focusCoroutine = null;
        }

        float targetAngle = transform.eulerAngles.y;

        if (IsZooming && fpsAim != null)
        {
            fpsAim.PanAxis.Value = targetAngle;
        }
        else if (!IsZooming && normalOrbit != null)
        {
            normalOrbit.HorizontalAxis.Value = targetAngle;
        }
    }
}
