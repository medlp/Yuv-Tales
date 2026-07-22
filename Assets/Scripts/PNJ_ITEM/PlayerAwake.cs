using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerAwake : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueTrigger dialogueTrigger;

    [Header("Parametres")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool destroyAfterTrigger = true;

    private Collider zoneCollider;
    private bool hasTriggered;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;

        if (dialogueTrigger == null)
            dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    private void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.SlotExists(SaveManager.SelectedSlot))
        {
            hasTriggered = true;
            if (destroyAfterTrigger)
                Destroy(gameObject);
            return;
        }

        StartCoroutine(CheckAlreadyInsideAtStart());
    }

    private IEnumerator CheckAlreadyInsideAtStart()
    {
        // Laisse la physique se stabiliser (colliders/rigidbodies pas encore
        // "enregistrés" au tout premier frame lors d'un chargement de scène).
        yield return new WaitForFixedUpdate();

        if (hasTriggered) yield break;

        Bounds bounds = zoneCollider.bounds;
        Collider[] overlaps = Physics.OverlapSphere(bounds.center, bounds.extents.x);

        foreach (Collider other in overlaps)
        {
            if (other.CompareTag(playerTag))
            {
                TriggerDialogue();
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        hasTriggered = true;
        dialogueTrigger.StartDialogue();

        if (destroyAfterTrigger)
            Destroy(gameObject);
    }
}