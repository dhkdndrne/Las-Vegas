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
	private DiceManager diceManager;

	public PhotonView PV => pv;
	public PlayerModel Model { get; private set; } = new();

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		diceManager = FindObjectOfType<DiceManager>();

		GameManager.Instance.TurnSystem.PlayerList.Add(this);
		GameManager.Instance.IngamePresenter.DiceRollAction += RollDice;

		this.UpdateAsObservable().Where(_ => pv.IsMine && Model.IsMyTurn.Value && Model.IsBettingTime.Value).Subscribe(_ =>
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits = Physics.RaycastAll(ray);

				foreach (var hit in hits)
				{
					if (hit.transform.TryGetComponent(out Casino casino))
					{
						int casinoNum = casino.CasinoNum;
						string playerID = PhotonNetwork.LocalPlayer.NickName;

						//주사위가 없으면 리턴
						if (diceManager.DiceNumberDic[casinoNum] == 0 && diceManager.DiceNumberDic[-casinoNum] == 0) return;

						//카지노에 주사위 베팅
						casino.PV.RPC(nameof(casino.RPC_BetDice), RpcTarget.All, playerID, diceManager.DiceNumberDic[casinoNum], diceManager.DiceNumberDic[-casinoNum]);

						//주사위 개수 업데이트
						pv.RPC(nameof(RPC_UpdateDiceAmount), RpcTarget.All, diceManager.DiceNumberDic[casinoNum], diceManager.DiceNumberDic[-casinoNum]);

						//베팅 끝
						Model.IsBettingTime.Value = false;
					}
				}
			}
		}).AddTo(gameObject);
	}

	public void InitPlayer(int playerNumber)
	{
		Model.InitModel(playerNumber,diceManager.SpecialDiceCount);
		GameManager.Instance.IngamePresenter.InitUIEvent(this);
	}

	private void RollDice()
	{
		if (pv.IsMine && Model.IsMyTurn.Value)
		{
			pv.RPC(nameof(RPC_RollDice), RpcTarget.MasterClient);
		}
	}

	[PunRPC]
	private void RPC_UpdateDiceAmount(int diceAmount, int sDiceAmount)
	{
		if (pv.IsMine)
			Model.UpdateDiceAmount(diceAmount, sDiceAmount);
	}

	[PunRPC]
	public void RPC_StartMyTurn()
	{
		if (pv.IsMine)
			Model.IsMyTurn.Value = true;
	}

	/// <summary>
	/// 마스터에게 주사위 굴리는거 요청
	/// </summary>
	[PunRPC]
	private void RPC_RollDice() => diceManager.RollDice().Forget();

	[PunRPC]
	public void RPC_BettingTime() => Model.IsBettingTime.Value = true;

	[PunRPC]
	public void RPC_GetMoney(int money) => Model.Money.Value += money;
}