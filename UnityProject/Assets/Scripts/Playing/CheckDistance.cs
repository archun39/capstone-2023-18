using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckDistance : MonoBehaviour
{
    public int MaxLap; //�ʴ� �ִ� ����
    public int NowLap; //���� ����

    public float CheckPointDistance; // üũ����Ʈ ���� �Ÿ�

    public Vector3 LastCP;
    public Vector3 LastCPPos;
    public int LastCPNumber;

    public TextMeshProUGUI disText;

    void Start()
    {
        MaxLap = 3;
        NowLap = 0;

        LastCP = GameObject.Find("CP00").gameObject.GetComponent<CheckPoiintInfo>().forward;
        LastCPPos = GameObject.Find("CP00").transform.position;
        LastCPNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPointDistance = Vector3.Dot(LastCP, (transform.position - LastCPPos));
        disText.text = gameObject.name + " " + NowLap + " " + LastCPNumber + " " + CheckPointDistance;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Checkpoint"))
        {
            CheckPoiintInfo cpinfo = other.GetComponent<CheckPoiintInfo>();

            if(LastCPNumber != cpinfo.CP_Num)
            {
                LastCP = cpinfo.forward;
                LastCPNumber = cpinfo.CP_Num;
                LastCPPos = cpinfo.centerPos;
            }
        }
    }
}
