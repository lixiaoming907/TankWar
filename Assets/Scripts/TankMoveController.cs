using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class TankMoveController : NetworkBehaviour
{
    public float moveSpeed = 10;
    public float rotateSpeed = 5;

    private Rigidbody rigid;
    [HideInInspector]
    public bool canMove = true;

    public AudioClip engineIdleClip; //不移动声音
    public AudioClip engineWalkClip; //移动声音

    public AudioSource audioSource;

    public GameObject tankAbove; //坦克上半身
    private Vector3 mousePositionOffer; //用来确定鼠标位置
    private RaycastHit hitInfo;
    private Ray ray;

    private bool _isMove;
    [HideInInspector]
    public bool isMove
    {
        get { return _isMove; }
        set
        {
            if (value != _isMove)
            {
                if (value)
                {
                    audioSource.clip = engineWalkClip;
                    audioSource.Play();
                }
                else
                {
                    audioSource.clip = engineIdleClip;
                    audioSource.Play();
                }
            }
            _isMove = value;
        }
    }

    public void OnEnable()
    {
        GameController._instance.AddPlayerInit(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        if (isLocalPlayer)
        {
            CameraController._instance.mTankPrefab = this.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, 1000))
            {
                Vector3 playerToMouse = hitInfo.point - transform.position;

                playerToMouse.y = 0f; //不会向上

                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);
                //传入向量playerToMouse，返回一个四元数类型的newRotation

                //myPlayerRigidbody.MoveRotation(newRotatation); //人物朝向向量方向
                tankAbove.transform.rotation = Quaternion.Lerp(tankAbove.transform.rotation, newRotatation, 0.03f);
            }
        }

    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (canMove && (h != 0 || v != 0))
            {
                //改变引擎声音
                isMove = true;
                TankMove(h, v);
            }
            else
            {
                //改变引擎声音
                isMove = false;
            }
        }
    }

    //控制坦克的移动和转向
    void TankMove(float h, float v)
    {
        rigid.AddForce(transform.forward * v * moveSpeed * Time.deltaTime, ForceMode.Force);
        transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
    }


}
