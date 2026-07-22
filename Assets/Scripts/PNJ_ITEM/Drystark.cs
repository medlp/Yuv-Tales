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

    private bool givePickaxe = false;
    private bool takeCrystals = false;

    [Header("Items")]
    [SerializeField] private ItemData pickaxe;
    [SerializeField] private ItemData crystals;


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
            if (!takeCrystals)
            {
                GameManager.Instance.GetInventorySystem().TryRemove(crystals, 5);
                takeCrystals = true;
            }

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
            if (!givePickaxe)
            {
                GameManager.Instance.GetInventorySystem().TryAdd(pickaxe, 1);
                givePickaxe = true;
            }
            dialogueTrigger.startingDialogueName = startDialogue;
        }
    }
}
