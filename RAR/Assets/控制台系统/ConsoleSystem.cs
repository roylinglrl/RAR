using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class ConsoleEvent : UnityEvent<string[]> { }

/// <summary>
/// Unity控制台系统，提供游戏内命令行界面功能
/// 支持内置命令、自定义命令注册、历史记录、自动补全等功能
/// </summary>
public class ConsoleSystem : MonoBehaviour
{
    [Header("控制台设置")]
    [SerializeField] public KeyCode toggleKey = KeyCode.BackQuote; // ` 键
    [SerializeField] public bool enableInBuild = false; // 构建版本中是否启用
    [SerializeField] public bool logToUnityConsole = true; // 是否同时输出到Unity控制台

    [Header("UI 设置")]
    [SerializeField] public ConsoleUI consoleUI;

    [Header("命令配置")]
    [SerializeField] public List<ConsoleCommand> registeredCommands = new List<ConsoleCommand>();

    public Dictionary<string, ConsoleCommand> commandDictionary = new Dictionary<string, ConsoleCommand>();
    public List<string> commandHistory = new List<string>();
    public int historyIndex = -1;

    public static ConsoleSystem instance;

    /// <summary>
    /// 获取控制台系统实例，如果不存在则创建新的实例
    /// </summary>
    public static ConsoleSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ConsoleSystem>();
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

    /// <summary>
    /// 初始化控制台系统，处理单例模式和构建版本检查
    /// </summary>
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // 检查是否在构建版本中启用
        if (!enableInBuild && !Debug.isDebugBuild)
        {
            gameObject.SetActive(false);
            return;
        }

        InitializeCommands();
    }

    /// <summary>
    /// 更新控制台状态，处理开关按键和输入事件
    /// </summary>
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

    /// <summary>
    /// 初始化所有命令，包括内置命令、注册命令和属性标记命令
    /// </summary>
    public void InitializeCommands()
    {
        // 清空现有命令字典
        commandDictionary.Clear();

        // 注册内置命令
        RegisterBuiltInCommands();

        // 注册用户定义的命令
        foreach (var cmd in registeredCommands)
        {
            RegisterCommand(cmd);
        }

        // 自动注册标记了[ConsoleCommand]特性的静态方法
        AutoRegisterAttributeCommands();

        Log($"控制台初始化完成，共加载命令: {commandDictionary.Count}");
    }

    /// <summary>
    /// 注册内置的基础命令，如help、clear、exit等
    /// </summary>
    private void RegisterBuiltInCommands()
    {
        // 帮助命令
        RegisterCommand(new ConsoleCommand
        {
            command = "help",
            description = "显示所有可用命令或指定命令的帮助信息",
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
            description = "清空控制台输出",
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
        // 添加金币
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
                    // 这里应该根据实际游戏逻辑调整
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

    /// <summary>
    /// 自动注册标记了ConsoleCommand特性的静态方法
    /// </summary>
    private void AutoRegisterAttributeCommands()
    {
        // 遍历所有程序集中的标记方法
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

    /// <summary>
    /// 注册带有特性标记的方法为控制台命令
    /// </summary>
    /// <param name="method">要注册的方法</param>
    /// <param name="attr">方法的控制台命令特性</param>
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

    /// <summary>
    /// 注册一个新的控制台命令
    /// </summary>
    /// <param name="command">要注册的命令对象</param>
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

    /// <summary>
    /// 取消注册指定名称的命令
    /// </summary>
    /// <param name="commandName">要取消注册的命令名称</param>
    public void UnregisterCommand(string commandName)
    {
        if (commandDictionary.ContainsKey(commandName.ToLower()))
        {
            commandDictionary.Remove(commandName.ToLower());
        }
    }

    /// <summary>
    /// 处理控制台输入事件，包括历史记录导航和自动补全
    /// </summary>
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

    /// <summary>
    /// 导航命令历史记录
    /// </summary>
    /// <param name="direction">导航方向，-1为向上，1为向下</param>
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

    /// <summary>
    /// 执行命令自动补全功能
    /// </summary>
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

    /// <summary>
    /// 执行输入的命令字符串
    /// </summary>
    /// <param name="input">完整的命令输入字符串</param>
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

        // 记录命令
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
                LogError($"命令执行异常: {e.Message}");
            }
        }
        else
        {
            LogError($"未知命令: {commandName}");
        }
    }

    /// <summary>
    /// 输出普通日志消息
    /// </summary>
    /// <param name="message">要输出的消息</param>
    public void Log(string message)
    {
        consoleUI?.AddOutputLine(message);

        if (logToUnityConsole)
        {
            Debug.Log($"[Console] {message}");
        }
    }

    /// <summary>
    /// 输出警告日志消息
    /// </summary>
    /// <param name="message">要输出的警告消息</param>
    public void LogWarning(string message)
    {
        consoleUI?.AddOutputLine($"<color=yellow>[警告] {message}</color>");

        if (logToUnityConsole)
        {
            Debug.LogWarning($"[Console] {message}");
        }
    }

    /// <summary>
    /// 输出错误日志消息
    /// </summary>
    /// <param name="message">要输出的错误消息</param>
    public void LogError(string message)
    {
        consoleUI?.AddOutputLine($"<color=red>[错误] {message}</color>");

        if (logToUnityConsole)
        {
            Debug.LogError($"[Console] {message}");
        }
    }

    /// <summary>
    /// 显示所有可用命令列表
    /// </summary>
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

    /// <summary>
    /// 显示指定命令的帮助信息
    /// </summary>
    /// <param name="commandName">要显示帮助的命令名称</param>
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

    /// <summary>
    /// 切换控制台显示/隐藏状态
    /// </summary>
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

    /// <summary>
    /// 显示控制台界面
    /// </summary>
    public void ShowConsole()
    {
        consoleUI?.Show();
        consoleUI?.FocusInputField();
    }

    /// <summary>
    /// 隐藏控制台界面
    /// </summary>
    public void HideConsole()
    {
        consoleUI?.Hide();
    }

    // 编辑器菜单项
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

/// <summary>
/// 控制台命令数据结构，包含命令名称、描述、用法和执行动作
/// </summary>
[System.Serializable]
public class ConsoleCommand
{
    public string command;
    public string description;
    public string usage;
    public Action<string[]> action;
}

/// <summary>
/// 控制台命令特性，用于标记可以作为控制台命令调用的静态方法
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