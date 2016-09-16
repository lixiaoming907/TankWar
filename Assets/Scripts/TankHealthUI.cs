using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.UI;

public class TankHealthUI : MonoBehaviour
{

    public TankHealth tankHealth;

    private Image healthImage;
    public Text nameTxt;
	// Use this for initialization
	void Start ()
	{
	    healthImage = GetComponent<Image>();
	    nameTxt.text = Dns.GetHostName();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    healthImage.fillAmount = tankHealth.curHealth / tankHealth.totalHealth;
	}
}
