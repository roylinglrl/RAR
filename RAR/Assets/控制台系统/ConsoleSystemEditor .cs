using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.控制台系统
{
#if UNITY_EDITOR

    [CustomEditor(typeof(ConsoleSystem))]
    public class ConsoleSystemEditor : Editor
    {
        private bool showCommands = true;
        private bool showSettings = true;
        public override void OnInspectorGUI()
        {
            ConsoleSystem system = (ConsoleSystem)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Unity 控制台系统", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 设置区域
            showSettings = EditorGUILayout.Foldout(showSettings, "系统设置", true);
            if (showSettings)
            {
                EditorGUI.indentLevel++;

                system.toggleKey = (KeyCode)EditorGUILayout.EnumPopup("切换按键", system.toggleKey);
                system.enableInBuild = EditorGUILayout.Toggle("发布版本启用", system.enableInBuild);
                system.logToUnityConsole = EditorGUILayout.Toggle("输出到Unity控制台", system.logToUnityConsole);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            // UI引用
            system.consoleUI = (ConsoleUI)EditorGUILayout.ObjectField(
                "控制台UI", system.consoleUI, typeof(ConsoleUI), true);

            // 命令列表
            showCommands = EditorGUILayout.Foldout(showCommands, "注册的命令", true);
            if (showCommands)
            {
                EditorGUI.indentLevel++;

                SerializedProperty commandsProp = serializedObject.FindProperty("registeredCommands");
                EditorGUILayout.PropertyField(commandsProp, true);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            // 操作按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("重新初始化命令"))
            {
                system.InitializeCommands();
            }

            if (GUILayout.Button("打开控制台"))
            {
                ConsoleSystem.ShowConsoleMenuItem();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
    /// <summary>
    /// 创建控制台预设的菜单项
    /// </summary>
    public class ConsoleCreator
    {
        [MenuItem("GameObject/UI/控制台系统", false, 10)]
        public static void CreateConsoleSystem()
        {
            // 创建画布（如果不存在）
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            // 创建控制台UI预制体
            GameObject consoleGO = new GameObject("Console");
            consoleGO.transform.SetParent(canvas.transform, false);

            // 添加UI组件
            RectTransform rect = consoleGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            CanvasGroup canvasGroup = consoleGO.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // 背景
            Image background = consoleGO.AddComponent<Image>();
            background.color = new Color(0, 0, 0, 0.85f);

            // 创建输出区域
            GameObject outputArea = new GameObject("OutputArea");
            outputArea.transform.SetParent(consoleGO.transform, false);

            RectTransform outputRect = outputArea.AddComponent<RectTransform>();
            outputRect.anchorMin = new Vector2(0, 0.2f);
            outputRect.anchorMax = new Vector2(1, 0.9f);
            outputRect.anchoredPosition = Vector2.zero;
            outputRect.sizeDelta = new Vector2(-20, -20);

            ScrollRect scrollRect = outputArea.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.decelerationRate = 0.135f;

            // 输出文本
            GameObject outputTextGO = new GameObject("OutputText");
            outputTextGO.transform.SetParent(outputArea.transform, false);

            RectTransform textRect = outputTextGO.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0, 1);
            textRect.anchoredPosition = new Vector2(10, -10);
            textRect.sizeDelta = new Vector2(-20, 0);

            TextMeshProUGUI outputText = outputTextGO.AddComponent<TextMeshProUGUI>();
            outputText.text = "控制台已就绪...\n输入 'help' 查看可用命令";
            outputText.color = Color.white;
            outputText.fontSize = 14;
            outputText.alignment = TextAlignmentOptions.TopLeft;
            outputText.richText = true;

            scrollRect.content = textRect;
            scrollRect.viewport = outputRect;

            // 创建输入区域
            GameObject inputArea = new GameObject("InputArea");
            inputArea.transform.SetParent(consoleGO.transform, false);

            RectTransform inputRect = inputArea.AddComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0, 0);
            inputRect.anchorMax = new Vector2(1, 0.2f);
            inputRect.anchoredPosition = Vector2.zero;
            inputRect.sizeDelta = new Vector2(-20, -10);

            HorizontalLayoutGroup layout = inputArea.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;

            // 输入框
            GameObject inputFieldGO = new GameObject("InputField");
            inputFieldGO.transform.SetParent(inputArea.transform, false);

            LayoutElement layoutElement = inputFieldGO.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1;

            Image inputBg = inputFieldGO.AddComponent<Image>();
            inputBg.color = new Color(0.1f, 0.1f, 0.1f);

            TMP_InputField inputField = inputFieldGO.AddComponent<TMP_InputField>();
            inputField.textComponent = inputFieldGO.AddComponent<TextMeshProUGUI>();
            inputField.textComponent.color = Color.white;
            inputField.textComponent.fontSize = 16;
            inputField.textComponent.alignment = TextAlignmentOptions.Left;

            // 提交按钮
            GameObject submitButtonGO = new GameObject("SubmitButton");
            submitButtonGO.transform.SetParent(inputArea.transform, false);

            Button submitButton = submitButtonGO.AddComponent<Button>();
            Image submitImage = submitButtonGO.AddComponent<Image>();
            submitImage.color = new Color(0.2f, 0.6f, 0.2f);

            TextMeshProUGUI submitText = submitButtonGO.AddComponent<TextMeshProUGUI>();
            submitText.text = "执行";
            submitText.color = Color.white;
            submitText.alignment = TextAlignmentOptions.Center;
            submitText.fontSize = 16;

            // 控制台组件
            ConsoleUI consoleUI = consoleGO.AddComponent<ConsoleUI>();
            consoleUI.canvasGroup = canvasGroup;
            consoleUI.scrollRect = scrollRect;
            consoleUI.outputText = outputText;
            consoleUI.inputField = inputField;
            consoleUI.submitButton = submitButton;
            consoleUI.closeButton = null; // 可选添加关闭按钮
            consoleUI.clearButton = null; // 可选添加清除按钮

            // 创建控制台系统（如果不存在）
            ConsoleSystem system = GameObject.FindFirstObjectByType<ConsoleSystem>();
            if (system == null)
            {
                GameObject systemGO = new GameObject("ConsoleSystem");
                system = systemGO.AddComponent<ConsoleSystem>();
                
            }

            system.consoleUI = consoleUI;

            // 选中新创建的对象
            Selection.activeGameObject = consoleGO;

            Debug.Log("控制台系统已创建完成");
        }
    }
#endif
}