using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Casino : MonoBehaviour
{
	public Queue<int> PrizeQueue { get; private set; } = new();   // 카지노에 걸린 상금 넣어둘 큐
	private Dictionary<string, int> bettingDiceDictionary = new() // 카지노에 배팅한 주사위 개수를 알기위한 딕셔너리
	{
		{ "Player 0", 0 },
		{ "Player 1", 0 },
		{ "Player 2", 0 },
		{ "Player 3", 0 },
		{ "Neutrality", 0 }
	};

	public void SetPrize(List<int> prizeList)
	{
		prizeList = prizeList.OrderByDescending(n => n).ToList();
		foreach (var prize in prizeList)
		{
			PrizeQueue.Enqueue(prize);
			UtilClass.DebugLog(prize, Define.LogType.Success);
		}

	}
}