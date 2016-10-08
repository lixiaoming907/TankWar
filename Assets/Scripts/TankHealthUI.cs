using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TankHealthUI : NetworkBehaviour
{

    public TankHealth tankHealth;

    private Image healthImage;
    public Text nameTxt;

    [HideInInspector]
    [SyncVar(hook = "OnTankNameChanged")]
    public string tankName;

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        healthImage = GetComponentInChildren<Image>();
        StartCoroutine(TankeStart());
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (isLocalPlayer)
        {
            string tankName = Dns.GetHostName();
            ScoreController._instance.CmdAddPlayer(tankName);
        }
    }

    private IEnumerator TankeStart()
    {
        yield return new WaitForSeconds(0.1f);
        if (isLocalPlayer)
        {
            string name = Dns.GetHostName();
            nameTxt.text = name;
            tankName = name;
            CmdDefineName(tankName);
            this.gameObject.name = name;
        }
        else
        {
            nameTxt.text = tankName;
        }
    }

    [Command]
    private void CmdDefineName(string tName)
    {
        tankName = tName;
    }

    // Update is called once per frame
    void Update()
    {
        healthImage.fillAmount = tankHealth.curHealth / tankHealth.totalHealth;
    }

    void OnTankNameChanged(string tName)
    {
        if (!isLocalPlayer)
        {
            Debug.Log("名字改变：" + tName);
            nameTxt.text = tName;
            this.gameObject.name = tName;
        }
    }
}
