using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Serializable]
public class PlayerModel
{
	private static int DICE_COUNT = 8;

	/*
	 *	두명이서 플레이하면 중립주사위 4개
	 *	3~4명이서 게임하면 중립 주사위 2개씩
	 *	3명이서 플레이하면 중립주사위 2개는 눈금에 맞는 카지노에 넣음
	 */
	public int Dice { get; private set; }
	public int SpecialDice { get; private set; }

	public int money;
	public ReactiveProperty<bool> IsMyTurn { get; private set; } = new();
	public ReactiveProperty<bool> IsBettingTime { get; private set; } = new();
 
	public void ResetDice(int specialDiceAmount)
	{
		Dice = DICE_COUNT;
		SpecialDice = specialDiceAmount;
	}

	public void UpdateDiceAmount(int diceAmount, int sDiceAmount)
	{
		Dice -= diceAmount;
		SpecialDice -= sDiceAmount;
		
		UtilClass.DebugLog($"Dice 개수 : {Dice}");
		UtilClass.DebugLog($"sDice 개수 : {SpecialDice}");
	}
}
