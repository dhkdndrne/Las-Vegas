using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BankSystem
{
	[field: SerializeField] public List<CardData> CardSoList { get; private set; } = new();

	// 돈 카드 54장(6만불, 7만불, 8만불, 9만불 각 5장씩 / 1만불, 4만불, 5만불 각 6장씩 / 2만불, 3만불 각 8장씩)
	// 총 돈 개수
	private readonly Dictionary<int, int> TOTALMONEY = new()
	{
		{ 10000, 6 }, { 20000, 8 }, { 30000, 8 }, { 40000, 6 }, { 50000, 6 }, { 60000, 5 }, { 70000, 5 }, { 80000, 5 }, { 90000, 5 }
	};

	private List<int> moneyList = new(); //돈이 들어있는 리스트 (게임에서 사용)

	public void Init()
	{
		moneyList.Clear();
		
		//딕셔너리의 모든 돈 리스트에 넣어줌
		foreach (var money in TOTALMONEY)
		{
			for (int i = 0; i < money.Value; i++)
			{
				moneyList.Add(money.Key);
			}
		}

		//돈 섞기
		for (int i = moneyList.Count - 1; i > 0; i--)
		{
			int random = Random.Range(0, i);
			(moneyList[i], moneyList[random]) = (moneyList[random], moneyList[i]);
		}
	}

	public List<int> GetRandomMoney()
	{
		List<int> list = new();

		int totalValue = 0;

		while (totalValue < 50000)
		{
			totalValue += moneyList[0];
			list.Add(moneyList[0]);
			moneyList.RemoveAt(0);
		}

		return list;
	}
}