using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "YuvTales/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identité")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Comportement")]
    public bool isStackable = true;
    public int maxStack = 99;
}