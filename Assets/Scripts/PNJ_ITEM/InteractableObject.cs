using UnityEngine;
using DS;

public abstract class InteractableObject : MonoBehaviour
{
    private string interactableID;
    public string InteractableID => interactableID;

    protected virtual void Awake()
    {
        interactableID = gameObject.name + "_" + transform.position.ToString("F2");
    }

    protected void MarkDestroyed()
    {
        DSDialogueFlags.Set("Destroyed_" + interactableID, true);
    }
}