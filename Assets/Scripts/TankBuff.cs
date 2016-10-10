using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TankBuff : MonoBehaviour
{

    List<Buff> buffList = new List<Buff>();

    private TankMoveController tankMove;
    private TankShoot tankShoot;

    void Start()
    {
        tankMove = GetComponent<TankMoveController>();
        tankShoot = GetComponent<TankShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].currentTime >= buffList[i].totleTime)
            {
                switch (buffList[i].type)
                {
                    case MagicBoxBase.BoxType.moveSpeed:
                        tankMove.moveSpeed /= 2;
                        tankMove.rotateSpeed /= 2;
                        break;
                    case MagicBoxBase.BoxType.radar:
                        CameraController._instance.wholeView = false;
                        break;
                    case MagicBoxBase.BoxType.shootSpeed:
                        tankShoot.intervalTime *= 3;
                        break;
                }
                buffList.RemoveAt(i);
            }
            else
            {
                buffList[i].currentTime += Time.deltaTime;
            }
        }
    }

    public void AddBuff(int buffTypeIndex)
    {
        Buff buff = new Buff(buffTypeIndex);
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].type == buff.type)
            {
                buffList[i].currentTime -= 10;
                return;
            }
        }
        buffList.Add(buff);
    }
}
