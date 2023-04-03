using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetKartInit : NetworkBehaviour
{
    //īƮ�� ó�� ���� �� �� Setting
    public GameObject Cam;
    public Transform spawnPos;

    void Start()
    {
        if(IsServer)
        {
            spawnPos = GameObject.Find("SpawnPosition").transform;
            transform.position = spawnPos.position;
        }

        if(IsOwner)
        {
            GameObject.Find("@PlayManager").GetComponent<PlayUI>().UserKart = gameObject.GetComponent<KartController>();
            GameObject.Find("@PlayManager").GetComponent<PlayCheckPoint>().Player = transform;

            Cam.SetActive(true);
        }
        else
        {
            Cam.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
