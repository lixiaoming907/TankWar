using UnityEngine;
using System.Collections;
using System.Security;
using UnityEngine.Networking;

public class TankHealth : NetworkBehaviour
{
    public GameObject tankBoomEffect;

    [HideInInspector]
    public float totalHealth = 100;
    [HideInInspector]
    [SyncVar]
    public float curHealth;

    // Use this for initialization
    void Start()
    {
        curHealth = totalHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //void OnCollisionEnter(Collision collisionInfo)
    //{
    //    if (collisionInfo.gameObject.tag == "Bullet")
    //    {
    //        TakeDamage();
    //    }
    //}

    public void TakeDamage()
    {
        if (curHealth > 0)
        {
            curHealth -= 10;
        }
        CheckTankHealth();
    }


    private void CheckTankHealth()
    {
        if (curHealth <= 0)//坦克死亡，该爆炸了
        {
            CmdTankBoom();
            GameController._instance.RemovePlayerFromList(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    [Command]
    void CmdTankBoom()
    {
        GameObject tankBoom = Instantiate(tankBoomEffect, transform.position, transform.rotation) as GameObject;
        //this.gameObject.SetActive(false);
        NetworkServer.Spawn(tankBoom);
    }
}
