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

    //�÷��� ���൵ üũ
    public static int Lap; // �� ���� ����������.
    public static int MaxLap; // �� ���� ������.
    public static int CP; // ���� �ִ�� ����� CheckPoint
    public static int MaxCP; // �ʿ� �����ϴ� �ִ� CheckPoint ����.
    public static CP[] nextCP;

    public static bool isReturning;

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
                go.AddComponent<PlayCheckPoint>();
            };
            s_Instance = go.GetComponent<PlayManager>();
        }
        isStart = false;
        isReturning = false;
        StartTime = 3f;
    }


    void Awake()
    {
        Init();
    }

    private void Update()
    {
        if(Lap == MaxLap)
        {
            Debug.Log("FINISH");
        }
    }

}
