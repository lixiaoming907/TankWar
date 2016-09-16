using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BulletController : NetworkBehaviour
{

    public float dealthTime = 2f; //死亡时间
    public float flySpeed = 5f; //初始飞行速度
    public float radius = 5f; //炮弹爆炸半径.
    public float boomPower = 10f; //炮弹爆炸力
    public GameObject bulletBoom; //子弹爆炸特效

    private Rigidbody rigid;
    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, dealthTime);
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * flySpeed, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ServerCallback]
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (isServer)
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
            if (colliders.Length > 0)
            {
                foreach (Collider hit in colliders)
                {
                    if (hit && hit.GetComponent<Rigidbody>() && hit.GetComponent<TankHealth>())
                    {
                        RpcGetShot(hit.gameObject, explosionPos);
                    }
                }
            }
            GameObject bulletBommPrefab = Instantiate(bulletBoom, transform.position, transform.rotation) as GameObject;
            NetworkServer.Spawn(bulletBommPrefab);
            StartCoroutine(DestroyBullet());
        }
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }

    [ClientRpc]
    void RpcGetShot(GameObject rigid, Vector3 explosionPos)
    {
        rigid.GetComponent<Rigidbody>().AddForce((rigid.transform.position - explosionPos).normalized * boomPower, ForceMode.Impulse);
        rigid.GetComponent<Rigidbody>().AddExplosionForce(boomPower, explosionPos, radius, 0F);
        rigid.GetComponent<TankHealth>().TakeDamage();
    }
}

