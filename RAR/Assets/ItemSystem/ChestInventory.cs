#nullable enable
using UnityEngine.Events;

public class ChestInventory : InventoryHolder,IIntractable
{
    public string? LootTableID;
    public UnityAction<IIntractable>? OnInteractionComplete { get; set; }
    public void Interact(Interactor interactor,out bool interactSuccessfully)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem);
        interactSuccessfully = true;
    }
    public void EndInteraction()
    {
        
    }
}