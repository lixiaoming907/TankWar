using UnityEngine;
using System.Collections;

public class Buff
{

    public float totleTime = 10;
    public float currentTime = 0;

    public MagicBoxBase.BoxType type = MagicBoxBase.BoxType.none;

    public Buff(int typeIndex)
    {
        switch (typeIndex)
        {
            case 1:
                type = MagicBoxBase.BoxType.moveSpeed;
                break;
            case 2:
                type = MagicBoxBase.BoxType.shootSpeed;
                break;
            case 3:
                type = MagicBoxBase.BoxType.radar;
                break;
        }
    }
}
