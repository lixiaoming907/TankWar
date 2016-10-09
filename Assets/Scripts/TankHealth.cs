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

    private CountDownController countDown;

    // Use this for initialization
    void Awake()
    {
        //Debug.Log("当前血量： " + curHealth);
        curHealth = totalHealth;
    }

    void OnEnable()
    {
        GameObject canvase = GameObject.Find("Canvas");
        countDown = canvase.transform.GetChild(0).GetComponent<CountDownController>();
        CameraController._instance.wholeView = false;
        curHealth = totalHealth;
        CmdTankResetPos();
    }

    void Start()
    {
        CameraController._instance.wholeView = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

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
            GameController._instance.RemovePlayerFromList(this.gameObject);

            if (isLocalPlayer)
            {
                CmdTankBoom();
                //进入死亡倒计时
                countDown.isDead = true;
                countDown.gameObject.SetActive(true);
                CameraController._instance.wholeView = true;
            }

            //Destroy(this.gameObject);
        }
    }

    public void OnDestroy()
    {
        CmdTankBoom();
        GameController._instance.RemovePlayerFromList(this.gameObject);
    }

    [Command]
    void CmdTankResetPos()
    {
        RpcActiveTrue(this.gameObject);
    }

    [ClientRpc]
    void RpcActiveTrue(GameObject tank)
    {
        tank.SetActive(true);
    }

    [Command]
    void CmdTankBoom()
    {
        GameObject tankBoom = Instantiate(tankBoomEffect, transform.position, transform.rotation) as GameObject;
        NetworkServer.Spawn(tankBoom);
        RpcActiveFalse(this.gameObject);
    }

    [ClientRpc]
    void RpcActiveFalse(GameObject tank)
    {
        tank.SetActive(false);
    }
}
