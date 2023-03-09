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

	public Player NowPlayingPlayer { get; private set; }

	private int playingPlayerIndex;
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
		}

		//마스터가 섞인 순서를 다른 플레이어들에게 알려줌
		pv.RPC(nameof(RPC_SetTurnQueue), RpcTarget.Others, viewIDArr);
		pv.RPC(nameof(RPC_InitPlayers), RpcTarget.All);
	}
	public void StartNextTurn()
	{
		NowPlayingPlayer = PlayerList[playingPlayerIndex];
		NowPlayingPlayer.PV.RPC(nameof(NowPlayingPlayer.RPC_StartMyTurn), RpcTarget.All);

		playingPlayerIndex = playingPlayerIndex + 1 >= PlayerList.Count - 1 ? 0 : playingPlayerIndex + 1;
	}

	[PunRPC]
	private void RPC_SetTurnQueue(int[] viewIDArr)
	{
		List<Player> tempList = new();

		for (int i = 0; i < viewIDArr.Length; i++)
		{
			foreach (var player in PlayerList)
			{
				if (player.GetComponent<PhotonView>().ViewID == viewIDArr[i])
				{
					tempList.Add(player);
					break;
				}
			}
		}

		PlayerList = tempList;
	}

	[PunRPC]
	private void RPC_InitPlayers()
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			PlayerList[i].InitPlayer(i);
		}
	}
}