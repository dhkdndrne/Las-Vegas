using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IngamePresenter : MonoBehaviour
{
	[SerializeField] private Button rollDiceBtn;
	[SerializeField] private List<Transform> diceUIList = new();
	[SerializeField] private GameObject diceUI;
	[SerializeField] private GameObject myTurnTextObject;
	public Action DiceRollAction;

	private PhotonView pv;
	
	private void Start()
	{
		rollDiceBtn.onClick.AddListener(() =>
		{
			rollDiceBtn.gameObject.SetActive(false);
			DiceRollAction.Invoke();
		});

		pv = GetComponent<PhotonView>();
	}

	public void SetButtonActivateEvent(Player player)
	{
		player.Model.IsMyTurn.Subscribe(value =>
		{
			rollDiceBtn.gameObject.SetActive(value);
			myTurnTextObject.SetActive(value);
		}).AddTo(gameObject);

		player.Model.IsBettingTime.Subscribe(value =>
		{
			if(!value)
				pv.RPC(nameof(RPC_TurnOffDiceUI),RpcTarget.All);
		}).AddTo(gameObject);
	}

	public void ShowDiceUI(int dot, int diceAmount, int sDiceAmount) => pv.RPC(nameof(RPC_ShowDiceUI), RpcTarget.All, dot, diceAmount, sDiceAmount);
	
	[PunRPC]
	private void RPC_ShowDiceUI(int dot, int diceAmount, int sDiceAmount)
	{
		if (diceAmount == 0 && sDiceAmount == 0) return;
		if (!diceUI.activeSelf)
		{
			foreach (var diceUIObj in diceUIList)
			{
				for (int i = 0; i < diceUIObj.childCount;i++)
				{
					diceUIObj.transform.GetChild(i).gameObject.SetActive(false);
				}
				
				diceUIObj.gameObject.SetActive(false);
			}
			
			diceUI.SetActive(true);
		}
		
		if (!diceUIList[dot].gameObject.activeSelf) 
			diceUIList[dot].gameObject.SetActive(true);

		for (int i = 0; i < diceAmount; i++)
			diceUIList[dot].GetChild(i).gameObject.SetActive(true);
		
		for (int j = 0; j < sDiceAmount; j++)
			diceUIList[dot].GetChild(diceUIList[dot].childCount - (j + 1)).gameObject.SetActive(true);
	}

	[PunRPC]
	private void RPC_TurnOffDiceUI() => diceUI.SetActive(false);
}