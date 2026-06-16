using DS; // AJOUT : Accès au namespace DS pour utiliser DSDialogueFlags
using DS.Data;
using DS.Enumerations;
using DS.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private List<Button> choicesButton = new List<Button>();
    private int selectedChoiceIndex = 0;

    private DSDialogueSO currentNode;
    Actor currentActor;

    public static bool isActive = false;

    private float inputLockDuration = 2f;
    private float lastTransitionTime = 0;
    private bool IsInputLocked => Time.unscaledTime - lastTransitionTime < inputLockDuration;


    #region Node Methods
    public void OpenDSDialogue(DSDialogueSO startingNode, Actor actor)
    {
        currentActor = actor;
        isActive = true;

        actorName.text = actor.name;

        backgroundBox.LeanScale(Vector3.one, 0.5f).setEaseInOutExpo();

        DisplayDSNode(startingNode);
    }

    private void DisplayDSNode(DSDialogueSO node)
    {
        lastTransitionTime = Time.unscaledTime;

        currentNode = node;
        messageText.text = node.Text;
        AnimateTextColor();

        choicesButton.Clear();
        selectedChoiceIndex = 0;

        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (node.DialogueType == DSDialogueType.SingleChoice)
        {
            choicesPanel.SetActive(false);
        }
        else
        {
            choicesPanel.SetActive(true);

            foreach (DSDialogueChoiceData choiceDialogue in node.Choices)
            {
                // MODIFICATION : Utilisation directe de ton script DSDialogueFlags pour masquer le choix si requis
                if (!DSDialogueFlags.IsChoiceAvailable(choiceDialogue))
                {
                    continue; // On passe au choix suivant sans créer de bouton (le choix est caché)
                }

                if (choiceDialogue.NextDialogue != null)
                {
                    // MODIFICATION : On passe l'objet choiceDialogue entier pour appliquer les effets au clic
                    SpawnChoiceButton(choiceDialogue);
                }
                else
                {
                    // MODIFICATION : Idem pour la fin du dialogue
                    SpawnEndDialogue(choiceDialogue);
                }
            }

            // SÉCURITÉ : Si aucun choix n'est visible à cause des conditions de flags, on ferme le dialogue
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
        if (!isActive || currentNode == null || currentNode.DialogueType == DSDialogueType.MultipleChoice)
            return;

        DSDialogueChoiceData choice = currentNode.Choices[0];

        // MODIFICATION : Applique l'effet du flag (OnChosenFlag) pour un nœud simple à choix unique
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

    // MODIFICATION : Prend désormais le DSDialogueChoiceData en paramètre
    private void SpawnChoiceButton(DSDialogueChoiceData choiceData)
    {
        GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);

        btn.GetComponentInChildren<TMP_Text>().text = choiceData.Text;

        Button button = btn.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            // MODIFICATION : Applique le changement de flag au clic avant de charger le nœud suivant
            DSDialogueFlags.ApplyChoiceEffect(choiceData);
            DisplayDSNode(choiceData.NextDialogue);
        });

        AddPointerEnterCallback(btn, button);

        choicesButton.Add(button);
    }

    // MODIFICATION : Prend désormais le DSDialogueChoiceData en paramètre
    private void SpawnEndDialogue(DSDialogueChoiceData choiceData)
    {
        GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);
        Button button = btn.GetComponent<Button>();

        btn.GetComponentInChildren<TMP_Text>().text = choiceData.Text;

        button.onClick.AddListener(() =>
        {
            // MODIFICATION : Applique le changement de flag au clic avant de fermer le dialogue
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
        if (!isActive || currentNode == null ||
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
    void AnimateTextColor()
    {
        messageText.alpha = 0f;
        LeanTween.value(gameObject, 0f, 1f, 0.5f).setOnUpdate((float val) => { messageText.alpha = val; }).setEaseInOutSine();
    }

    public void CloseDialogue()
    {
        isActive = false;

        currentNode = null;
        choicesPanel.SetActive(false);

        backgroundBox.LeanScale(Vector3.zero, 0.5f);

        FindFirstObjectByType<PlayerControllerTPS>().LockCamera(false);
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
        choicesPanel.SetActive(false);
    }
    #endregion
}