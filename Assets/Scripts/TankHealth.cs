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
    void Awake()
    {
        //Debug.Log("当前血量： " + curHealth);
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
            StartCoroutine(TankDestroy());
        }
    }

    private IEnumerator TankDestroy()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }

    public void OnDestroy()
    {
        CmdTankBoom();
    }

    [Command]
    void CmdTankBoom()
    {
        GameObject tankBoom = Instantiate(tankBoomEffect, transform.position, transform.rotation) as GameObject;
        //this.gameObject.SetActive(false);
        NetworkServer.Spawn(tankBoom);
    }
}
