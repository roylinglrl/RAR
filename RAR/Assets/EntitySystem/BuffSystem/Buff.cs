
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Buff
{
    public int id;
    public string name;
    public int duration;
    public int stacks;
}
