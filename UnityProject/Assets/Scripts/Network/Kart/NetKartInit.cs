using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetKartInit : NetworkBehaviour
{
    //īƮ�� ó�� ���� �� �� Setting
    public GameObject Cam;

    void Start()
    {

        if(IsOwner)
        {
            Cam.SetActive(true);
        }
        else
        {
            Cam.SetActive(false);
        }
        
    }


}
