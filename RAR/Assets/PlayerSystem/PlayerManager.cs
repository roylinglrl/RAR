using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] private CombatEntity playerCombatEntity;
    public CombatEntity PlayerCombatEntity => playerCombatEntity;
    [SerializeField] private BuffManager buffManager;
    public BuffManager BuffManager => buffManager;
    [SerializeField] private PlayerBackpackHolder playerBackpackHolder;
    public PlayerBackpackHolder PlayerBackpackHolder => playerBackpackHolder;

    public void Init(GameObject player)
    {
        playerCombatEntity = player.GetComponent<CombatEntity>();
        buffManager = player.GetComponent<BuffManager>();
        playerBackpackHolder = player.GetComponent<PlayerBackpackHolder>();
    }
}
