using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonComponent<T> : MonoBehaviour where T : Object
{
	private static T instance;

	protected bool initialized;

	protected bool secondCalled;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<T>();
				
				if (instance != null && Application.isPlaying)
				{
					(instance as SingletonComponent<T>).Initialize();
					Debug.Log("Creating " + instance.GetType().ToString() + " Instance . . .");
				}
				
			}

			if (instance == null)
			{
				Debug.LogFormat("[SingletonComponent] Returning null instance for component of type {0}.", typeof(T));
			}

			return instance;
		}
	}

    #region Unity Functions
    protected virtual void Awake()
	{
		SetInstance();
	}

	protected virtual void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
    #endregion
   
	public static bool Exists()
	{
		return instance != null;
	}

	public bool SetInstance()
	{
		if (instance != null && instance != gameObject.GetComponent<T>())
		{
			Destroy(this.gameObject);
			//Debug.LogError("[SingletonComponent] Instance already set for type " + typeof(T));
			return false;
		}



		instance = gameObject.GetComponent<T>();

		Initialize();

		return true;
	}

	protected virtual void OnInitialize()
	{
	}

	private void Initialize()
	{
		if (!initialized)
		{
			OnInitialize();
			initialized = true;
		}
	}
}
