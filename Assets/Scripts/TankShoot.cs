using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TankShoot : NetworkBehaviour
{

    public GameObject bulletPrefab;
    public AudioSource shootSource;
    public Transform bulletTrans;
    public float intervalTime = 2f; //发射炮弹的时间间隔
    private float fireTime = 0; //发射的时间
    

    [HideInInspector]
    public bool canShoot = true;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (canShoot && Input.GetKeyDown(KeyCode.Space))
            {
                CmdTankFire();
                canShoot = false;
                fireTime = 0;
            }
            if (fireTime < intervalTime)
            {
                fireTime += Time.deltaTime;
            }
            if (fireTime >= intervalTime)
            {
                canShoot = true;
            }
        }
    }

    [Command]
    void CmdTankFire()
    {
        shootSource.Play();
        GameObject bullet = Instantiate(bulletPrefab, bulletTrans.position, bulletTrans.rotation) as GameObject;
        NetworkServer.Spawn(bullet);
    }
}
