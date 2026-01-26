using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pressAnyKeyPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private Transform logoTransform;

    [Header("Parallax Settings")]
    [SerializeField] private float parallaxIntensity = 0.05f; // 微动强度
    [SerializeField] private float smoothSpeed = 5f; // 平滑移动速度
    
    [Header("Button Settings")]
    [SerializeField] private float buttonHoverScale = 1.1f; // 按钮悬停放大
    [SerializeField] private float buttonHoverSpeed = 8f; // 悬停动画速度

        private Vector2 initialMousePos;
    private Vector2 targetParallaxPos;
    private Vector2 currentParallaxPos;
    private Vector2[] buttonInitialPositions;
    private RectTransform[] buttonTransforms;
    private bool menuActive = false;
    void Start()
    {
        // 初始化状态
        pressAnyKeyPanel.SetActive(true);
        menuPanel.SetActive(false);
        
        // 获取所有按钮的初始位置
        InitializeButtons();
        
        // 设置初始鼠标位置
        initialMousePos = Input.mousePosition;
    }
    void InitializeButtons()
    {
        buttonTransforms = buttonsPanel.GetComponentsInChildren<RectTransform>();
        buttonInitialPositions = new Vector2[buttonTransforms.Length];
        
        for (int i = 0; i < buttonTransforms.Length; i++)
        {
            buttonInitialPositions[i] = buttonTransforms[i].anchoredPosition;
        }
    }
    void Update()
    {
        if (!menuActive)
        {
            // 检测任意按键输入
            if (Input.anyKeyDown && !Input.GetMouseButton(0))
            {
                ShowMainMenu();
            }
        }
        else
        {
            // 计算鼠标偏移量（基于屏幕中心）
            Vector2 mousePos = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 mouseOffset = (mousePos - screenCenter) / screenCenter;
            
            // 计算目标位置（反向移动，创造高级感）
            targetParallaxPos = -mouseOffset * parallaxIntensity;
            
            // 平滑移动到目标位置
            currentParallaxPos = Vector2.Lerp(
                currentParallaxPos, 
                targetParallaxPos, 
                Time.unscaledDeltaTime * smoothSpeed
            );
            
            // 应用视差效果到菜单背景（不包括LOGO）
            ApplyParallaxEffect();
            
            // 更新按钮悬停效果
            UpdateButtonEffects();
        }
    }
    void ApplyParallaxEffect()
    {
        // 菜单面板微动
        if (menuPanel.GetComponent<RectTransform>() != null)
        {
            menuPanel.GetComponent<RectTransform>().anchoredPosition = 
                currentParallaxPos * 100f; // 适当缩放
        }
        
        // 按钮独立微动（轻微不同步，创造深度感）
        for (int i = 0; i < buttonTransforms.Length; i++)
        {
            // 每个按钮有轻微不同的偏移
            float depthFactor = 1f + (i * 0.05f);
            Vector2 buttonOffset = currentParallaxPos * depthFactor * 80f;
            buttonTransforms[i].anchoredPosition = buttonInitialPositions[i] + buttonOffset;
        }
    }
    void UpdateButtonEffects()
    {
        // 这里可以添加按钮悬停的额外效果
        // 例如：根据鼠标位置轻微旋转按钮
    }
    
    void ShowMainMenu()
    {
        menuActive = true;
        
        // 添加简单的进入动画
        StartCoroutine(ShowMenuAnimation());
    }
    
    System.Collections.IEnumerator ShowMenuAnimation()
    {
        // 淡出"按下任意键"提示
        CanvasGroup pressGroup = pressAnyKeyPanel.GetComponent<CanvasGroup>();
        if (pressGroup == null) pressGroup = pressAnyKeyPanel.AddComponent<CanvasGroup>();
        
        float fadeTime = 0.5f;
        float timer = 0f;
        
        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            pressGroup.alpha = 1f - (timer / fadeTime);
            yield return null;
        }
        
        pressAnyKeyPanel.SetActive(false);
        
        // 显示菜单并播放进入动画
        menuPanel.SetActive(true);
        CanvasGroup menuGroup = menuPanel.GetComponent<CanvasGroup>();
        if (menuGroup == null) menuGroup = menuPanel.AddComponent<CanvasGroup>();
        menuGroup.alpha = 0f;
        
        // 从左侧滑入按钮
        RectTransform buttonsRect = buttonsPanel.GetComponent<RectTransform>();
        Vector2 originalPos = buttonsRect.anchoredPosition;
        Vector2 startPos = originalPos + new Vector2(-200f, 0f);
        
        timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / fadeTime;
            
            // 淡入
            menuGroup.alpha = t;
            
            // 滑入（弹性效果）
            float elasticT = 1f - Mathf.Pow(1f - t, 3f); // 缓出
            buttonsRect.anchoredPosition = Vector2.Lerp(startPos, originalPos, elasticT);
            
            yield return null;
        }
        
        menuGroup.alpha = 1f;
        buttonsRect.anchoredPosition = originalPos;
    }
    
    // 按钮悬停效果增强
    public void OnButtonHoverEnter(RectTransform button)
    {
        StartCoroutine(ScaleButton(button, buttonHoverScale));
    }
    
    public void OnButtonHoverExit(RectTransform button)
    {
        StartCoroutine(ScaleButton(button, 1f));
    }
    
    System.Collections.IEnumerator ScaleButton(RectTransform button, float targetScale)
    {
        Vector3 startScale = button.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float timer = 0f;
        
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * buttonHoverSpeed;
            button.localScale = Vector3.Lerp(startScale, endScale, Mathf.SmoothStep(0f, 1f, timer));
            yield return null;
        }
    }
}
