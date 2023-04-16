using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCheckPoint : MonoBehaviour
{
    public CP[] CP;
    public Transform Player;
    //���ư� ���� Ʈ������, ��Ƽ�÷��̽� ���� �����Ͽ� ������ �� ����.

    private void Awake()
    {
        init();
    }

    void init()
    {
        //���� �����ϴ� CP Component ����.
        CP = GameObject.Find("=====CheckPoint======").GetComponentsInChildren<CP>();

        //���� �����ϴ� �ִ� CP ���� ����.
        PlayManager.CP = 0;
        PlayManager.nextCP = new CP[] { CP[0] };
        PlayManager.MaxCP = CP.Length;

        //�� CP Index �ο�
        for (int i=0; i<CP.Length; i++)
        {
            CP[i].CheckPointNum = i;
        }

        
    }
    
    public IEnumerator ReturnToCP()
    {
        /* ������
        PlayManager.isReturning = true;
        // üũ����Ʈ�� ���ư� ���� īƮ�� ������ ����.
        Rigidbody rb = Player.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // �ֱ� üũ����Ʈ�� ��ġ�� ���.
        Transform ReturnPoint = CP[PlayManager.CP].GetReturnPoint();

        // �ֱ� üũ����Ʈ�� īƮ�� ��ġ/ȸ�� ����
        Player.position = ReturnPoint.position + new Vector3(0, 1, 0);
        Player.rotation = ReturnPoint.rotation;

        //0.3�� ���� �ٽ� �����̱�.
        yield return new WaitForSeconds(0.3f);
        rb.constraints = RigidbodyConstraints.None;
        PlayManager.isReturning = false;
        */
        yield return null;
    }

}
