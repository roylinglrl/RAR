using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public interface IIntractable
{
    public UnityAction<IIntractable> OnInteractionComplete { get; set; }
    public void Interact(Interactor interactor,out bool interactSuccessfully);
    public void EndInteraction();
}