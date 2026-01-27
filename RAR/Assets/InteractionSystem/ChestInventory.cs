using UnityEngine;
using UnityEngine.Events;

public class ChestInventory : InventoryHolder, IIntractable
{
    public string interactionName = "打开宝箱";
    
    public UnityAction<IIntractable> OnInteractionComplete { get; set; }
    
    public string InteractionName => interactionName;
    
    public void Interact(Interactor interactor, out bool interactSuccessfully)
    {
        // 这里触发宝箱打开逻辑
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem,this);
        interactSuccessfully = true;
    }
    
    public void EndInteraction()
    {
        // 宝箱关闭逻辑
        Debug.Log("宝箱关闭");
    }
    public void NotifyInteractionCompleted()
    {
        // 当玩家关闭宝箱UI时调用这个方法
        OnInteractionComplete?.Invoke(this);
        Debug.Log("宝箱交互已完成");
    }
}