
public class HotBarDisplay : StaticInventoryDisplay
{
    public bool canClick = false;

    public override void AssignSlot(InventorySystem inventorySystem)
    {
        base.AssignSlot(inventorySystem);
        for (int i = 0; i < inventorySlotForUI.Length; i++)
        {
            inventorySlotForUI[i].canClick = canClick;
        }
    }
}