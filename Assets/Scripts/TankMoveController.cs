using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class TankMoveController : NetworkBehaviour
{
    public float moveSpeed = 10;
    public float rotateSpeed = 5;
    public float gangKouSpeed = 0.02f;

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

    private float offsetV = 0;
    private Vector3 crossValue;

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
            CameraController._instance.mTankPaoKou = tankAbove;
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
                tankAbove.transform.rotation = Quaternion.Lerp(tankAbove.transform.rotation, newRotatation, gangKouSpeed);
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
        rigid.AddForce(Vector3.forward * v * moveSpeed * Time.deltaTime, ForceMode.Force);
        rigid.AddForce(Vector3.right * h * moveSpeed * Time.deltaTime, ForceMode.Force);

        if (v > 0 && h == 0) //向上
        {
            offsetV = Vector3.Angle(Vector3.forward, transform.forward); //计算自身Z轴和世界Z轴的夹角
            crossValue = Vector3.Cross(Vector3.forward, transform.forward); //计算两个向量的叉积 本游戏以XZ轴为平面 用来判断坦克自身方向在世界坐标的顺时针还是逆时针

            if ((offsetV < 4 && offsetV >= 0))
            {
                offsetV = 0;
                transform.forward = Vector3.forward;
            }
            else if (offsetV <= 180 && offsetV > 176)
            {
                offsetV = 180;
                transform.forward = Vector3.back;
            }
            else if (crossValue.y > 0 && offsetV < 90) //坦克自身Z轴在世界坐标Z轴的顺时针方向，需要反向旋转
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV <= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y > 0 && offsetV >= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV > 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
            
        }
        else if (v < 0 && h == 0) //向下
        {
            offsetV = Vector3.Angle(Vector3.back, transform.forward);
            crossValue = Vector3.Cross(Vector3.back, transform.forward);

            if ((offsetV < 4 && offsetV >= 0))
            {
                offsetV = 0;
                transform.forward = Vector3.back;
            }
            else if (offsetV <= 180 && offsetV > 176)
            {
                offsetV = 180;
                transform.forward = Vector3.forward;
            }
            else if (offsetV == 180)
            {
                rigid.AddForce(transform.forward * v * moveSpeed * Time.deltaTime, ForceMode.Force);
            }
            else if (offsetV == 0)
            {
                rigid.AddForce(transform.forward * -v * moveSpeed * Time.deltaTime, ForceMode.Force);
            }
            else if (crossValue.y > 0 && offsetV < 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV <= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y > 0 && offsetV >= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV > 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
        }
        else if (h > 0 && v == 0) //向右
        {
            offsetV = Vector3.Angle(Vector3.right, transform.forward);
            crossValue = Vector3.Cross(Vector3.right, transform.forward);

            if ((offsetV < 4 && offsetV >= 0))
            {
                offsetV = 0;
                transform.forward = Vector3.right;
            }
            else if (offsetV <= 180 && offsetV > 176)
            {
                offsetV = 180;
                transform.forward = Vector3.left;
            }
            else if (crossValue.y > 0 && offsetV < 90)
            {
                transform.RotateAround(transform.position, transform.up, h * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV <= 90)
            {
                transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y > 0 && offsetV >= 90)
            {
                transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV > 90)
            {
                transform.RotateAround(transform.position, transform.up, h * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
            }
        }
        else if (h < 0 && v == 0) //向左
        {
            offsetV = Vector3.Angle(Vector3.left, transform.forward);
            crossValue = Vector3.Cross(Vector3.left, transform.forward);

            if ((offsetV < 4 && offsetV >= 0))
            {
                offsetV = 0;
                transform.forward = Vector3.left;
            }
            else if (offsetV <= 180 && offsetV > 176)
            {
                offsetV = 180;
                transform.forward = Vector3.right;
            }
            else if (crossValue.y > 0 && offsetV < 90)
            {
                transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV <= 90)
            {
                transform.RotateAround(transform.position, transform.up, h * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y > 0 && offsetV >= 90)
            {
                transform.RotateAround(transform.position, transform.up, h * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV > 90)
            {
                transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -h * rotateSpeed * Time.deltaTime);
            }
        }
        else if (h < 0 && v < 0) //左下
        {
            offsetV = Vector3.Angle((Vector3.left + Vector3.back) / 2, transform.forward);
            crossValue = Vector3.Cross((Vector3.left + Vector3.back) / 2, transform.forward);

            if ((offsetV < 4 && offsetV >= 0))
            {
                offsetV = 0;
                transform.forward = (Vector3.left + Vector3.back) / 2;
            }
            else if (offsetV <= 180 && offsetV > 176)
            {
                offsetV = 180;
                transform.forward = -(Vector3.left + Vector3.back) / 2;
            }
            else if (crossValue.y > 0 && offsetV < 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV <= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y > 0 && offsetV >= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV > 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
        }
        else if (h > 0 && v < 0) //右下
        {
            offsetV = Vector3.Angle((Vector3.right + Vector3.back) / 2, transform.forward);
            crossValue = Vector3.Cross((Vector3.right + Vector3.back) / 2, transform.forward);

            if ((offsetV < 4 && offsetV >= 0))
            {
                offsetV = 0;
                transform.forward = (Vector3.right + Vector3.back) / 2;
            }
            else if (offsetV <= 180 && offsetV > 176)
            {
                offsetV = 180;
                transform.forward = -(Vector3.right + Vector3.back) / 2;
            }
            else if (crossValue.y > 0 && offsetV < 90)
            {
                transform.RotateAround(transform.position, transform.up, h * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV <= 90)
            {
                transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y > 0 && offsetV >= 90)
            {
                transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -h * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV > 90)
            {
                transform.RotateAround(transform.position, transform.up, h * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
            }
        }
        else if (h > 0 && v > 0) //右上
        {
            offsetV = Vector3.Angle((Vector3.right + Vector3.forward) / 2, transform.forward);
            crossValue = Vector3.Cross((Vector3.right + Vector3.forward) / 2, transform.forward);

            if ((offsetV < 4 && offsetV >= 0))
            {
                offsetV = 0;
                transform.forward = (Vector3.right + Vector3.forward) / 2;
            }
            else if (offsetV <= 180 && offsetV > 176)
            {
                offsetV = 180;
                transform.forward = -(Vector3.right + Vector3.forward) / 2;
            }
            else if (crossValue.y > 0 && offsetV < 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV <= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y > 0 && offsetV >= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV > 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
        }
        else if (h < 0 && v > 0) //左上
        {
            offsetV = Vector3.Angle((Vector3.left + Vector3.forward) / 2, transform.forward);
            crossValue = Vector3.Cross((Vector3.left + Vector3.forward) / 2, transform.forward);

            if ((offsetV < 4 && offsetV >= 0))
            {
                offsetV = 0;
                transform.forward = (Vector3.left + Vector3.forward) / 2;
            }
            else if (offsetV <= 180 && offsetV > 176)
            {
                offsetV = 180;
                transform.forward = -(Vector3.left + Vector3.forward) / 2;
            }
            else if (crossValue.y > 0 && offsetV < 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV <= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y > 0 && offsetV >= 90)
            {
                transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, -v * rotateSpeed * Time.deltaTime);
            }
            else if (crossValue.y < 0 && offsetV > 90)
            {
                transform.RotateAround(transform.position, transform.up, v * -1 * rotateSpeed * Time.deltaTime);
                tankAbove.transform.RotateAround(transform.position, transform.up, v * rotateSpeed * Time.deltaTime);
            }
        }

        //rigid.AddForce(transform.forward * v * moveSpeed * Time.deltaTime, ForceMode.Force);
        //transform.RotateAround(transform.position, transform.up, h * rotateSpeed * Time.deltaTime);
    }


}
