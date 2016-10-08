using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking;

public class ScoreController : NetworkBehaviour
{

    public struct Score
    {
        public string tankName;
        public int deadCount;
        public int killCount;

        public Score(string tName)
        {
            tankName = tName;
            deadCount = killCount = 0;
        }
    }

    public class ScoreList : SyncListStruct<Score>{}
    ScoreList scoreList = new ScoreList();

    //List<Score> scoreList = new List<Score>();
    
    public static ScoreController _instance;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        scoreList.Callback = OnScoreChanged;
    }

    public void OnScoreChanged(SyncList<Score>.Operation op, int itemindex)
    {
        ScoreCountWindow._instance.ResetScorePanel(scoreList);
    }

    [Command]
    public void CmdAddPlayer(string tName)
    {
        Score score = new Score(tName);
        scoreList.Add(score);
        //ScoreCountWindow._instance.RpcAddPlayerPanel(tName);
    }

    [Command]
    public void CmdRemovePlayer(string tName)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].tankName == tName)
            {
                scoreList.Remove(scoreList[i]);
                //ScoreCountWindow._instance.RpcRemovePlayerPanel(tName);
            }
        }
    }

    [Command]
    public void CmdAddDeadCount(string tName)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].tankName == tName)
            {
                Score newScore = new Score(scoreList[i].tankName);
                newScore.deadCount = scoreList[i].deadCount + 1;
                newScore.killCount = scoreList[i].killCount;
                scoreList[i] = newScore;
                //ScoreCountWindow._instance.RpcAddDeadCount(tName);
                return;
            }
        }
    }

    [Command]
    public void CmdAddKillCount(string tName)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].tankName == tName)
            {
                Score newScore = new Score(scoreList[i].tankName);
                newScore.deadCount = scoreList[i].deadCount;
                newScore.killCount = scoreList[i].killCount + 1;
                scoreList[i] = newScore;
                //ScoreCountWindow._instance.RpcAddKillCount(tName);
            }
        }
    }
}
