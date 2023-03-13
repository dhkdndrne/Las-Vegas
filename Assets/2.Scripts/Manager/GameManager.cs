using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	#region Inspector

	public TurnSystem TurnSystem { get; private set; }
	[field: SerializeField] public IngamePresenter IngamePresenter { get; private set; }
	
    #endregion
	
    #region Field

	private PhotonView pv;
	public Action InitAction;
	public bool IsGameStarted { get; private set; }
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
		TurnSystem.SetRandomTurn();

		await UniTask.Delay(3000);
		pv.RPC(nameof(TurnSystem.RPC_StartNextTurn),RpcTarget.All);
		
		pv.RPC(nameof(RPC_SetGameState),RpcTarget.All,true);
	}

	[PunRPC]
	private void RPC_SetGameState(bool value) => IsGameStarted = value;
}