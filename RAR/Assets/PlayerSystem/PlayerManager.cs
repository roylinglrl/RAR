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
}
