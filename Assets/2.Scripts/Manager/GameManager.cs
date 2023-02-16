using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	#region Inspector

	public TurnSystem TurnSystem { get; private set; }
	[field: SerializeField] public IngamePresenter IngamePresenter { get; private set; }
	
    #endregion
	
    #region Field

	private PhotonView pv;

    #endregion
	
	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		TurnSystem = GetComponent<TurnSystem>();
	}

	/// <summary>
	/// Master Client만 실행
	/// </summary>
	public void InitGame()
	{
		DiceManager.Instance.InitDice();
		
		TurnSystem.SetRandomTurn();
		
		TurnSystem.StartNextTurn();
		//카지노 가격 설정
	}
}