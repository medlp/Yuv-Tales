using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Curseur par défaut")]
    [SerializeField] private Texture2D defaultCursorTexture;
    [SerializeField] private Vector2 defaultHotspot = Vector2.zero;

    [Header("Curseur de jeu")]
    [SerializeField] private Texture2D gameplayCursorTexture;
    [SerializeField] private Vector2 gameplayHotspot = Vector2.zero;

    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    public static CursorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        SetCursor(defaultCursorTexture, defaultHotspot);
    }

    public void SetGameplayCursor()
    {
        SetCursor(gameplayCursorTexture, gameplayHotspot);
    }

    public void SetCursor(Texture2D texture, Vector2 hotspot)
    {
        if (texture == null)
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
            return;
        }

        Cursor.SetCursor(texture, hotspot, cursorMode);
    }

    public void ResetToSystemCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
    }

    public void SetCursorLockState(CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;
    }
}