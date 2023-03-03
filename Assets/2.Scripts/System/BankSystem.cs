using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BankSystem
{
	[SerializeField] private Transform deckposition;
	
	private readonly string MONEYCARDNAME = "MoneyCard";

	// 돈 카드 54장(6만불, 7만불, 8만불, 9만불 각 5장씩 / 1만불, 4만불, 5만불 각 6장씩 / 2만불, 3만불 각 8장씩)
	// 총 돈 개수
	private readonly Dictionary<int, int> TOTALMONEY = new()
	{
		{ 10000, 6 }, { 20000, 8 }, { 30000, 8 }, { 40000, 6 }, { 50000, 6 }, { 60000, 5 }, { 70000, 5 }, { 80000, 5 }, { 90000, 5 }
	};

	private List<Money> moneyCardList = new();
	private List<Money> usedMoneyCardList = new();

	public void Init()
	{
		int index = 0;
		//딕셔너리의 모든 돈 리스트에 넣어줌
		foreach (var money in TOTALMONEY)
		{
			for (int i = 0; i < money.Value; i++)
			{
				// 돈 카드 오브젝트 생성 및 초기화
				var card = PhotonNetwork.Instantiate(MONEYCARDNAME,Vector3.one, Quaternion.identity).GetComponent<Money>();
				card.Init(money.Key.ToString(),deckposition.position);
				moneyCardList.Add(card);
			}
			index++;
		}

		//돈 섞기
		for (int i = moneyCardList.Count - 1; i > 0; i--)
		{
			int random = Random.Range(0, i);
			(moneyCardList[i], moneyCardList[random]) = (moneyCardList[random], moneyCardList[i]);
		}
	}

	public List<Money> GetRandomMoney()
	{
		List<Money> list = new();

		int totalValue = 0;

		while (totalValue < 50000)
		{
			totalValue += moneyCardList[0].MoneyData.Price;
			list.Add(moneyCardList[0]);

			usedMoneyCardList.Add(moneyCardList[0]);
			moneyCardList.RemoveAt(0);
		}

		return list;
	}
}