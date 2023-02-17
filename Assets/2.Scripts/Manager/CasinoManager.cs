using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CasinoManager : MonoBehaviour
{
	private PhotonView pv;

	private BankSystem bankSystem = new();

	[field: SerializeField] public Casino[] Casinos { get; private set; } = new Casino[6];

	private void Start()
	{
		GameManager.Instance.InitAction += bankSystem.Init;
		GameManager.Instance.InitAction += SetCasinoPrices;
	}

	public void SetCasinoPrices()
	{
		foreach (var casino in Casinos)
		{
			casino.SetPrize(bankSystem.GetRandomMoney());
			UtilClass.DebugLog("-----------------",Define.LogType.LogError);
		}
	}

}