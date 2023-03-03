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
		TurnSystem.StartNextTurn();
	}

}