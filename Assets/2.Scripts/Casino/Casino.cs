using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class Casino : MonoBehaviour
{

	[SerializeField] private Transform moneyPos; //돈 놓을 위치
	[field: SerializeField] public int CasinoNum { get; private set; }

	public List<Money> PrizeList { get; private set; } = new();   // 카지노에 걸린 상금 넣어둘 큐
	private Dictionary<string, int> bettingDiceDictionary = new() // 카지노에 배팅한 주사위 개수를 알기위한 딕셔너리
	{
		{ "Player 0", 0 },
		{ "Player 1", 0 },
		{ "Player 2", 0 },
		{ "Player 3", 0 },
		{ "Special", 0 }
	};
	public List<KeyValuePair<string, int>> SortedList { get; private set; }
	
	private PhotonView pv;
	public PhotonView PV => pv;

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		RemovePlayerKeyAndInitList().Forget();
	}

	private async UniTaskVoid RemovePlayerKeyAndInitList()
	{
		await UniTask.WaitUntil(() => GameManager.Instance.IsGameStarted);

		var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		for (int i = 4; i >= playerCount; i--)
		{
			bettingDiceDictionary.Remove($"Player {i}");
		}
	}
	
	public void SetPrize(List<Money> prizeList)
	{
		prizeList = prizeList.OrderByDescending(n => n.MoneyData.Price).ToList();

		Vector3 v3TargetPos = moneyPos.position;
		Vector3 v3Offset = new Vector3(0, 0, -3.285f);
		int index = 0;

		foreach (var prize in prizeList)
		{
			PrizeList.Add(prize);
			prize.ChangeSortingOrder(index);
			prize.MoveToPostition(v3TargetPos + (v3Offset * index++));
		}
	}
	
	[PunRPC]
	public void RPC_BetDice(string playerID, int diceAmount, int sDiceAmount)
	{
		bettingDiceDictionary[playerID] += diceAmount;
		bettingDiceDictionary["Special"] += sDiceAmount;
		
		UtilClass.DebugLog($"{playerID} 배팅 개수 : {bettingDiceDictionary[playerID]}");
		UtilClass.DebugLog($"Special 배팅 개수 : {bettingDiceDictionary["Special"]}");

		SortedList = new List<KeyValuePair<string, int>>(bettingDiceDictionary);
		SortedList.Sort((x, y) => y.Value.CompareTo(x.Value));
	}
}