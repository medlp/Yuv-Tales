using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public class FamiliarPingController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float maxPingDistance = 50f;
    [SerializeField] private CompanionFollowing familiarAgent;
    [SerializeField] private GameObject pingMarkerPrefab;

    private GameObject currentMarker;


    private void OnEnable()
    {
        familiarAgent.OnPingArrived += HandlePingArrived;
    }

    private void OnDisable()
    {
        familiarAgent.OnPingArrived -= HandlePingArrived;
    }

    private void HandlePingArrived()
    {
        if (currentMarker != null)
        {
            Destroy(currentMarker);
            currentMarker = null;
        }
    }

    public void OnPingPerformed(InputAction.CallbackContext context)
    {
        Ray ray = mainCamera.ScreenPointToRay(GetAimScreenPosition());

        if (Physics.Raycast(ray, out RaycastHit hit, maxPingDistance, groundLayerMask))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                familiarAgent.Ping(navHit.position);
                SpawnPingMarker(navHit.position);
            }
        }
    }

    private Vector3 GetAimScreenPosition()
    {
        return new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    }

    private void SpawnPingMarker(Vector3 position)
    {
        if (currentMarker != null)
            Destroy(currentMarker);

        currentMarker = Instantiate(pingMarkerPrefab, position, Quaternion.identity);
    }
}
