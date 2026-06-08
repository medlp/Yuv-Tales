using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using DS.Data;
using DS.ScriptableObjects;
using DS.Enumerations;
using System;

public class DialogueManager : MonoBehaviour
{

    [Header("UI References")]
    public Image actorImage;
    public TMP_Text actorName;
    public TMP_Text messageText;
    public RectTransform backgroundBox;

    [Header("Choices UI")]
    public GameObject choicesPanel;
    public GameObject choiceButtonPrefab;


    private DSDialogueSO currentNode;
    Actor currentActor;

    public static bool isActive = false;

    #region Node Methods
    public void OpenDSDialogue(DSDialogueSO startingNode, Actor actor)
    {
        currentActor = actor;
        isActive = true;

        actorName.text = actor.name;
        actorImage.sprite = actor.sprite;

        backgroundBox.LeanScale(Vector3.one, 0.5f).setEaseInOutExpo();

        DisplayDSNode(startingNode);
    }

    private void DisplayDSNode(DSDialogueSO node)
    {
        currentNode = node;
        messageText.text = node.Text;
        AnimateTextColor();

        foreach(Transform child in choicesPanel.transform)
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
                if (choiceDialogue.NextDialogue != null)
                {
                    SpawnChoiceButton(choiceDialogue.Text, choiceDialogue.NextDialogue);
                }
                else
                {
                    SpawnEndDialogue();
                }
            }
        }
    }

    public void NextNode()
    {
        if (!isActive || currentNode == null || currentNode.DialogueType == DSDialogueType.MultipleChoice)
        {
            return;
        }

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
        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            DisplayDSNode(nextDialogue);
        });
    }

    private void SpawnEndDialogue()
    {
        GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);
        btn.GetComponentInChildren<TMP_Text>().text = "Close";
        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            CloseDialogue();
        });
    }
#endregion

    #region Utility Methods

    void AnimateTextColor()
    {
        messageText.alpha = 0f;

        LeanTween.value(gameObject, 0f, 1f, 0.5f).setOnUpdate((float val) =>{messageText.alpha = val;}).setEaseInOutSine();

    }

    public void CloseDialogue()
    {
        Debug.Log("Conversation ended !");
        isActive = false;

        currentNode = null;
        choicesPanel.SetActive(false);

        backgroundBox.LeanScale(Vector3.zero, 0.5f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
        choicesPanel.SetActive(false);
    }
    #endregion

}