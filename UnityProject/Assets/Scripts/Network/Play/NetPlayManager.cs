using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class NetPlayManager : NetworkBehaviour
{
    //���� ���� ��ġ
    public GameObject[] StartingPoints;

    // ��ü������ �˾ƾ� �ϴ°�
    public NetworkVariable<bool> isStart; //���� ������ �����ߴ���
    public NetworkVariable<float> PlayTime; // ���� ���� �ð�.
    public int UserCount = 0;

    //�÷��� ���൵ üũ
    //�� �÷��̾� ���� üũ �� �� �ֵ��� �Ѵ�.
    public int MaxLap; // �� ���� ������
    public int MaxCP; // �ش� �ʿ� CP�� �� �� �ִ���.

    //�÷��̾ ���൵
    private SortedDictionary<ulong, NetPlayerInfo> Players = new SortedDictionary<ulong, NetPlayerInfo>();

    //����
    public NetworkList<ulong> rank;


    public void Awake()
    {
        rank = new NetworkList<ulong>();
    }

    private void Start()
    {

        setMapInfo();
    }

    private void Update()
    {
        if(IsServer)
        {
            //�׽�Ʈ �ڵ�
            if(Input.GetKeyDown(KeyCode.F5) && isStart.Value == false)
            {
                StartGameButton();
            }

            if(isStart.Value)
            {
                PlayTime.Value += Time.deltaTime;
                GetRank();
            }
        }

    }

    //�׽�Ʈ �ڵ� �ӽ÷� �����ϱ� ����.
    void StartGameButton()
    {
        StartCoroutine(StartCountDown());
    }

    public IEnumerator StartCountDown()
    {
        NetPlayUI ui = GetComponent<NetPlayUI>();
        for(int i=3; i>0; i--)
        {
            ui.Count.text = i.ToString();
            ui.CountdownClientRPC(i);
            yield return new WaitForSeconds(1);
        }
        ui.CountdownClientRPC(0);
        isStart.Value = true;
    }

    void setMapInfo()
    {
        //������ ���� ������ �޾ƿͼ� ����Ѵ�.
        MaxLap = 1;
        MaxCP = GameObject.FindGameObjectsWithTag("Checkpoint").Count();
        StartingPoints = GameObject.FindGameObjectsWithTag("StartPoint");
    }

    //��ŷ ���
    void GetRank()
    {
        //1. LAP �켱 ����
        //2. CHECK POINT �� �켱 ����
        //3. CHECK POINT ���� �� �Ÿ� ��� ���

        /*
        var _rank = from pair in Players
                       orderby pair.Value.Lap, pair.Value.RpNum, pair.Value.CheckPointDistance
                       select pair;

        */
        var _rank = Players.OrderByDescending(r => r.Value, new NetPlayerInfoComparer())
                   .Select(r => r.Key)
                   .ToArray();

        int i = 0;
        foreach (ulong key in _rank)
        {
            rank[i] = _rank[i];
            i++;
        }

    }


    //�� �÷��̾� ���� Ȯ��.

    //Player ����� ������ ���� �ִ� Player �����Ϳ� �߰�.
    [ServerRpc (RequireOwnership = false)]
    public void AddPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong uid = serverRpcParams.Receive.SenderClientId;

        if (!Players.ContainsKey(uid))
        {
            NetworkObject user = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
            NetPlayerInfo userinfo = user.GetComponent<NetPlayerInfo>();

            Players.Add(uid, userinfo);
            rank.Add(uid);
            Debug.Log(user.name);
            user.transform.position = StartingPoints[UserCount].transform.position;
            UserCount += 1;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CloseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        OnGameClose();
    }
    //���� ����

    private async void OnGameClose()
    {
        using (new Load("Closing the game..."))
        {
            await MatchmakingService.UnLockLobby();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
    }
}
