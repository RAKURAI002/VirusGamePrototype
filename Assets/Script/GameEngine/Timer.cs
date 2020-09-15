using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    public GameObject slider { get; set; }
    public abstract void ForceFinish();

}
