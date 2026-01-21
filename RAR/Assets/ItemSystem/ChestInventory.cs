using UnityEngine.Events;

public class ChestInventory : InventoryHolder,IIntractable
{
    public UnityAction<IIntractable> OnInteractionComplete { get; set; }
    public void Interact(Interactor interactor,out bool interactSuccessfully)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(InventorySystem);
        interactSuccessfully = true;
    }
    public void EndInteraction()
    {
        
    }
}