using UnityEngine.Events;

public interface IIntractable
{
    UnityAction<IIntractable> OnInteractionComplete { get; set; }
    string InteractionName { get; }  // 显示在HUD上的名称
    
    void Interact(Interactor interactor, out bool interactSuccessfully);
    void EndInteraction();
}