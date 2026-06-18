using UnityEngine;

public class DigZone : MonoBehaviour
{
    [SerializeField] private Material dugMaterial; 
    private bool isAlreadyDug = false;

    public bool IsAlreadyDug => isAlreadyDug;

    public void OnDigComplete()
    {
        if (isAlreadyDug) return;

        isAlreadyDug = true;

        if (GetComponent<Renderer>() != null && dugMaterial != null)
        {
            GetComponent<Renderer>().material = dugMaterial;
        }

        Debug.Log("La zone a été fouillée avec succès !");
    }
}

