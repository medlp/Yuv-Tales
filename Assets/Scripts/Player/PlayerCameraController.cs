using UnityEngine;
using Unity.Cinemachine;

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

    private CinemachineOrbitalFollow normalOrbit;
    private CinemachinePanTilt fpsAim;
    private Transform cameraTransform;

    public bool IsZooming { get; private set; }

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
}
