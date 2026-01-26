using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _instance;
    public static SceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject sceneManagerObject = new GameObject("SceneManager");
                _instance = sceneManagerObject.AddComponent<SceneManager>();
                DontDestroyOnLoad(sceneManagerObject);

            }
            return _instance;
        }
    }

    public event Action<float> OnLoadProgressChanged;
    public event Action OnLoadComplete;
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="mode">加载模式</param>
    public void LoadSceneAsync(string sceneName,LoadSceneMode mode = LoadSceneMode.Single)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName,mode));
    }
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName,LoadSceneMode mode)
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName,mode);
        asyncOperation.allowSceneActivation = false;
        while(asyncOperation.progress < 0.9f)
        {
            OnLoadProgressChanged?.Invoke(asyncOperation.progress);
            yield return null;
        }
        OnLoadProgressChanged?.Invoke(1.0f);
        asyncOperation.allowSceneActivation = true;
        while(!asyncOperation.isDone)
        {
            yield return null;
        }
        OnLoadComplete?.Invoke();
    }
        /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="mode">加载模式</param>
    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, mode);
        OnLoadComplete?.Invoke();
    }
    /// <summary>
    /// 切换场景（加载新场景并卸载当前场景）
    /// </summary>
    /// <param name="sceneName">新场景名称</param>
    public void SwitchScene(string sceneName)
    {
        LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
        /// <summary>
    /// 添加场景（加载新场景但不卸载当前场景）
    /// </summary>
    /// <param name="sceneName">新场景名称</param>
    public void AddScene(string sceneName)
    {
        LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
        /// <summary>
    /// 卸载场景
    /// </summary>
    /// <param name="sceneName">要卸载的场景名称</param>
    public void UnloadScene(string sceneName)
    {
        StartCoroutine(UnloadSceneCoroutine(sceneName));
    }
        private System.Collections.IEnumerator UnloadSceneCoroutine(string sceneName)
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
        // 强制垃圾回收
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
        /// <summary>
    /// 获取当前活动场景名称
    /// </summary>
    /// <returns>当前活动场景名称</returns>
    public string GetCurrentSceneName()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentSceneName = GetCurrentSceneName();
        LoadSceneAsync(currentSceneName, LoadSceneMode.Single);
    }
}
