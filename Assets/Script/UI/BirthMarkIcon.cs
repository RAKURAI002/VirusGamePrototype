using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


public class BirthMarkIcon : Icon
{
    BirthMarkData birthMarkData;

    public void InitializeIconData(BirthMarkData _birthMarkData)
    {
        birthMarkData = _birthMarkData;

    }

    protected override void SetDescription()
    {
        desciption.Clear();
      //  desciption.AppendLine($"{birthMarkData.name}(Level{birthMarkData.level})");
        desciption.AppendLine($"dd");

        Debug.Log($"{desciption}");
    }
}
