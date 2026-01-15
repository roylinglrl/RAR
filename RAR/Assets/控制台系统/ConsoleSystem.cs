using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class ConsoleEvent : UnityEvent<string[]> { }

/// <summary>
/// Unity 控制台系统主控制器
/// </summary>
public class ConsoleSystem : MonoBehaviour
{
    [Header("控制台设置")]
    [SerializeField] public KeyCode toggleKey = KeyCode.BackQuote; // ` 键
    [SerializeField] public bool enableInBuild = false; // 发布版本是否启用
    [SerializeField] public bool logToUnityConsole = true; // 是否同时输出到Unity控制

    [Header("UI 引用")]
    [SerializeField] public ConsoleUI consoleUI;

    [Header("命令设置")]
    [SerializeField] public List<ConsoleCommand> registeredCommands = new List<ConsoleCommand>();

    public Dictionary<string, ConsoleCommand> commandDictionary = new Dictionary<string, ConsoleCommand>();
    public List<string> commandHistory = new List<string>();
    public int historyIndex = -1;

    public static ConsoleSystem instance;
    public static ConsoleSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ConsoleSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ConsoleSystem");
                    instance = go.AddComponent<ConsoleSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // 检查是否在发布版本中启用
        if (!enableInBuild && !Debug.isDebugBuild)
        {
            gameObject.SetActive(false);
            return;
        }

        InitializeCommands();
    }
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleConsole();
        }

        if (consoleUI != null && consoleUI.IsVisible)
        {
            HandleConsoleInput();
        }
    }
    public void InitializeCommands()
    {
        // 清除现有命令
        commandDictionary.Clear();

        // 注册内置命令
        RegisterBuiltInCommands();

        // 注册手动添加的命令
        foreach (var cmd in registeredCommands)
        {
            RegisterCommand(cmd);
        }

        // 自动查找并注册所有标记了 [ConsoleCommand] 特性的方法
        AutoRegisterAttributeCommands();

        Log($"控制台已初始化，可用命令数: {commandDictionary.Count}");
    }

    private void RegisterBuiltInCommands()
    {
        // 帮助命令
        RegisterCommand(new ConsoleCommand
        {
            command = "help",
            description = "显示所有命令或指定命令的帮助",
            usage = "help [command]",
            action = (args) =>
            {
                if (args.Length > 0)
                {
                    ShowCommandHelp(args[0]);
                }
                else
                {
                    ShowAllCommands();
                }
            }
        });
        // 清屏命令
        RegisterCommand(new ConsoleCommand
        {
            command = "clear",
            description = "清除控制台输出",
            action = (args) => consoleUI?.ClearOutput()
        });
        // 退出命令
        RegisterCommand(new ConsoleCommand
        {
            command = "exit",
            description = "关闭控制台",
            action = (args) => HideConsole()
        });
        // 系统信息命令
        RegisterCommand(new ConsoleCommand
        {
            command = "sysinfo",
            description = "显示系统信息",
            action = (args) =>
            {
                Log($"Unity版本: {Application.unityVersion}");
                Log($"平台: {Application.platform}");
                Log($"内存使用: {System.GC.GetTotalMemory(false) / 1024 / 1024} MB");
                Log($"帧率: {1.0f / Time.deltaTime:F1}");
                Log($"游戏时间: {Time.time:F1}秒");
            }
        });
        // 时间缩放命令
        RegisterCommand(new ConsoleCommand
        {
            command = "timescale",
            description = "设置游戏时间缩放",
            usage = "timescale <value>",
            action = (args) =>
            {
                if (args.Length == 0)
                {
                    Log($"当前时间缩放: {Time.timeScale}");
                    return;
                }
                if (float.TryParse(args[0], out float scale))
                {
                    Time.timeScale = Mathf.Clamp(scale, 0f, 100f);
                    Log($"时间缩放已设置为: {Time.timeScale}");
                }
                else
                {
                    LogError("无效的时间缩放值");
                }
            }
        });
        // 无敌模式命令
        RegisterCommand(new ConsoleCommand
        {
            command = "godmode",
            description = "切换无敌模式",
            action = (args) =>
            {
                bool godMode = PlayerPrefs.GetInt("GodMode", 0) == 1;
                godMode = !godMode;
                PlayerPrefs.SetInt("GodMode", godMode ? 1 : 0);
                Log($"无敌模式: {(godMode ? "开启" : "关闭")}");
            }
        });
        // 金币命令
        RegisterCommand(new ConsoleCommand
        {
            command = "addgold",
            description = "添加金币",
            usage = "addgold <amount>",
            action = (args) =>
            {
                if (args.Length == 0)
                {
                    LogError("需要指定金币数量");
                    return;
                }
                if (int.TryParse(args[0], out int amount))
                {
                    // 这里可以根据你的游戏逻辑修改
                    int currentGold = PlayerPrefs.GetInt("Gold", 0);
                    currentGold += amount;
                    PlayerPrefs.SetInt("Gold", currentGold);
                    Log($"金币已添加，当前金币: {currentGold}");
                }
                else
                {
                    LogError("无效的金币数量");
                }
            }
        });
    }
    private void AutoRegisterAttributeCommands()
    {
        // 查找所有程序集中标记了 ConsoleCommand 特性的方法
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        var attr = method.GetCustomAttribute<ConsoleCommandAttribute>();
                        if (attr != null)
                        {
                            RegisterAttributeCommand(method, attr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"自动注册命令时出错: {e.Message}");
            }
        }
    }

    private void RegisterAttributeCommand(MethodInfo method, ConsoleCommandAttribute attr)
    {
        RegisterCommand(new ConsoleCommand
        {
            command = attr.CommandName,
            description = attr.Description,
            usage = attr.Usage,
            action = (args) =>
            {
                try
                {
                    var parameters = method.GetParameters();
                    object[] methodArgs = new object[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (i < args.Length)
                        {
                            // 尝试转换参数类型
                            methodArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
                        }
                        else
                        {
                            // 使用默认值
                            methodArgs[i] = parameters[i].DefaultValue;
                        }
                    }

                    method.Invoke(null, methodArgs);
                }
                catch (Exception e)
                {
                    LogError($"执行命令失败: {e.Message}");
                }
            }
        });
    }
    public void RegisterCommand(ConsoleCommand command)
    {
        if (commandDictionary.ContainsKey(command.command))
        {
            Debug.LogWarning($"命令 '{command.command}' 已存在，将被覆盖");
        }

        commandDictionary[command.command.ToLower()] = command;

        if (consoleUI != null)
        {
            consoleUI.UpdateAutoComplete(commandDictionary.Keys.ToList());
        }
    }
    public void UnregisterCommand(string commandName)
    {
        if (commandDictionary.ContainsKey(commandName.ToLower()))
        {
            commandDictionary.Remove(commandName.ToLower());
        }
    }

    private void HandleConsoleInput()
    {
        // 历史记录导航
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            NavigateHistory(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            NavigateHistory(1);
        }

        // 自动补全
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AutoComplete();
        }
    }

    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0) return;

        historyIndex = Mathf.Clamp(historyIndex + direction, -1, commandHistory.Count - 1);

        if (historyIndex >= 0 && historyIndex < commandHistory.Count)
        {
            consoleUI?.SetInputText(commandHistory[historyIndex]);
        }
        else if (historyIndex == -1)
        {
            consoleUI?.SetInputText("");
        }
    }

    private void AutoComplete()
    {
        string input = consoleUI?.GetInputText() ?? "";
        if (string.IsNullOrWhiteSpace(input)) return;

        var matches = commandDictionary.Keys
            .Where(cmd => cmd.StartsWith(input.ToLower()))
            .ToList();

        if (matches.Count == 1)
        {
            consoleUI?.SetInputText(matches[0] + " ");
        }
        else if (matches.Count > 1)
        {
            Log($"可能的命令: {string.Join(", ", matches)}");
        }
    }

    public void ExecuteCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        // 添加到历史记录
        commandHistory.Add(input);
        historyIndex = commandHistory.Count;

        // 解析命令
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        string commandName = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts.Skip(1).ToArray() : new string[0];

        // 记录输入
        consoleUI?.AddOutputLine($"> {input}");

        // 执行命令
        if (commandDictionary.TryGetValue(commandName, out ConsoleCommand command))
        {
            try
            {
                command.action?.Invoke(args);
            }
            catch (Exception e)
            {
                LogError($"命令执行错误: {e.Message}");
            }
        }
        else
        {
            LogError($"未知命令: {commandName}");
        }
    }

    public void Log(string message)
    {
        consoleUI?.AddOutputLine(message);

        if (logToUnityConsole)
        {
            Debug.Log($"[Console] {message}");
        }
    }

    public void LogWarning(string message)
    {
        consoleUI?.AddOutputLine($"<color=yellow>[警告] {message}</color>");

        if (logToUnityConsole)
        {
            Debug.LogWarning($"[Console] {message}");
        }
    }

    public void LogError(string message)
    {
        consoleUI?.AddOutputLine($"<color=red>[错误] {message}</color>");

        if (logToUnityConsole)
        {
            Debug.LogError($"[Console] {message}");
        }
    }

    private void ShowAllCommands()
    {
        Log("=== 可用命令 ===");
        foreach (var cmd in commandDictionary.Values.OrderBy(c => c.command))
        {
            Log($"<color=#00FF00>{cmd.command}</color> - {cmd.description}");
            if (!string.IsNullOrEmpty(cmd.usage))
            {
                Log($"    用法: {cmd.usage}");
            }
        }
    }

    private void ShowCommandHelp(string commandName)
    {
        if (commandDictionary.TryGetValue(commandName.ToLower(), out ConsoleCommand command))
        {
            Log($"=== {command.command} ===");
            Log($"描述: {command.description}");
            if (!string.IsNullOrEmpty(command.usage))
            {
                Log($"用法: {command.usage}");
            }
        }
        else
        {
            LogError($"命令 '{commandName}' 不存在");
        }
    }

    public void ToggleConsole()
    {
        if (consoleUI == null) return;

        if (consoleUI.IsVisible)
        {
            HideConsole();
        }
        else
        {
            ShowConsole();
        }
    }

    public void ShowConsole()
    {
        consoleUI?.Show();
        consoleUI?.FocusInputField();
    }

    public void HideConsole()
    {
        consoleUI?.Hide();
    }

    // 编辑器调试用
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Console/Show Console")]
    public static void ShowConsoleMenuItem()
    {
        Instance.ShowConsole();
    }

    [UnityEditor.MenuItem("Tools/Console/Hide Console")]
    public static void HideConsoleMenuItem()
    {
        Instance.HideConsole();
    }
#endif
}
[System.Serializable]
public class ConsoleCommand
{
    public string command;
    public string description;
    public string usage;
    public Action<string[]> action;
}

/// <summary>
/// 命令特性，用于标记静态方法为控制台命令
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : System.Attribute
{
    public string CommandName { get; }
    public string Description { get; }
    public string Usage { get; }

    public ConsoleCommandAttribute(string commandName, string description = "", string usage = "")
    {
        CommandName = commandName;
        Description = description;
        Usage = usage;
    }
}
