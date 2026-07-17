using DS; // AJOUT : Accès au namespace DS pour utiliser DSDialogueFlags
using DS.Data;
using DS.Enumerations;
using DS.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text actorName;
    public TMP_Text messageText;
    public RectTransform backgroundBox;

    [Header("Choices UI")]
    public GameObject choicesPanel;
    public GameObject choiceButtonPrefab;

    [Header("Animation Settings")]
    [Tooltip("Durée de l'apparition/disparition de la boîte")]
    [SerializeField] private float transitionDuration = 0.25f;
    [Tooltip("Temps d'attente entre chaque lettre (en secondes)")]
    [SerializeField] private float typingSpeed = 0.02f; // Plus la valeur est petite, plus c'est rapide

    private List<Button> choicesButton = new List<Button>();
    private int selectedChoiceIndex = 0;

    private DSDialogueSO currentNode;
    Actor currentActor;

    public static bool isActive = false;

    private PlayerControllerTPS playerController;

    private Coroutine scaleCoroutine;
    private Coroutine textFadeCoroutine;

    private bool isTyping = false;
    private string currentFullText = "";

    #region Node Methods
    public void OpenDSDialogue(DSDialogueSO startingNode, Actor actor, bool lockMovement = false)
    {
        currentActor = actor;
        isActive = true;

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(ScaleOverTime(backgroundBox, Vector3.one, transitionDuration));

        DisplayDSNode(startingNode);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateState(GameState.Dialogue);
        }

        if (lockMovement)
        {
            playerController.SetMovementLocked(lockMovement);
        }
    }

    private void DisplayDSNode(DSDialogueSO node)
    {
        currentNode = node;
        currentFullText = node.Text;

        actorName.text = string.IsNullOrEmpty(node.ActorName)
            ? currentActor.name
            : node.ActorName;

        choicesPanel.SetActive(false);
        choicesButton.Clear();
        selectedChoiceIndex = 0;

        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        AnimateTextTyping(currentFullText);
    }

    private void SetupChoicesAfterTyping()
    {
        if (currentNode == null) return;

        if (currentNode.DialogueType == DSDialogueType.SingleChoice)
        {
            choicesPanel.SetActive(false);
        }
        else
        {
            choicesPanel.SetActive(true);

            foreach (DSDialogueChoiceData choiceDialogue in currentNode.Choices)
            {
                if (!DSDialogueFlags.IsChoiceAvailable(choiceDialogue))
                {
                    continue;
                }

                if (choiceDialogue.NextDialogue != null)
                {
                    SpawnChoiceButton(choiceDialogue);
                }
                else
                {
                    SpawnEndDialogue(choiceDialogue);
                }
            }

            if (choicesButton.Count == 0)
            {
                CloseDialogue();
                return;
            }

            SetupButtonNavigation();

            StartCoroutine(SelectButtonNextFrame());
        }
    }

    private IEnumerator SelectButtonNextFrame()
    {
        EventSystem.current.SetSelectedGameObject(null);

        yield return null;

        UpdateChoiceVisual();
    }

    public void NextNode()
    {
        if (!isActive || currentNode == null)
            return;

        if (isTyping)
        {
            FinishTypingInstantly();
            return;
        }

        if (currentNode.DialogueType == DSDialogueType.MultipleChoice)
            return;

        DSDialogueChoiceData choice = currentNode.Choices[0];

        DSDialogueFlags.ApplyChoiceEffect(choice);

        if (choice.NextDialogue != null)
        {
            DisplayDSNode(choice.NextDialogue);
        }
        else
        {
            CloseDialogue();
        }
    }

    private void SpawnChoiceButton(DSDialogueChoiceData choiceData)
    {
        GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);

        btn.GetComponentInChildren<TMP_Text>().text = choiceData.Text;

        Button button = btn.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            DSDialogueFlags.ApplyChoiceEffect(choiceData);
            DisplayDSNode(choiceData.NextDialogue);
        });

        AddPointerEnterCallback(btn, button);

        choicesButton.Add(button);
    }

    private void SpawnEndDialogue(DSDialogueChoiceData choiceData)
    {
        GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);
        Button button = btn.GetComponent<Button>();

        btn.GetComponentInChildren<TMP_Text>().text = choiceData.Text;

        button.onClick.AddListener(() =>
        {
            DSDialogueFlags.ApplyChoiceEffect(choiceData);
            CloseDialogue();
        });

        AddPointerEnterCallback(btn, button);

        choicesButton.Add(button);
    }

    private void AddPointerEnterCallback(GameObject btn, Button button)
    {
        EventTrigger trigger = btn.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = btn.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((_) =>
        {
            if (isTyping) return;

            selectedChoiceIndex = choicesButton.IndexOf(button);
            EventSystem.current.SetSelectedGameObject(null);
        });

        trigger.triggers.Add(entry);
    }
    #endregion

    #region Gamepad Navigation
    public void SetupButtonNavigation()
    {
        for (int i = 0; i < choicesButton.Count; i++)
        {
            Navigation nav = new Navigation { mode = Navigation.Mode.Explicit };

            nav.selectOnUp = i > 0 ? choicesButton[i - 1] : choicesButton[choicesButton.Count - 1];
            nav.selectOnDown = i < choicesButton.Count - 1 ? choicesButton[i + 1] : choicesButton[0];

            choicesButton[i].navigation = nav;
        }
    }

    public void NavigateChoices(Vector2 input)
    {
        if (isTyping || !isActive || currentNode == null ||
               currentNode.DialogueType != DSDialogueType.MultipleChoice ||
               choicesButton == null || choicesButton.Count == 0)
            return;

        if (input.y < -0.5f)
        {
            selectedChoiceIndex = (selectedChoiceIndex + 1) % choicesButton.Count;
            UpdateChoiceVisual();
        }
        else if (input.y > 0.5f)
        {
            selectedChoiceIndex = (selectedChoiceIndex - 1 + choicesButton.Count) % choicesButton.Count;
            UpdateChoiceVisual();
        }
    }

    public void ConfirmChoice()
    {
        if (!isActive || currentNode == null)
            return;

        if (isTyping)
        {
            FinishTypingInstantly();
            return;
        }

        if (currentNode.DialogueType == DSDialogueType.MultipleChoice)
        {
            if (choicesButton.Count > 0)
            {
                choicesButton[selectedChoiceIndex].onClick.Invoke();
            }
        }
        else
        {
            NextNode();
        }
    }

    private void UpdateChoiceVisual()
    {
        if (choicesButton.Count == 0) return;

        choicesButton[selectedChoiceIndex].Select();
    }
    #endregion

    #region Utility Methods
    void AnimateTextTyping(string fullText)
    {
        if (textFadeCoroutine != null)
        {
            StopCoroutine(textFadeCoroutine);
        }

        textFadeCoroutine = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string textToType) // Anim texte : effet Typewriter et fondu
    {
        isTyping = true;
        messageText.text = textToType;
        messageText.ForceMeshUpdate(); 

        TMP_TextInfo textInfo = messageText.textInfo;
        int totalVisibleCharacters = textInfo.characterCount;

        for (int i = 0; i < totalVisibleCharacters; i++)
        {
            SetCharacterAlpha(i, 0);
        }

        for (int i = 0; i < totalVisibleCharacters; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                yield return new WaitForSeconds(typingSpeed);
                continue;
            }

            StartCoroutine(FadeInCharacter(i, 0.15f)); // 0.15s de durée de fondu par lettre

            yield return new WaitForSeconds(typingSpeed);
        }

        OnTypingComplete();
    }

    private IEnumerator FadeInCharacter(int charIndex, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 255f, elapsed / duration);

            SetCharacterAlpha(charIndex, (byte)alpha);

            yield return null;
        }

        SetCharacterAlpha(charIndex, 255);
    }

    private void SetCharacterAlpha(int charIndex, byte alpha) // Change l'opacité de chaque lettre en passant par les 4 coins qui la comopose
    {
        TMP_TextInfo textInfo = messageText.textInfo;

        if (charIndex >= textInfo.characterCount) return;

        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
        Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

        if (vertexIndex + 3 < vertexColors.Length)
        {
            vertexColors[vertexIndex + 0].a = alpha;
            vertexColors[vertexIndex + 1].a = alpha;
            vertexColors[vertexIndex + 2].a = alpha;
            vertexColors[vertexIndex + 3].a = alpha;
        }

        messageText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void FinishTypingInstantly()
    {
        if (textFadeCoroutine != null)
        {
            StopAllCoroutines(); // Coupe l'écriture principale ET tous les fondus de lettres individuels en cours
        }

        messageText.text = currentFullText;
        messageText.alpha = 1f;
        messageText.ForceMeshUpdate();

        TMP_TextInfo textInfo = messageText.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            SetCharacterAlpha(i, 255);
        }

        OnTypingComplete();
    }

    private void OnTypingComplete()
    {
        isTyping = false;
        textFadeCoroutine = null;

        // Une fois que l'ecriture est terminée on genere les choix 
        SetupChoicesAfterTyping();
    }

    public void CloseDialogue()
    {
        isActive = false;

        currentNode = null;
        choicesPanel.SetActive(false);

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(ScaleOverTime(backgroundBox, Vector3.zero, transitionDuration));

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateState(GameState.Gameplay);
        }
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 targetScale, float duration) // Scale de la DialogueBox quand elle apparait et disparait
    {
        Vector3 startScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            float t = Mathf.SmoothStep(0f, 1f, percent);

            target.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        target.localScale = targetScale;
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
        choicesPanel.SetActive(false);
        playerController = FindFirstObjectByType<PlayerControllerTPS>();

    }
    #endregion
}