using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System.Reflection;

public class BuffIcon : Icon
{
    Resource.Effect effect;

    int _duration;
    private void Update()
    {
        int duration = CharacterManager.Instance.gameObject.transform.Find("ItemEffectTimer/" + effect.instanceID).GetComponent<ItemEffectTimer>().timeLeft;
       
        if(duration != _duration)
        {
            updateTextBuilder.Append($"<color=blue>{GetFormattedDuration(duration)}</color>");
            _duration = duration;
            UpdateText();
        }
        
    }

    string GetFormattedDuration(int duration)
    {
        int hours = Mathf.FloorToInt(duration / 3600);
        int minutes = Mathf.FloorToInt(duration % 3600 / 60);
        int seconds = Mathf.FloorToInt(duration % 3600 % 60f);

        return $"{(hours > 0 ? hours.ToString() + "H : " : "")}{((hours < 2 && minutes > 0) ? (minutes).ToString() + " M" : "")}{((hours == 0 && minutes == 0) ? " : " + seconds.ToString() + " S" : "")}";

    }


    protected void SetDescription()
    {
        permanentTextBuilder.AppendLine(effect.name);
        List<FieldInfo> fInfos = effect.stats.GetType().GetFields().Where(es => (int)es.GetValue(effect.stats) > 0).ToList();

        foreach (FieldInfo fInfo in fInfos)
        {
            permanentTextBuilder.AppendLine($"+ {fInfo.Name} : {fInfo.GetValue(effect.stats)}");

        }

        Debug.Log($"{permanentTextBuilder}");
        SetText();
    }

    public override void Initialize<T>(T data, bool isWink)
    {
        effect = data as Resource.Effect;
        this.isWink = isWink;
        Debug.Log($"{effect.spritePath}");
        GetComponent<Image>().sprite = Resources.Load<Sprite>(effect.spritePath);
        SetDescription();
    }
}
