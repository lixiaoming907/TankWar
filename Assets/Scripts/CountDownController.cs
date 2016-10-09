using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CountDownController : MonoBehaviour
{
    public GameObject CountDownObject;
    public Text CountDownText;

    public GameObject tankPrefab;
    public NetworkStartPosition[] startPos;
    [HideInInspector]
    public bool isDead;

    public float totleDeadTime = 10f;
    private float deadTime;
    private int countDown;

    public void OnEnable()
    {
        deadTime = totleDeadTime;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            CountDownObject.SetActive(true);
            if (deadTime <= 0)
            {
                isDead = false;
                deadTime = 0;
            }
            else
            {
                deadTime -= Time.deltaTime;
                countDown = (int)deadTime;
                CountDownText.text = countDown.ToString();
            }
        }
        else
        {
            //坦克重生
            Vector3 pos = startPos[Random.Range(0, startPos.Length)].transform.position;
            CameraController._instance.mTankPrefab.SetActive(true);
            CameraController._instance.mTankPrefab.transform.position = pos;
            CameraController._instance.wholeView = false;

            CountDownObject.SetActive(false);
        }
    }
}
