using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

    /// <summary>
    /// 控制台UI界面
    /// </summary>
    public class ConsoleUI : MonoBehaviour
    {
        [Header("UI 组件")]
        [SerializeField] public CanvasGroup canvasGroup;
        [SerializeField] public ScrollRect scrollRect;
        [SerializeField] public TMP_Text outputText;
        [SerializeField] public TMP_InputField inputField;
        [SerializeField] public Button submitButton;
        [SerializeField] public Button closeButton;
        [SerializeField] public Button clearButton;

        [Header("显示设置")]
        [SerializeField] public int maxOutputLines = 100;
        [SerializeField] public Color timestampColor = new Color(0.5f, 0.5f, 0.5f);
        [SerializeField] public Color commandColor = new Color(0f, 1f, 1f);

        public List<string> outputLines = new List<string>();
        public List<string> autoCompleteOptions = new List<string>();

        public bool IsVisible => canvasGroup.alpha > 0;

        void Start()
        {
            // 绑定事件
            inputField.onSubmit.AddListener(OnSubmit);
            submitButton.onClick.AddListener(() => OnSubmit(inputField.text));
            closeButton.onClick.AddListener(() => ConsoleSystem.Instance.HideConsole());
            clearButton.onClick.AddListener(ClearOutput);

            // 初始隐藏
            Hide();
        }
        public void Show()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            FocusInputField();
        }
        public void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        public void FocusInputField()
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
        public void AddOutputLine(string line)
        {
            string timestamp = $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(timestampColor)}>[{System.DateTime.Now:HH:mm:ss}]</color> ";
            string formattedLine = timestamp + line;

            outputLines.Add(formattedLine);

            // 限制最大行数
            if (outputLines.Count > maxOutputLines)
            {
                outputLines.RemoveAt(0);
            }

            UpdateOutputDisplay();

            // 滚动到底部
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0;
        }

        public void ClearOutput()
        {
            outputLines.Clear();
            UpdateOutputDisplay();
        }

        private void UpdateOutputDisplay()
        {
            outputText.text = string.Join("\n", outputLines);
        }

        private void OnSubmit(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            ConsoleSystem.Instance.ExecuteCommand(input);
            inputField.text = "";
            FocusInputField();
        }

        public string GetInputText()
        {
            return inputField.text;
        }

        public void SetInputText(string text)
        {
            inputField.text = text;
            inputField.caretPosition = text.Length;
            FocusInputField();
        }

        public void UpdateAutoComplete(List<string> commands)
        {
            autoCompleteOptions = commands;
        }

        // 提供一些实用的UI方法
        public void AddSeparator()
        {
            AddOutputLine("<color=gray>--------------------------------</color>");
        }

        public void AddSuccessMessage(string message)
        {
            AddOutputLine($"<color=green>✓ {message}</color>");
        }

        public void AddErrorMessage(string message)
        {
            AddOutputLine($"<color=red>✗ {message}</color>");
        }

    }
