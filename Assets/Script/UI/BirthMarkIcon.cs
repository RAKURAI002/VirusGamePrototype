using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class BirthMarkIcon : Icon
{
    BirthMarkData birthMarkData;

    public override void Initialize<T>(T data, bool isWink)
    {
        birthMarkData = data as BirthMarkData;
        this.isWink = isWink;

        GetComponent<Image>().sprite = Resources.Load<Sprite>(birthMarkData.spritePath);
        SetDescription();
    }

    void SetDescription()
    {
        permanentTextBuilder.Clear();

        permanentTextBuilder.AppendLine($"{birthMarkData.name} Level{birthMarkData.level}\n");
        permanentTextBuilder.AppendLine($"{birthMarkData.description} : {birthMarkData.effectValues[birthMarkData.level - 1] *100}%");
        SetText();

    }
}
