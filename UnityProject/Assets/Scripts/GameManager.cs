using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager s_Instance;// ���ϼ��� ����.
    public static GameManager Instance { get { Init(); return s_Instance; } }
    SoundManager _Sound = new SoundManager();
    public static SoundManager Sound { get { return Instance._Sound; } }

    static void Init()
    {

        if (s_Instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                //���� Manager Object�� ���� �� ���� ����.
                go = new GameObject { name = "@Managers" };
                go.AddComponent<GameManager>();
            };
            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<GameManager>();
            s_Instance._Sound.Init();
        }

    }

    void Awake()
    {
        Init();
    }

}
