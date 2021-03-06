﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class MagicBox : MagicBoxBase
{
    GameObject tank;
    
    public float boxRotateSpeed = 10;

    [ServerCallback]
    void Update()
    {
        transform.Rotate(transform.up, boxRotateSpeed * Time.deltaTime);
    }

    [ServerCallback] //根据不同的类型 给客户端发消息增加不同的buff
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            tank = other.gameObject;
            switch (buffType)
            {
                case BoxType.moveSpeed:
                    RpcAddSpeedBuff(tank);
                    break;
                case BoxType.radar:
                    RpcAddRadarBuff(tank);
                    break;
                case BoxType.shootSpeed:
                    RpcAddShootBuff(tank);
                    break;
            }
            StartCoroutine(DestroyBox());
        }
    }

    [ClientRpc]
    void RpcAddSpeedBuff(GameObject tankObject)
    {
        if (CameraController._instance.mTankPrefab == tankObject)
        {
            TankBuff tankBuff = CameraController._instance.mTankPrefab.GetComponent<TankBuff>();
            if (tankBuff.CheckBuff((int) BoxType.moveSpeed))
            {
                TankMoveController moveController =
                    CameraController._instance.mTankPrefab.GetComponent<TankMoveController>();
                moveController.moveSpeed *= 2;
                moveController.rotateSpeed *= 2;
            }
            tankBuff.AddBuff((int)BoxType.moveSpeed);
        }
    }

    [ClientRpc]
    void RpcAddShootBuff(GameObject tankObject)
    {
        if (CameraController._instance.mTankPrefab == tankObject)
        {
            TankBuff tankBuff = CameraController._instance.mTankPrefab.GetComponent<TankBuff>();
            if (tankBuff.CheckBuff((int) BoxType.shootSpeed))
            {
                TankShoot tankShoot = CameraController._instance.mTankPrefab.GetComponent<TankShoot>();
                tankShoot.intervalTime /= 3;
            }
            tankBuff.AddBuff((int)BoxType.shootSpeed);
        }
    }

    [ClientRpc]
    void RpcAddRadarBuff(GameObject tankObject)
    {
        if (CameraController._instance.mTankPrefab == tankObject)
        {
            TankBuff tankBuff = CameraController._instance.mTankPrefab.GetComponent<TankBuff>();
            if (tankBuff.CheckBuff((int) BoxType.radar))
            {
                CameraController._instance.wholeView = true;
            }
            tankBuff.AddBuff((int)BoxType.radar);
        }
    }

    private IEnumerator DestroyBox()
    {
        yield return new WaitForSeconds(0.1f);
        MagicBoxController._instance.canCreatBox[posIndex] = true;
        Destroy(this.gameObject);
    }
}
