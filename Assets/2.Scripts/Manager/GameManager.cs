using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IPunObservable
{
	#region Inspector

	public TurnSystem TurnSystem { get; private set; }
	[field: SerializeField] public IngamePresenter IngamePresenter { get; private set; }

    #endregion

    #region Field

	private PhotonView pv;
	public Action InitAction;
	public bool IsGameStarted { get; private set; }
	public int round;

	#endregion

	protected override void Awake()
	{
		base.Awake();
		pv = GetComponent<PhotonView>();
		TurnSystem = GetComponent<TurnSystem>();
	}

	/// <summary>
	/// Master Client만 실행
	/// </summary>
	public async UniTaskVoid InitGame()
	{
		InitAction?.Invoke();
		CasinoManager.Instance.PV.RPC(nameof(CasinoManager.Instance.RPC_InitCasino), RpcTarget.All);

		TurnSystem.SetRandomTurn();

		await UniTask.Delay(3000);
		pv.RPC(nameof(TurnSystem.RPC_StartNextTurn), RpcTarget.MasterClient);
		pv.RPC(nameof(RPC_SetGameState), RpcTarget.All, true);

		round++;
	}

	[PunRPC]
	private void RPC_SetGameState(bool value) => IsGameStarted = value;

	public async UniTaskVoid StartNextRound()
	{
		round++;
		if (round > 1)
		{
			UtilClass.DebugLog("게임 끝났땅");
			GameOver();
		}
		else
		{

			CasinoManager.Instance.PV.RPC(nameof(CasinoManager.Instance.RPC_InitCasino), RpcTarget.All);

			await UniTask.Delay(3000);
			pv.RPC(nameof(TurnSystem.RPC_SetNextRoundTurn), RpcTarget.MasterClient);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(round);
		}
		else
		{
			round = (int)stream.ReceiveNext();
		}
	}

	private void GameOver()
	{
		TurnSystem.PlayerList.Sort((p1, p2) => p1.Model.Money.Value < p2.Model.Money.Value ? 1 : -1);

		int rank = 1;
		foreach (var p in TurnSystem.PlayerList)
		{
			UtilClass.DebugLog($"{rank}등 = {p.Model.PlayerNumber} / {p.Model.Money}");
		}
	}
}