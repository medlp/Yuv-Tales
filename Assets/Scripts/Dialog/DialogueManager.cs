using DS.Data;
using DS.Enumerations;
using DS.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using YuvTales.UI.Dialogue;
using YuvTales.UI.Core;

public class DialogueManager : MonoBehaviour
{

    [Header("UI References (Legacy - Kept for Unity references)")]
    public TMP_Text actorName;
    public TMP_Text messageText;
    public RectTransform backgroundBox;

    [Header("Choices UI (Legacy - Kept for Unity references)")]
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

    private DialogueMenuController _uiController;


    #region Node Methods
    public void OpenDSDialogue(DSDialogueSO startingNode, Actor actor)
    {
        currentActor = actor;
        isActive = true;

        // Legacy UI
        // actorName.text = actor.name;
        // backgroundBox.LeanScale(Vector3.one, 0.5f).setEaseInOutExpo();

        // UI Toolkit
        UIManager.Instance.ShowPanel(PanelType.Dialogue);
        _uiController = FindFirstObjectByType<DialogueMenuController>();

        DisplayDSNode(startingNode);
    }

    private void DisplayDSNode(DSDialogueSO node)
    {
        lastTransitionTime = Time.unscaledTime;
        currentNode = node;

        // Legacy UI Setup
        /*
        messageText.text = node.Text;
        AnimateTextColor();
        choicesButton.Clear();
        selectedChoiceIndex = 0;
        foreach(Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        */

        // UI Toolkit Data Setup
        DialogueData data = new DialogueData
        {
            ActorName = currentActor != null ? currentActor.name : "Unknown",
            MessageText = node.Text
        };

        if (node.DialogueType == DSDialogueType.SingleChoice)
        {
            // choicesPanel.SetActive(false); // Legacy
        }
        else
        {
            // choicesPanel.SetActive(true); // Legacy

            foreach (DSDialogueChoiceData choiceDialogue in node.Choices)
            {
                DialogueChoiceData choiceData = new DialogueChoiceData
                {
                    Text = choiceDialogue.Text
                };

                if (choiceDialogue.NextDialogue != null)
                {
                    // SpawnChoiceButton(choiceDialogue.Text, choiceDialogue.NextDialogue); // Legacy
                    choiceData.OnSelected = () => DisplayDSNode(choiceDialogue.NextDialogue);
                }
                else
                {
                    // SpawnEndDialogue(choiceDialogue.Text); // Legacy
                    choiceData.OnSelected = () => CloseDialogue();
                }

                data.Choices.Add(choiceData);
            }

            // SetupButtonNavigation(); // Legacy
            // StartCoroutine(SelectButtonNextFrame()); // Legacy
        }

        if (_uiController != null)
        {
            _uiController.DisplayDialogue(data);
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

        if(choice.NextDialogue != null)
        {
            DisplayDSNode(choice.NextDialogue);
        }
        else
        {
            CloseDialogue();
        }
    }

    private void SpawnChoiceButton(string text, DSDialogueSO nextDialogue)
    {
        GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);

        btn.GetComponentInChildren<TMP_Text>().text = text;

        Button button = btn.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            DisplayDSNode(nextDialogue);
        });

        AddPointerEnterCallback(btn, button);

        choicesButton.Add(button);
    }

    private void SpawnEndDialogue(string text)
    {
        GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);
        Button button = btn.GetComponent<Button>();

        btn.GetComponentInChildren<TMP_Text>().text = text;

        button.onClick.AddListener(() =>
        {
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
        // messageText.alpha = 0f;
        // LeanTween.value(gameObject, 0f, 1f, 0.5f).setOnUpdate((float val) =>{messageText.alpha = val;}).setEaseInOutSine();
    }

    public void CloseDialogue()
    {
        isActive = false;

        currentNode = null;
        // choicesPanel.SetActive(false); // Legacy
        // backgroundBox.LeanScale(Vector3.zero, 0.5f); // Legacy

        // Note: Camera unlock is now handled by DialogueMenuController.OnHide()
        UIManager.Instance.HidePanel(PanelType.Dialogue);
        _uiController = null;
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (backgroundBox != null)
            backgroundBox.transform.localScale = Vector3.zero;
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
    }
    #endregion

}