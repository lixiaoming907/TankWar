using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{
    public static GameController _instance;

    private Camera camera0;

    private List<GameObject> tankTransList = new List<GameObject>();

    private Vector3 m_DesiredPosition;
    private Vector3 m_MoveVelocity;
    public float m_DampTime = 0.2f;
    private float m_ZoomSpeed;
    public float m_ScreenEdgeBuffer = 4f;
    public float m_MinSize = 6.5f;

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {
        camera0 = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LateUpdate()
    {
        CameraMove();//相机的移动

        CameraZoom(); //相机的缩放
    }

    private void CameraMove()
    {
        FindAveragePosition();
        camera0.gameObject.transform.position = Vector3.SmoothDamp(camera0.gameObject.transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void CameraZoom()
    {
        float requiredSize = FindRequiredSize();
        camera0.orthographicSize = Mathf.SmoothDamp(camera0.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = camera0.gameObject.transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < tankTransList.Count; i++)
        {
            if (!tankTransList[i].activeSelf)
                continue;

            Vector3 targetLocalPos = camera0.gameObject.transform.InverseTransformPoint(tankTransList[i].transform.position);
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

        for (int i = 0; i < tankTransList.Count; i++)
        {
            if (!tankTransList[i].gameObject.activeSelf)
                continue;

            averagePos += tankTransList[i].transform.position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = camera0.gameObject.transform.position.y;

        m_DesiredPosition = averagePos;
    }

    public void AddPlayerInit(GameObject player)
    {
        if (!tankTransList.Contains(player))
        {
            tankTransList.Add(player);
            player.SetActive(true);
        }
    }

    public void RemovePlayerFromList(GameObject player)
    {
        if (tankTransList.Contains(player))
        {
            tankTransList.Remove(player);
        }
    }
}
