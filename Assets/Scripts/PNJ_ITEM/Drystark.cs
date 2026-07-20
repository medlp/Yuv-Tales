using DS;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Drystark : MonoBehaviour
{
    [SerializeField] private float timeToWait = 300f;
    private float cooldown = 0f;

    private string waitFlag = "Wait_5_Minutes";
    private string waitDialogue = "WAIT";
    
    private string pickaxeDialogue = "GP";
    private string startDialogue = "Global";

    private string havePickaxe = "Has_Pickaxe";

    private DialogueTrigger dialogueTrigger;
    private bool isWaitingSetupDone = false;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    void Update()
    {
        if (DSDialogueFlags.Get("Craft_Pickaxe"))
        {
            DSDialogueFlags.Set("Citizens_Hint", false);
            DSDialogueFlags.Set("Crystals_Blocking", false);
        }

        if (DSDialogueFlags.Get(waitFlag))
        {
            if (!isWaitingSetupDone)
            {
                dialogueTrigger.startingDialogueName = waitDialogue;
                isWaitingSetupDone = true;
            }

            if (cooldown < timeToWait)
            {
                cooldown += Time.deltaTime;
                Debug.Log($"cooldown : {cooldown}");
            }
            else
            {
                cooldown = 0f; 
                isWaitingSetupDone = false;

                DSDialogueFlags.Set(waitFlag, false);
                DSDialogueFlags.Set("Craft_Pickaxe", false);

                dialogueTrigger.startingDialogueName = pickaxeDialogue;
            }
        }

        if (DSDialogueFlags.Get(havePickaxe))
        {
            dialogueTrigger.startingDialogueName = startDialogue;
        }
    }
}
