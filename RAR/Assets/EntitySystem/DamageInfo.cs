using UnityEngine;

public class DamageInfo
{
    public GameObject sourceEntity;
    public GameObject targetEntity;
    public float RawDamage;
    public float FinalDamage;
    public bool IsCriticalHit;

    public bool IsCanceled;

    //TODO: Add more damage info
}
