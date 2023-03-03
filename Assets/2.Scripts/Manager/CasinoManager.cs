using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Bam.Singleton;
public class CasinoManager : Singleton<CasinoManager>
{
	[SerializeField] private BankSystem bankSystem = new();

	[field: SerializeField] public Casino[] Casinos { get; private set; } = new Casino[6];
	
	private void Start()
	{
		GameManager.Instance.InitAction += bankSystem.Init;
		GameManager.Instance.InitAction += InitCasino;
	}

	public void InitCasino()
	{
		foreach (var casino in Casinos)
		{
			casino.SetPrize(bankSystem.GetRandomMoney());
		}
	}
}