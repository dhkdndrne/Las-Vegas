using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Player : MonoBehaviourPun
{
	private PhotonView pv;
	public PlayerModel Model { get; private set; } = new();
	
	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		
		GameManager.Instance.TurnSystem.PlayerList.Add(this);
		GameManager.Instance.IngamePresenter.DiceRollAction += RollDice;
		
		this.UpdateAsObservable().Where(_ => Model.IsMyTurn.Value).Subscribe(_ =>
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.transform.TryGetComponent(out Casino casino))
					{
						//주사위 배팅 넣자
					}
				}
			}
		}).AddTo(gameObject);
	}

	public void InitPlayer(int specialDiceAmount)
	{
		Model.ResetDice(specialDiceAmount);
		GameManager.Instance.IngamePresenter.SetButtonActivateEvent(this);
	}

	private void RollDice()
	{
		if (pv.IsMine && Model.IsMyTurn.Value)
		{
			pv.RPC(nameof(RPC_RollDice),RpcTarget.MasterClient);
		}
	}

	public void StartMyTurn() => pv.RPC(nameof(RPC_StartMyTurn), RpcTarget.All);
	public void EndMyTurn() =>pv.RPC(nameof(RPC_EndMyTurn), RpcTarget.All);
	
	[PunRPC]
	private void RPC_StartMyTurn()
	{
		if (pv.IsMine)
			Model.IsMyTurn.Value = true;
		
	}

	[PunRPC]
	private void RPC_EndMyTurn()
	{
		if (pv.IsMine)
			Model.IsMyTurn.Value = false;
	}
	
	/// <summary>
	/// 마스터에게 주사위 굴리는거 요청
	/// </summary>
	[PunRPC]
	private void RPC_RollDice() => DiceManager.Instance.RollDice();

}