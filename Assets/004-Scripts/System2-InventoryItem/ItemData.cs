using UnityEngine;

public enum ItemType
{
    Resource,
    Tool,
    Seed,
    CraftedObject
}

public enum ItemUsage
{
    None,
    Equippable,
    Consumable,
    Placable
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField, TextArea(4, 8)] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public ItemType Type { get; private set; }
    [field: SerializeField] public ItemUsage Usage { get; private set; }

    [Tooltip("Maximum amount of this item that can be in a single slot.")]
    [field: SerializeField] public int MaxStackSize { get; private set; } = 10;

    [Tooltip("Can this item be placed on the hotbar and used/equipped?")]
    [field: SerializeField] public bool IsEquipable { get; private set; }
    [field: SerializeField] public GameObject WorldPrefab { get; private set; }
}
