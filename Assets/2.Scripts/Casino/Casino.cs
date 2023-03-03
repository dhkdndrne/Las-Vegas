using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Casino : MonoBehaviour
{

	[SerializeField] private Transform moneyPos; //돈 놓을 위치
	[field: SerializeField] public int CasinoNum { get; private set; }

	public Queue<Money> PrizeQueue { get; private set; } = new(); // 카지노에 걸린 상금 넣어둘 큐
	private Dictionary<string, int> bettingDiceDictionary = new() // 카지노에 배팅한 주사위 개수를 알기위한 딕셔너리
	{
		{ "Player 0", 0 },
		{ "Player 1", 0 },
		{ "Player 2", 0 },
		{ "Player 3", 0 },
		{ "Special", 0 }
	};

	private PhotonView pv;
	public PhotonView PV => pv;

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
	}

	public void SetPrize(List<Money> prizeList)
	{
		prizeList = prizeList.OrderByDescending(n => n.MoneyData.Price).ToList();

		Vector3 v3TargetPos = moneyPos.position;
		Vector3 v3Offset = new Vector3(0, 0, -3.285f);
		int index = 0;

		foreach (var prize in prizeList)
		{
			PrizeQueue.Enqueue(prize);
			prize.ChangeSortingOrder(index);
			prize.MoveToPostition(v3TargetPos + (v3Offset * index++));
		}
	}

	[PunRPC]
	public void RPC_BetDice(string playerID, int diceAmount, int sDiceAmount)
	{
		bettingDiceDictionary[playerID] += diceAmount;
		bettingDiceDictionary["Special"] += sDiceAmount;
	}
}