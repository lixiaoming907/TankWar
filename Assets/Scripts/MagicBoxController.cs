using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MagicBoxController : NetworkBehaviour
{
    public static MagicBoxController _instance;

    public GameObject magicBoxPrefab;
    public float[] creatBoxInterval;
    private float[] currentTime;
    public Vector3[] boxPos;
    [HideInInspector]
    public bool[] canCreatBox;

    //[HideInInspector]
    //public bool canCreatBox = true;

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    [ServerCallback]
    void Start()
    {
        canCreatBox = new bool[boxPos.Length];
        currentTime = new float[boxPos.Length];
        for (int i = 0; i < canCreatBox.Length; i++)
        {
            canCreatBox[i] = true;
        }
    }


    [ServerCallback]
    void Update()
    {
        for (int i = 0; i < boxPos.Length; i++)
        {
            if (canCreatBox[i])
            {
                if (currentTime[i] >= creatBoxInterval[i])
                {
                    canCreatBox[i] = false;
                    //在服务器生成箱子，并同步到客户端
                    GameObject box = Instantiate(magicBoxPrefab);
                    int typeSend = Random.Range(1, 4);
                    box.transform.position = boxPos[i];
                    box.GetComponent<MagicBoxBase>().buffType = (MagicBoxBase.BoxType)typeSend;
                    box.GetComponent<MagicBoxBase>().posIndex = i;
                    CreatMagicBoxOnClient(box);
                }
                else
                {
                    currentTime[i] += Time.deltaTime;
                }
            }
            else
                currentTime[i] = 0;
        }
    }

    //在客户端生成随机箱子
    void CreatMagicBoxOnClient(GameObject box)
    {
        NetworkServer.Spawn(box);
    }
}
