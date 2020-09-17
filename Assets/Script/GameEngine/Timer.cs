using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Timer : MonoBehaviour
{
    public ActivityInformation activityInformation;
    public bool isFinished;

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        EventManager.Instance.OnActivityAssigned += OnActivityAssigned;
        EventManager.Instance.OnActivityFinished -= OnActivityAssigned;
    }
    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        if (EventManager.Instance)
        {
            EventManager.Instance.OnActivityAssigned -= OnActivityAssigned;
            EventManager.Instance.OnActivityFinished -= OnActivityAssigned;
        }
    }
    protected virtual void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
    }
    protected virtual void OnActivityAssigned(ActivityInformation activityInformation)
    {

    }
    public virtual void InitializeSlider()
    {
    }
    protected GameObject slider = null;
    public GameObject Slider { get { return slider; } set { slider = value; } }
    public abstract void ForceFinish();

}
