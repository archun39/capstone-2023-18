using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    static PlayManager s_Instance;
    public static PlayManager Instace { get { Init(); return s_Instance; } }

    //���� �����ߴ���?
    public static bool isStart;
    public static float StartTime;


    static void Init()
    {
        if (s_Instance == null)
        {
            GameObject go = GameObject.Find("@PlayManager");

            if (go == null)
            {
                //���� Manager Object�� ���� �� ���� ����.
                go = new GameObject { name = "@PlayManager" };
                go.AddComponent<PlayUI>();
                go.AddComponent<CountDown>();
            };
            s_Instance = go.GetComponent<PlayManager>();
        }
        isStart = false;
        StartTime = 3f;
    }


    void Awake()
    {
        Init();
    }

}
