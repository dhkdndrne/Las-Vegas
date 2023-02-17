using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnSystem : MonoBehaviour
{
    public List<Player> PlayerList { get; private set; } = new();
    public Queue<Player> TurnQueue { get; private set; } = new();

    public Player NowPlayingPlayer { get; private set; }
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void SetRandomTurn()
    {
        for (int i = 0; i < 100; i++)
        {
            int index1 = Random.Range(0, PlayerList.Count);
            int index2 = Random.Range(0, PlayerList.Count);

            (PlayerList[index1], PlayerList[index2]) = (PlayerList[index2], PlayerList[index1]);
        }

        int[] viewIDArr = new int[PlayerList.Count];
        
        for (int i = 0; i < PlayerList.Count; i++)
        {
            viewIDArr[i] = PlayerList[i].GetComponent<PhotonView>().ViewID;
            TurnQueue.Enqueue(PlayerList[i]);
        }
        
        //마스터가 섞인 순서를 다른 플레이어들에게 알려줌
        pv.RPC(nameof(RPC_SetTurnQueue),RpcTarget.Others,viewIDArr);
        pv.RPC(nameof(RPC_InitPlayers),RpcTarget.All);
    }
    public void StartNextTurn()
    {
        NowPlayingPlayer = TurnQueue.Dequeue();
        NowPlayingPlayer.StartMyTurn();
    }

    /// <summary>
    /// 플레이어 순서 담겨있는 큐 동기화
    /// </summary>
    /// <param name="playerQueue"></param>
    [PunRPC]
    private void RPC_SetTurnQueue(int[] viewIDArr)
    {
        for (int i = 0; i < viewIDArr.Length; i++)
        {
            foreach (var player in PlayerList)
            {
                if (player.GetComponent<PhotonView>().ViewID == viewIDArr[i])
                {
                    TurnQueue.Enqueue(player);
                    break;
                }
            }
        }
    }

    [PunRPC]
    private void RPC_InitPlayers()
    {
        foreach (var player in PlayerList)
        {
            player.InitPlayer();
        }
    }
}
