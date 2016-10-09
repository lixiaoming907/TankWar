using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MagicBoxController : NetworkBehaviour
{
    public static MagicBoxController _instance;

    public GameObject magicBoxPrefab;
    public float creatBoxInterval = 10f;
    public float currentTime = 0;
    public Vector3[] boxPos;

    [HideInInspector]
    public bool canCreatBox = true;

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
	
	}
	

    [ServerCallback]
	void Update () {
        if (canCreatBox)
        {
            if (currentTime >= creatBoxInterval)
            {
                //在服务器生成箱子，并同步到客户端
                GameObject box = Instantiate(magicBoxPrefab);
                int typeSend = Random.Range(1, 4);
                int posSend = Random.Range(1, boxPos.Length);
                magicBoxPrefab.transform.position = boxPos[posSend];
                box.GetComponent<MagicBoxBase>().buffType = (MagicBoxBase.BoxType)typeSend;
                CreatMagicBoxOnClient(box);
                canCreatBox = false;
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }
        else
            currentTime = 0;
	}

    //在客户端生成随机箱子
    void CreatMagicBoxOnClient(GameObject box)
    {
        NetworkServer.Spawn(box);
    }
}
