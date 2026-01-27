using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Transform interactionPoint;
    public LayerMask interactionLayerMask;
    public float interactionRange = 2f;
    
    [Header("UI Reference")]
    public InteractionHUD interactionHUD;
    
    [Header("Selection Settings")]
    public float scrollCooldown = 0.15f;
    
    public bool IsInteracting { get; private set; }
    
    private List<(IIntractable interactable, Transform transform)> availableInteractables = new List<(IIntractable, Transform)>();
    private int currentSelectionIndex = -1;
    private float lastScrollTime = 0f;
    
    // 添加：记录当前交互的对象
    private IIntractable currentInteractable;
    
    private void Start()
    {
        if (interactionHUD == null)
        {
            Debug.LogError("InteractionHUD is not assigned!");
        }
    }
    
    private void Update()
    {
        // 如果正在交互，不更新检测（但保留UI更新）
        UpdateAvailableInteractables();
        UpdateHUD();
        
        if (!IsInteracting)
        {
            HandleSelectionInput();
            HandleInteractionInput();
        }
    }
    
    private void UpdateAvailableInteractables()
    {
        availableInteractables.Clear();
        
        // 检测范围内的所有可交互对象
        Collider2D[] colliders = Physics2D.OverlapCircleAll(interactionPoint.position, interactionRange, interactionLayerMask);
        
        foreach (Collider2D collider in colliders)
        {
            IIntractable interactable = collider.GetComponent<IIntractable>();
            if (interactable != null)
            {
                availableInteractables.Add((interactable, collider.transform));
            }
        }
        
        // 如果没有可交互对象，重置选择并隐藏HUD
        if (availableInteractables.Count == 0)
        {
            currentSelectionIndex = -1;
            interactionHUD?.Hide();
        }
        else if (currentSelectionIndex >= availableInteractables.Count || currentSelectionIndex == -1)
        {
            // 选择最近的
            SelectClosest();
        }
    }
    
    private void SelectClosest()
    {
        if (availableInteractables.Count == 0) return;
        
        float closestDistance = float.MaxValue;
        int closestIndex = 0;
        
        for (int i = 0; i < availableInteractables.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, availableInteractables[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        
        currentSelectionIndex = closestIndex;
    }
    
    private void UpdateHUD()
    {
        if (availableInteractables.Count == 0 || IsInteracting)
        {
            interactionHUD?.Hide();
        }
        else
        {
            interactionHUD?.ShowOptions(availableInteractables, currentSelectionIndex);
        }
    }
    
    private void HandleSelectionInput()
    {
        // 滚轮切换选择
        if (Time.time - lastScrollTime >= scrollCooldown)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            
            if (scroll != 0 && availableInteractables.Count > 1)
            {
                lastScrollTime = Time.time;
                
                if (scroll > 0) // 向上滚动
                {
                    currentSelectionIndex--;
                    if (currentSelectionIndex < 0)
                        currentSelectionIndex = availableInteractables.Count - 1;
                }
                else // 向下滚动
                {
                    currentSelectionIndex++;
                    if (currentSelectionIndex >= availableInteractables.Count)
                        currentSelectionIndex = 0;
                }
            }
        }
    }
    
    private void HandleInteractionInput()
    {
        // F键交互
        if (Keyboard.current.fKey.wasPressedThisFrame && 
            currentSelectionIndex >= 0 && 
            currentSelectionIndex < availableInteractables.Count)
        {
            StartInteraction(availableInteractables[currentSelectionIndex].interactable);
        }
    }
    
    private void StartInteraction(IIntractable interactable)
    {
        IsInteracting = true;
        currentInteractable = interactable;
        
        interactable.Interact(this, out bool success);
        
        if (success)
        {
            // 立即订阅完成事件
            currentInteractable.OnInteractionComplete += OnInteractionComplete;
            
            // 隐藏HUD（已经在UpdateHUD中处理了）
        }
        else
        {
            // 交互失败，立即重置状态
            IsInteracting = false;
            currentInteractable = null;
        }
    }
    
    private void OnInteractionComplete(IIntractable interactable)
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnInteractionComplete -= OnInteractionComplete;
        }
        
        EndInteraction();
    }
    
    // 添加一个公共方法，允许外部强制结束交互
    public void ForceEndInteraction()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnInteractionComplete -= OnInteractionComplete;
        }
        
        EndInteraction();
    }
    
    private void EndInteraction()
    {
        IsInteracting = false;
        currentInteractable = null;
        
        // 清除当前选择，强制下一次重新选择
        currentSelectionIndex = -1;
        
        // 立即更新可交互对象和HUD
        UpdateAvailableInteractables();
        
        // 如果周围有可交互对象，立即显示HUD
        if (availableInteractables.Count > 0)
        {
            SelectClosest();
            UpdateHUD();
        }
    }
    
    private void OnDisable()
    {
        interactionHUD?.Hide();
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRange);
        }
    }
    #endif
}