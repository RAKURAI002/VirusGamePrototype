using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        EventManager.Instance.OnActivityAssigned += OnActivityAssigned;
        EventManager.Instance.OnActivityFinished -= OnActivityAssigned;
    }
    protected virtual void OnDisable()
    {
        if (EventManager.Instance)
        {
            EventManager.Instance.OnActivityAssigned -= OnActivityAssigned;
            EventManager.Instance.OnActivityFinished -= OnActivityAssigned;
        }
    }
    protected virtual void OnActivityAssigned(ActivityInformation activityInformation)
    {

    }
    public virtual void UpdateSlider()
    {
    }
    protected GameObject slider;
    public GameObject Slider { get { return slider; } set { slider = value; } }
    public abstract void ForceFinish();

}
