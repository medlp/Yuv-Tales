using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; } 

    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);

    private Transform target;
    private Camera cam;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
        promptRoot.SetActive(false);
    }

    public void Show(string prompt, Transform anchor)
    {
        target = anchor;
        promptText.text = prompt;


        if (IsBlocked())
        {
            promptRoot.SetActive(false);
            return;
        }

        promptRoot.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        promptRoot.SetActive(false);
    }

    private bool IsBlocked()
    {
        GameManager gm = GameManager.Instance;
        bool isBlocked = gm != null && gm.CurrentState != GameState.Gameplay;
        return isBlocked;
    }

    private void LateUpdate()
    {
        if (target == null || IsBlocked())
        {
            promptRoot.SetActive(false);
            return;
        }

        Vector3 screenPos = cam.WorldToScreenPoint(target.position + worldOffset);
        if (screenPos.z < 0) { promptRoot.SetActive(false); return; } // cible derrière la caméra

        promptRoot.SetActive(true);
        promptRoot.transform.position = screenPos;
    }
}