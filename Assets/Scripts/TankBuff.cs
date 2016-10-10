using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TankBuff : NetworkBehaviour
{

    List<Buff> buffList = new List<Buff>();

    private TankMoveController tankMove;
    private TankShoot tankShoot;

    private Image fastRun;
    private Image radar;
    private Image shootSpeed;

    void Start()
    {
        tankMove = GetComponent<TankMoveController>();
        tankShoot = GetComponent<TankShoot>();

        if (isLocalPlayer)
        {
            fastRun = GameObject.Find("Canvas/FastRun").GetComponent<Image>();
            radar = GameObject.Find("Canvas/Radar").GetComponent<Image>();
            shootSpeed = GameObject.Find("Canvas/ShootSpeed").GetComponent<Image>();

            fastRun.fillAmount = 0;
            radar.fillAmount = 0;
            shootSpeed.fillAmount = 0;
        }
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
                        fastRun.fillAmount = 0;
                        break;
                    case MagicBoxBase.BoxType.radar:
                        CameraController._instance.wholeView = false;
                        radar.fillAmount = 0;
                        break;
                    case MagicBoxBase.BoxType.shootSpeed:
                        tankShoot.intervalTime *= 3;
                        shootSpeed.fillAmount = 0;
                        break;
                }
                buffList.RemoveAt(i);
            }
            else
            {
                buffList[i].currentTime += Time.deltaTime;
                switch (buffList[i].type)
                {
                    case MagicBoxBase.BoxType.moveSpeed:
                        fastRun.fillAmount = (buffList[i].totleTime - buffList[i].currentTime) / buffList[i].totleTime;
                        break;
                    case MagicBoxBase.BoxType.radar:
                        radar.fillAmount = (buffList[i].totleTime - buffList[i].currentTime) / buffList[i].totleTime;
                        break;
                    case MagicBoxBase.BoxType.shootSpeed:
                        shootSpeed.fillAmount = (buffList[i].totleTime - buffList[i].currentTime) / buffList[i].totleTime;
                        break;
                }
            }
        }
    }

    //检测有没有同类buff
    public bool CheckBuff(int buffTypeIndex)
    {
        Buff buff = new Buff(buffTypeIndex);
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].type == buff.type)
            {
                return false;
            }
        }
        return true;
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
