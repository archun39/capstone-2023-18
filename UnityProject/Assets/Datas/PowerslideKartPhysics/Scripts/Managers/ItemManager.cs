﻿
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace PowerslideKartPhysics
{
    // This class manages items to be used by karts, deriving items from child objects
    public class ItemManager : NetworkBehaviour
    {
        public static ItemManager instance;
        Item[] items = new Item[0];
        public NetKartController[] allKarts = new NetKartController[0];
        public List<playerData> PlayerDatas = new List<playerData>();
        public List<playerData> RedTeam = new List<playerData>(); // teamnum = 0
        public List<playerData> BlueTeam = new List<playerData>(); // teamnum = 1
        public NetworkObject No1Player;
        public NetPlayManager npm;
        
        public enum SpinAxis { Yaw = 0, Pitch = 1, Roll = 2 }
        public enum itemName
        {
            //monkey = HomingItem
            position, HomingItem = 0, GuardOneItem = 1, LimitSkillItem = 2, SquidItem = 3, 
            BirdStrikeItem = 4, FishItem = 5, BoostItem = 6, ShieldItem = 7, SlowItem = 8,
            ThunderItem = 9,BombItem = 10,BufferItem_ReverseTeam = 11,RushItem = 12
        }
        private void Awake()
        {
            StartCoroutine(FindComponent());
        }
        public override void OnNetworkSpawn()
        {
            Init();
            items = GetComponentsInChildren<Item>();
            allKarts = FindObjectsOfType<NetKartController>();
        }

        private void Update()
        {
            if (IsServer && npm != null && npm.isStart.Value)
            {
                if(npm.rank.Count != 0)
                {
                    No1Player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(npm.rank[0]);
                }

            }
        }
        IEnumerator FindComponent()
        {
            while (GameObject.Find("@PlayManager") == null)
            {
                yield return null;
            }
            GameObject.Find("@PlayManager").TryGetComponent(out npm);
        }
        public void GetAllKarts()
        {
            if (!IsServer) return;
            allKarts = FindObjectsOfType<NetKartController>();
            
            PlayerDatas = new List<playerData>();
            for (int i = 0; i < allKarts.Length; i++)
            {
                playerData tmp;
                tmp.clientId = allKarts[i].GetComponent<NetworkObject>().OwnerClientId;
                tmp.networkobjectId = allKarts[i].GetComponent<NetworkObject>().NetworkObjectId;
                tmp.teamNumber = allKarts[i].GetComponent<NetPlayerInfo>().teamNumber.Value;
                if (tmp.teamNumber == 0)
                {
                    //redTeam
                    RedTeam.Add(tmp);
                }
                else if (tmp.teamNumber == 1)
                {
                    //blueTeam
                    BlueTeam.Add(tmp);
                }
                
                PlayerDatas.Add(tmp);
            }
            Debug.Log("playerdata.count :  " + PlayerDatas.Count);
            
        }

        void Init()
        {
            if (instance == null)
            {
                GameObject im = GameObject.Find("NetItemsManager");
                if (im == null)
                {
                    im = new GameObject { name = "NetItemsManager" };
                }

                if (im.GetComponent<ItemManager>() == null)
                {
                    im.AddComponent<ItemManager>();
                }
                //DontDestroyOnLoad(im);
                instance = im.GetComponent<ItemManager>();
            }
        }

        // Return a random item from the list of items
        public Item GetRandomItem(int myRank, bool test, ulong objid) {
            if (test)
            {
                if (items.Length == 0) { return null; }
                return items[Random.Range(0, items.Length)];
            }
            else
            {
                itemName[] myItems = RandomItem(myRank);
                itemName itemName = myItems[Random.Range(0, myItems.Length)];
                Item item = null;
                if (itemName == itemName.position)
                {
                    if (objid == (int)PlayerPosition.Attack)
                    {
                        item = GetItem(itemName.BombItem.ToString());
                    }
                    else if (objid == (int)PlayerPosition.Defender)
                    {
                        item = GetItem(itemName.BufferItem_ReverseTeam.ToString());
                    }
                    else if(objid == (int)PlayerPosition.Runner)
                    {
                        item = GetItem(itemName.RushItem.ToString());
                    }
                }
                item = GetItem(itemName.ToString());
                return item;
            }
            
        }

        // Return an item of a specific type from the list if it exists
        public Item GetItem<T>() where T : Item {
            for (int i = 0; i < items.Length; i++) {
                if (items[i] is T) {
                    return items[i];
                }
            }
            return null;
        }

        // Return an item by its object name if it exists in the list
        public Item GetItem(string itemName) {
            
            for (int i = 0; i < items.Length; i++) {
                if (string.Compare(itemName, items[i].itemName, true) == 0 || string.Compare(itemName, items[i].transform.name, true) == 0) {
                    return items[i];
                }
            }
            return null;
        }


        public itemName[] RandomItem(int myRank)
        {
            itemName[] myItems= null;
            itemName[] item1 = new[]
            {
                itemName.position, itemName.FishItem, itemName.FishItem, itemName.FishItem, itemName.FishItem,
                itemName.ShieldItem, itemName.ShieldItem, itemName.SquidItem, itemName.LimitSkillItem, itemName.ShieldItem
            };
            itemName[] item2 = new[]
            {
                itemName.position,itemName.FishItem,itemName.FishItem,itemName.FishItem,itemName.FishItem,
                itemName.ShieldItem,itemName.BirdStrikeItem,itemName.HomingItem,itemName.BirdStrikeItem
            };
            itemName[] item3 = new[]
            {
                itemName.position, itemName.BirdStrikeItem, itemName.BirdStrikeItem, itemName.BirdStrikeItem,
                itemName.BirdStrikeItem,
                itemName.HomingItem, itemName.HomingItem, itemName.SquidItem, itemName.ShieldItem,
                itemName.LimitSkillItem
            };
            itemName[] item4 = new[]
            {
                itemName.position, itemName.BirdStrikeItem, itemName.BirdStrikeItem, itemName.BirdStrikeItem,
                itemName.BirdStrikeItem,
                itemName.HomingItem, itemName.HomingItem, itemName.GuardOneItem, itemName.ShieldItem,
                itemName.LimitSkillItem
            };
            itemName[] item5 = new[]
            {
                itemName.position, itemName.HomingItem, itemName.HomingItem, itemName.HomingItem, itemName.HomingItem,
                itemName.BirdStrikeItem, itemName.BirdStrikeItem, itemName.SlowItem, itemName.GuardOneItem,
                itemName.BoostItem
            };
            itemName[] item6 = new[]
            {
                itemName.position, itemName.HomingItem, itemName.HomingItem, itemName.HomingItem, itemName.HomingItem,
                itemName.BirdStrikeItem, itemName.BirdStrikeItem, itemName.SlowItem, itemName.GuardOneItem,
                itemName.BoostItem
            };
            itemName[] item7 = new[]
            {
                itemName.position, itemName.BoostItem, itemName.BoostItem, itemName.BoostItem, itemName.BoostItem,
                itemName.HomingItem, itemName.HomingItem, itemName.SquidItem, itemName.ThunderItem,
                itemName.LimitSkillItem
            };
            itemName[] item8 = new[]
            {
                itemName.position, itemName.BoostItem, itemName.BoostItem, itemName.BoostItem, itemName.BoostItem,
                itemName.HomingItem, itemName.HomingItem, itemName.BoostItem, itemName.ThunderItem,
                itemName.LimitSkillItem
            };

            switch (myRank)
            {
                case 0 :
                    myItems = item1;
                    break;
                case 1 :
                    myItems = item2;
                    break;
                case 2 :
                    myItems = item3;
                    break;
                case 3 :
                    myItems = item4;
                    break;
                case 4 :
                    myItems = item5;
                    break;
                case 5 :
                    myItems = item6;
                    break;
                case 6 :
                    myItems = item7;
                    break;
                case 7 :
                    myItems = item8;
                    break;
            }

            return myItems;
        }
        
    }

    
    public struct playerData : INetworkSerializable, System.IEquatable<playerData>
    {
        public ulong clientId;
        public ulong networkobjectId;
        public int teamNumber;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out clientId);
                reader.ReadValueSafe(out networkobjectId);
                reader.ReadValueSafe(out teamNumber);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(clientId);
                writer.WriteValueSafe(networkobjectId);
                writer.WriteValueSafe(teamNumber);
            }
        }

        public bool Equals(playerData other)
        {
            return clientId == other.clientId && networkobjectId == other.networkobjectId &&
                   teamNumber == other.teamNumber;
        }
    }
}
