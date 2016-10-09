using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TankMoveController : NetworkBehaviour
{
    public float moveSpeed = 10;
    public float rotateSpeed = 5;

    public GameObject playerController;

    private Rigidbody rigid;
    [HideInInspector]
    public bool canMove = true;

    public AudioClip engineIdleClip; //不移动声音
    public AudioClip engineWalkClip; //移动声音

    public AudioSource audioSource;

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
