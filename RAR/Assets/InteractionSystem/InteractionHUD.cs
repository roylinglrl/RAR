using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionHUD : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hudContainer;  // HUD容器
    public GameObject optionPrefab;  // 选项预制体
    public Transform optionsParent;  // 选项父对象
    
    [Header("Display Settings")]
    public float optionSpacing = 60f;
    public Color normalColor = new Color(1f, 1f, 1f, 0.3f);
    public Color selectedColor = new Color(1f, 0.8f, 0f, 0.7f);
    public Color normalTextColor = Color.white;
    public Color selectedTextColor = Color.black;
    
    private List<GameObject> activeOptions = new List<GameObject>();
    private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        Hide();
    }
    
    public void ShowOptions(List<(IIntractable interactable, Transform transform)> interactables, int selectedIndex)
    {
        if (interactables.Count == 0)
        {
            Hide();
            return;
        }
        
        // 显示HUD
        if (!hudContainer.activeSelf)
            hudContainer.SetActive(true);
            
        canvasGroup.alpha = 1f;
        
        // 清除旧选项
        ClearOptions();
        
        // 创建新选项
        for (int i = 0; i < interactables.Count; i++)
        {
            var interactable = interactables[i].interactable;
            CreateOption(interactable.InteractionName, i, i == selectedIndex);
        }
    }
    
    private void CreateOption(string optionText, int index, bool isSelected)
    {
        if (optionPrefab == null || optionsParent == null)
        {
            Debug.LogError("Option prefab or parent not set!");
            return;
        }
        
        // 实例化选项
        GameObject option = Instantiate(optionPrefab, optionsParent);
        
        // 设置位置
        RectTransform rectTransform = option.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            float yPos = -index * optionSpacing;
            rectTransform.anchoredPosition = new Vector2(0, yPos);
        }
        
        // 设置文本
        TextMeshProUGUI textComponent = option.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = $"按 F {optionText}";
        }
        
        // 设置高亮
        Image highlightImage = option.GetComponent<Image>();
        if (highlightImage != null)
        {
            highlightImage.color = isSelected ? selectedColor : normalColor;
        }
        
        // 设置文本颜色
        if (textComponent != null)
        {
            textComponent.color = isSelected ? selectedTextColor : normalTextColor;
        }
        
        // 设置缩放
        if (isSelected)
        {
            option.transform.localScale = Vector3.one * 1.1f;
        }
        
        activeOptions.Add(option);
    }
    
    private void ClearOptions()
    {
        foreach (GameObject option in activeOptions)
        {
            if (option != null)
                Destroy(option);
        }
        activeOptions.Clear();
    }
    
    public void Hide()
    {
        canvasGroup.alpha = 0f;
        ClearOptions();
        if (hudContainer != null)
            hudContainer.SetActive(false);
    }
    
    private void OnDestroy()
    {
        ClearOptions();
    }
}