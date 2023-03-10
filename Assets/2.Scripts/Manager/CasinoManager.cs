using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Bam.Singleton;
public class CasinoManager : Singleton<CasinoManager>
{
	private BankSystem bankSystem;

	[field: SerializeField] public Casino[] Casinos { get; private set; } = new Casino[6];

	private PhotonView pv;

	private void Start()
	{
		pv = GetComponent<PhotonView>();
		bankSystem = GetComponent<BankSystem>();

		GameManager.Instance.InitAction += bankSystem.Init;
		GameManager.Instance.InitAction += () => pv.RPC(nameof(RPC_InitCasino), RpcTarget.All);
	}
	
	[PunRPC]
	private void RPC_InitCasino()
	{
		foreach (var casino in Casinos)
		{
			casino.SetPrize(bankSystem.GetPrizeList());
		}
	}
}