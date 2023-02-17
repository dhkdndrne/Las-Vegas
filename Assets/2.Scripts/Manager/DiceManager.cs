using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using Photon.Pun;
using UnityEngine;
using UniRx;

public class DiceManager : MonoBehaviour
{
	private List<Dice> diceList = new();
	public Dictionary<int, int> DiceNumberDic { get; private set; } = new();          // 주사위 눈금 수 저장
	public Dictionary<string, int> CasinoDiceAmountDic { get; private set; } = new(); // 카지노 주사위 눈금 수 저장

	public readonly int DICE_COUNT = 8;
	private readonly string DICE_PREFAB_NAME = "Dice";
	public int RemainSpecialDice { get; private set; }
	public int SpecialDiceCount { get; private set; }

	private void Start()
	{
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
		
		for (int i = 0; i < DICE_COUNT + SpecialDiceCount; i++)
		{
			diceList.Add(PhotonNetwork.Instantiate(DICE_PREFAB_NAME, new Vector3(0, 5, 0), Quaternion.identity).GetComponent<Dice>());

			if (DICE_COUNT + SpecialDiceCount - i <= SpecialDiceCount)
				diceList[i].ChangeDiceColor(Define.DiceType.Special);

			diceList[i].SetActivate(false);
		}
	}

	public void RollDice()
	{
		Player player = GameManager.Instance.TurnSystem.NowPlayingPlayer;
		for (int i = 0; i < player.Model.Dice + player.Model.SpecialDice; i++)
		{
			diceList[i].SetActivate(true);
			diceList[i].Roll().Forget();
		}
	}
}