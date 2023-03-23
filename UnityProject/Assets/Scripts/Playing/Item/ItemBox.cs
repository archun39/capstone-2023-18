using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = 0; //Default Layer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Kart") && gameObject.layer != 30)
        {
            StartCoroutine(RemakeBox(other.transform.GetComponent<KartController>()));
        }
    }

    ITEMS SelectItem()
    {
        //�������� ȹ���� ������ ��Ȳ�� ���߾� �������� ������.
        //����� �ν��͹ۿ� �����Ƿ� �ν��͸�.
        return ITEMS.BOOST;
    }

    IEnumerator RemakeBox(KartController user)
    {
        //������ ȹ�� ��, �ڽ� disable
        //n�� �� �����
        user.hasItem = SelectItem();
        gameObject.layer = 30;
        yield return new WaitForSeconds(2f);
        gameObject.layer = 0;
    }
}
