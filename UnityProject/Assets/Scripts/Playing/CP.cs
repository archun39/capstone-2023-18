using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP : MonoBehaviour
{
    public int CheckPointNum;
    public CP[] NextCheckPoint;
    PlayCheckPoint playcheckpoint;
    public bool CorrectNext;
    /*

    private void Start()
    {
        playcheckpoint = GameObject.Find("@PlayManager").GetComponent<PlayCheckPoint>();
    }

    public Transform GetReturnPoint()
    {
        return gameObject.transform;
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Kart"))
        {
            CorrectNext = false;
            for (int i = 0; i < PlayManager.nextCP.Length; i++)
            {
                if (PlayManager.nextCP[i].CheckPointNum == CheckPointNum)
                {
                    CorrectNext = true;
                }
            }
            if (CorrectNext || PlayManager.CP == CheckPointNum)
            {
                //�ùٸ��� ���� �� ���
                PlayManager.CP = CheckPointNum;
                PlayManager.nextCP = NextCheckPoint;
            }
            else
            {
                //������ �� �ùٸ��� ���� ������ ���
                StartCoroutine(playcheckpoint.ReturnToCP());
            }
        }
    }
    */
}
