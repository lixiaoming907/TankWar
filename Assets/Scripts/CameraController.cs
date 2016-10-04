using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CameraController : MonoBehaviour
{

    public static CameraController _instance;

    private Camera camera0;

    private Vector3 m_DesiredPosition;
    private Vector3 m_MoveVelocity;
    private float m_ZoomSpeed;

    public float m_DampTime = 0.2f;
    public float m_ScreenEdgeBuffer = 4f;
    public float m_MinSize = 6.5f;
    public Vector3 m_ViewRange; //摄像机前方距离
    public float defaultCameraSize; //摄像机标准大小
    public float spreadRate;  //摄像机和坦克之间的权重

    [HideInInspector]
    public GameObject mTankPrefab;

    //[HideInInspector]
    public bool wholeView = false; //雷达效果

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {
        camera0 = Camera.main;
    }

    public void LateUpdate()
    {
        if (mTankPrefab)
        {
            CameraMove();//相机的移动

            CameraZoom(); //相机的缩放
        }
    }

    private void CameraMove()
    {
        FindAveragePosition();
        camera0.gameObject.transform.position = Vector3.SmoothDamp(camera0.gameObject.transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void CameraZoom()
    {
        if (wholeView)
        {
            float requiredSize = FindRequiredSize();
            camera0.orthographicSize = Mathf.SmoothDamp(camera0.orthographicSize, requiredSize, ref m_ZoomSpeed,
                m_DampTime);
        }
        else
        {
            camera0.orthographicSize = Mathf.SmoothDamp(camera0.orthographicSize, defaultCameraSize, ref m_ZoomSpeed,
                m_DampTime);
        }
    }

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = camera0.gameObject.transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < GameController._instance.tankTransList.Count; i++)
        {
            if (!GameController._instance.tankTransList[i].activeSelf)
                continue;

            Vector3 targetLocalPos =
                camera0.gameObject.transform.InverseTransformPoint(GameController._instance.tankTransList[i].transform.position);
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / camera0.aspect);
        }

        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);
        return size;
    }

    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;
        if (wholeView) //全局视野buff
        {
            for (int i = 0; i < GameController._instance.tankTransList.Count; i++)
            {
                if (!GameController._instance.tankTransList[i].gameObject.activeSelf)
                    continue;

                averagePos += GameController._instance.tankTransList[i].transform.position;
                numTargets++;
            }

            if (numTargets > 0)
                averagePos /= numTargets;

        }
        else
        {
            averagePos = (mTankPrefab.transform.position +
                          (mTankPrefab.transform.forward + mTankPrefab.transform.TransformPoint(m_ViewRange))) * spreadRate;
        }
        averagePos.y = camera0.gameObject.transform.position.y;
        m_DesiredPosition = averagePos;
    }
}
