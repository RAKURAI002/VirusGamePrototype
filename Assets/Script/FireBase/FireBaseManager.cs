using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Linq;
using System.Threading.Tasks;

public class FireBaseManager : SingletonComponent<FireBaseManager>
{
    public DatabaseReference reference;

    protected override void Awake()
    {
        base.Awake();
    
    }

    protected override void OnInitialize()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://virus-game-project.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    void Start()
    {
    }

    public void SendData(string json)
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("users/").Child(LoadManager.Instance.playerData.UID).SetRawJsonValueAsync(json);
    }
    public DataSnapshot[] ReceiveData()
    {
        DataSnapshot snapshot = null;
        FirebaseDatabase.DefaultInstance.GetReference("users/").GetValueAsync().ContinueWith(task => { 
            if (task.IsCompleted) { 
                snapshot = task.Result;
                Debug.Log(snapshot.Children.Where(ss => ss.Key == LoadManager.Instance.playerData.UID).ToList()[0].GetRawJsonValue()); 
            } });

        return snapshot.Children.Where(ss => ss.Key == LoadManager.Instance.playerData.UID).ToArray() ;//JsonHelper.ToJson(snapshot.Children.Where(ss => ss.Key == PlayerData.UID).ToArray());
    }
    IEnumerator ReceiveDataCoroutine()
    {
        Task<DataSnapshot> task = FirebaseDatabase.DefaultInstance.GetReference("users/").GetValueAsync();
        yield return null;
        DataSnapshot snapshot = task.Result;
        
    }
}
