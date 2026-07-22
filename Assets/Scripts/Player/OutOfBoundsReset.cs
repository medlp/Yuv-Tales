using UnityEngine;

public class OutOfBoundsReset : MonoBehaviour
{
    [Header("Détection de chute")]
    [Tooltip("Altitude Y en dessous de laquelle le joueur est considéré 'hors-map'")]
    [SerializeField] private float fallThresholdY = 0f;

    [Header("Paramètres de repositionnement")]
    [Tooltip("Altitude maximale d'où partira le raycast depuis le ciel")]
    [SerializeField] private float skyHeightY = 500f;

    [Tooltip("Calque(s) considéré(s) comme du sol valide")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Marge pour faire réapparaître le joueur un peu au-dessus du sol")]
    [SerializeField] private float spawnOffset = 1.2f;

    [Header("Point de secours (Fallback)")]
    [Tooltip("Si le raycast ne trouve aucun sol, téléporte le joueur ici")]
    [SerializeField] private Transform fallbackSpawnPoint;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Vérifie si le joueur est tombé sous le seuil critique
        if (transform.position.y < fallThresholdY)
        {
            TryResetPosition();
        }
    }

    private void TryResetPosition()
    {
        //Point d'origine du Raycast (meme X et Z que le joueur, mais Y plsu haut)
        Vector3 rayOrigin = new Vector3(transform.position.x, skyHeightY, transform.position.z);

        // Distance maximale du raycast
        float maxDistance = skyHeightY - fallThresholdY + 10f;

        //On lance le raycast du haut vers le bas
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, maxDistance, groundLayer))
        {
            Vector3 targetPosition = hit.point + Vector3.up * spawnOffset;
            TeleportPlayer(targetPosition);
        }
        else
        {
            // Aucun sol trouvé au-dessus du joueur
            if (fallbackSpawnPoint != null)
            {
                TeleportPlayer(fallbackSpawnPoint.position);
            }
            else
            {
                Debug.LogWarning("Aucun sol trouvé et pas de FallbackSpawnPoint assigné !");
            }
        }
    }

    private void TeleportPlayer(Vector3 targetPosition)
    {
        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = targetPosition;
            characterController.enabled = true;
        }
        else
        {
            transform.position = targetPosition;
        }

        Debug.Log($"Joueur repositionné avec succès en : {targetPosition}");
    }

    // Dessine une ligne rouge dans l'éditeur pour visualiser la zone de chute
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        center.y = fallThresholdY;
        Gizmos.DrawWireCube(center, new Vector3(20f, 0.1f, 20f));
    }
}