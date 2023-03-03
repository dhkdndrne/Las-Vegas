using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
	private PhotonView pv;

	private List<Dice> diceList = new();
	private List<UniTask<(int, Define.DiceType)>> rollResultList = new();
	public Dictionary<int, int> DiceNumberDic { get; private set; } = new()
	{
		{ -1, 0 }, { -2, 0 }, { -3, 0 }, { -4, 0 }, { -5, 0 }, { -6, 0 },
		{ 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }
	};                                                                                // 주사위 눈금 수 저장 (음수는 중립 주사위)
	public Dictionary<string, int> CasinoDiceAmountDic { get; private set; } = new(); // 카지노 주사위 눈금 수 저장

	public readonly int DICE_COUNT = 8;
	private readonly string DICE_PREFAB_NAME = "Dice";

	public int RemainSpecialDice { get; private set; }
	public int SpecialDiceCount { get; private set; }
	
	private void Start()
	{
		pv = GetComponent<PhotonView>();
		GameManager.Instance.InitAction += InitDice;
	}

	private void InitDice()
	{
		//	두명이서 플레이하면 중립주사위 4개
		//	3~4명이서 게임하면 중립 주사위 2개씩
		//	3명이서 플레이하면 중립주사위 2개는 눈금에 맞는 카지노에 넣음

		int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		SpecialDiceCount = playerCount == 2 ? 4 : 2;
		RemainSpecialDice = playerCount == 3 ? 2 : 0;

		pv.RPC(nameof(RPC_RefreshSpecialDice), RpcTarget.Others, SpecialDiceCount, RemainSpecialDice);

		for (int i = 0; i < DICE_COUNT + SpecialDiceCount; i++)
		{
			diceList.Add(PhotonNetwork.Instantiate(DICE_PREFAB_NAME, new Vector3(0, 5, 0), Quaternion.identity).GetComponent<Dice>());

			if (DICE_COUNT + SpecialDiceCount - i <= SpecialDiceCount)
				diceList[i].ChangeDiceColor(Define.DiceType.Special);

			diceList[i].SetActivate(false);
		}
	}

	public async UniTaskVoid RollDice()
	{
		Player player = GameManager.Instance.TurnSystem.NowPlayingPlayer;
		rollResultList.Clear();

		for (int i = 0; i < player.Model.Dice + player.Model.SpecialDice; i++)
		{
			diceList[i].SetActivate(true);
			rollResultList.Add(diceList[i].Roll());
		}

		var diceResultList = await UniTask.WhenAll(rollResultList);

		foreach (var value in diceResultList)
		{
			int dot = value.Item2 == Define.DiceType.Special ? value.Item1 * -1 : value.Item1;
			DiceNumberDic[dot]++;
		}

		for (int i = 0; i < 6; i++)
		{
			GameManager.Instance.IngamePresenter.ShowDiceUI(i, DiceNumberDic[i + 1], DiceNumberDic[-(i + 1)]);
			pv.RPC(nameof(RPC_RefreshRolledDice), RpcTarget.Others, i + 1, DiceNumberDic[i + 1]);
			pv.RPC(nameof(RPC_RefreshRolledDice), RpcTarget.Others, -(i + 1), DiceNumberDic[-(i + 1)]);
		}

		player.PV.RPC(nameof(player.RPC_BettingTime), RpcTarget.All);
	}

	[PunRPC]
	private void RPC_RefreshRolledDice(int key, int value)
	{
		DiceNumberDic[key] = value;
	}

	[PunRPC]
	private void RPC_RefreshSpecialDice(int sDiceAmount, int remainsDiceAmount)
	{
		SpecialDiceCount = sDiceAmount;
		RemainSpecialDice = remainsDiceAmount;
	}
}