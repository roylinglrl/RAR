using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Interactor : MonoBehaviour
{
    public Transform InteractionPoint;
    public LayerMask InteractionLayerMask;
    public float InteractionRange = 1f;
    public bool IsInteracting { get; private set; }
    private void Update()
    {
        var colliders = Physics2D.OverlapCircleAll(InteractionPoint.position, InteractionRange, InteractionLayerMask);
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent(out IIntractable intractable))
                {
                    StartInteraction(intractable);
                    break;
                }
            }
        }
    }
    void StartInteraction(IIntractable intractable)
    {
        intractable.Interact(this, out bool interactSuccessfully);
        if (interactSuccessfully)
        {
            IsInteracting = true;
            intractable.OnInteractionComplete += OnInteractionComplete;
        }
    }
        void OnInteractionComplete(IIntractable intractable)
    {
        intractable.OnInteractionComplete -= OnInteractionComplete;
        EndInteraction();
    }
    void EndInteraction()
    {
        IsInteracting = false;
    }
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (InteractionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(InteractionPoint.position, InteractionRange);
        }
    }
    #endif
}