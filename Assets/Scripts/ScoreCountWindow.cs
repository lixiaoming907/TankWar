using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreCountWindow : MonoBehaviour
{

    public GameObject scoreCountPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Tab)) //显示分数面板
	    {
            scoreCountPanel.SetActive(true);
        }
	    if (Input.GetKeyUp(KeyCode.Tab)) //隐藏分数面板
	    {
            scoreCountPanel.SetActive(false);
        }
	}
}
