using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casino : MonoBehaviour
{
	public Queue<int> Prizes { get; private set; } = new();	// 카지노에 걸린 상금 넣어둘 큐
	private Dictionary<string, int> bettingDiceDictionary = new()	// 카지노에 배팅한 주사위 개수를 알기위한 딕셔너리
	{
		{ "Player 0", 0 },
		{ "Player 1", 0 },
		{ "Player 2", 0 },
		{ "Player 3", 0 },
		{ "Neutrality", 0 }
	};
}