using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    protected GameObject slider;
    public GameObject Slider { get { return slider; } set { slider = value; } }
    public abstract void ForceFinish();

}
