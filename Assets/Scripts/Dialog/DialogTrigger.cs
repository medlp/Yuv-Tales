using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Message[] messages;
    public Actor[] actors;

    public void StartDialogue()
    {
        FindFirstObjectByType<DialogManager>().OpenDialogue(messages, actors);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DialogManager.isActive)
                FindFirstObjectByType<DialogManager>().CloseDialogue();
        }
        
    }
}

[System.Serializable]
public class Message
{
    public int actorID;
    public string message;
}

[System.Serializable]
public class Actor 
{ 
    public string name;
    public Sprite sprite;
}