using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CasinoManager : MonoBehaviour
{
	private PhotonView pv;
	public PhotonView PV { get => pv; }

	private BankSystem bankSystem;
	
	public Casino[] Casinos { get;private set; } = new Casino[6];


	public void SetCasinoPrices()
	{
		foreach (var casino in Casinos)
		{
			casino.SetPrice();
		}
	}
	
}