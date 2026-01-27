using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]private GameObject playerPrefab;

    [SerializeField]private bool isGameStart;
    [SerializeField]private bool isOnUI;

    public bool IsGameStart
    {
        get { return isGameStart; }
        set { isGameStart = value; }
    }
    public bool IsOnUI
    {
        get { return isOnUI; }
        set { isOnUI = value; }
    }
    
    private void Awake()
    {
        Instance = this;
    }
    public void GameStartWithSave(int saveIndex)
    {
        //读取存档 位置
        //获取存档ID是否存在存档 如果不存在则无法开始
        //读取存档数据
        //加载基地场景
        //生成玩家
        //加载玩家的数据
    }

    public void putPlayerPrefab(Vector3 position)
    {
        Instantiate(playerPrefab, position, Quaternion.identity);
    }
}
