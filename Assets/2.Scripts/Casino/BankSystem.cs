using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankSystem : MonoBehaviour
{
	[field: SerializeField] public List<CardData> CardSoList { get; private set; } = new();

	// 돈 카드 54장(6만불, 7만불, 8만불, 9만불 각 5장씩 / 1만불, 4만불, 5만불 각 6장씩 / 2만불, 3만불 각 8장씩)
	// 총 돈 개수
	public Dictionary<int, int> MoneyDic { get; private set; } = new()
	{
		{ 10000, 6 }, { 20000, 8 }, { 30000, 8 }, { 40000, 6 }, { 50000, 6 }, { 60000, 5 }, { 70000, 5 }, { 80000, 5 }, { 90000, 5 }
	};
	
}