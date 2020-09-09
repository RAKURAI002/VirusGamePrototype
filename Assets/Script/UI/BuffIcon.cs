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
            transform.Find("BuffInformationPanel/BuffDuration").GetComponent<Text>().text = ($"<color=blue>{GetFormattedDuration(duration)}</color>");
            _duration = duration;
        }
        
    }

    public void StartBuffIcon(Resource.Effect _effect)
    {
        effect = _effect;
    }

    public new void OnPointerEnter(PointerEventData eventData)
    {
        Text buffInformationText = transform.Find("BuffInformationPanel/BuffInformationText").GetComponent<Text>();
        buffInformationText.transform.parent.gameObject.SetActive(true);

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(effect.name);
        List<FieldInfo> fInfos = effect.stats.GetType().GetFields().Where(es => (int)es.GetValue(effect.stats) > 0).ToList();

        foreach(FieldInfo fInfo in fInfos)
        {
            stringBuilder.AppendLine($"+ {fInfo.Name} : {fInfo.GetValue(effect.stats)}");
        }

        int duration = CharacterManager.Instance.gameObject.transform.Find("ItemEffectTimer/" + effect.instanceID).GetComponent<ItemEffectTimer>().timeLeft;

        


        transform.Find("BuffInformationPanel/BuffDuration").GetComponent<Text>().text = ($"<color=blue>{GetFormattedDuration(duration)}</color>");
        buffInformationText.text = stringBuilder.ToString();

        buffInformationText.transform.parent.GetComponent<RectTransform>().sizeDelta =
            new Vector2(buffInformationText.preferredWidth, buffInformationText.preferredHeight + 15) + new Vector2(5, 5);

    }
    string GetFormattedDuration(int duration)
    {
        int hours = Mathf.FloorToInt(duration / 3600);
        int minutes = Mathf.FloorToInt(duration % 3600 / 60);
        int seconds = Mathf.FloorToInt(duration % 3600 % 60f);

        return $"{(hours > 0 ? hours.ToString() + "H : " : "")}{((hours < 2 && minutes > 0) ? (minutes).ToString() + " M" : "")}{((hours == 0 && minutes == 0) ? " : " + seconds.ToString() + " S" : "")}";

    }

    public new void OnPointerExit(PointerEventData eventData)
    {
        transform.Find("BuffInformationPanel").gameObject.SetActive(false);

    }

    public override void AddDescription()
    {
        throw new System.NotImplementedException();
    }
}
