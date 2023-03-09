using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngamePresenter : MonoBehaviour
{
	[SerializeField] private Button rollDiceBtn;
	[SerializeField] private List<Transform> diceUIList = new();
	[SerializeField] private GameObject diceUI;
	[SerializeField] private GameObject myTurnTextObject;

	[SerializeField] private TextMeshProUGUI diceAmountText;
	[SerializeField] private TextMeshProUGUI sDiceAmountText;

	[Header("카지노 정보창")]
	[SerializeField] private Transform casinoInfoObject;
	[SerializeField] private TextMeshProUGUI casinoNumText;
	[SerializeField] private TextMeshProUGUI prizeText;

	[field: SerializeField] public List<TextMeshProUGUI> PlayersPrizeList { get; private set; } = new();

	public Action DiceRollAction;

	private PhotonView pv;

	private ReactiveProperty<Casino> selectedCasino = new();
	private bool isPanelMoved;
	private float elapse;

	private void Start()
	{
		pv = GetComponent<PhotonView>();

		rollDiceBtn.onClick.AddListener(() =>
		{
			rollDiceBtn.gameObject.SetActive(false);
			DiceRollAction.Invoke();
		});

		// 매프레임 체크하면서 카지노 위에 마우스가 올라갔는지 체크 후 카지노에 마우스가 올라가있으면 카지노 정보 출력
		Observable.EveryUpdate().Where(_ =>GameManager.Instance.IsGameStarted).Subscribe(_ =>
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);

			foreach (var hit in hits)
			{
				if (!EventSystem.current.IsPointerOverGameObject() && hit.transform.TryGetComponent(out Casino casino))
				{
					if (casino != selectedCasino.Value)
					{
						selectedCasino.Value = casino;
						casinoInfoObject.transform.DOMoveX(0, .5f);
					}
					isPanelMoved = true;
					elapse = 0;
					break;
				}

				isPanelMoved = false;
			}

			if (!isPanelMoved && selectedCasino.Value != null)
			{
				elapse += Time.deltaTime;

				if (elapse >= 0.3f)
				{
					selectedCasino.Value = null;
					casinoInfoObject.transform.DOMoveX(-600, .5f);
					elapse = 0f;
				}
			}
		}).AddTo(gameObject);

		selectedCasino.Where(value => value != null).Subscribe(value =>
		{
			casinoNumText.text = $"카지노 {value.CasinoNum}";

			string text = String.Empty;
			for (int i = 0; i < value.PrizeList.Count; i++)
			{
				text += $"{i + 1}등 상금 : {value.PrizeList[i].MoneyData.Price} \n";
			}
			prizeText.text = text;
			
		}).AddTo(gameObject);
	}
	
	public void InitUIEvent(Player player)
	{
		player.Model.IsMyTurn.Subscribe(value =>
		{
			rollDiceBtn.gameObject.SetActive(value);
			myTurnTextObject.SetActive(value);
		}).AddTo(gameObject);

		player.Model.IsBettingTime.Subscribe(value =>
		{
			if (!value)
				pv.RPC(nameof(RPC_TurnOffDiceUI), RpcTarget.All);
		}).AddTo(gameObject);

		player.Model.Dice.Subscribe(value =>
		{
			diceAmountText.text = "나의 주사위 개수 :" + value;
		}).AddTo(gameObject);

		player.Model.SpecialDice.Subscribe(value =>
		{
			sDiceAmountText.text = "나의 특수 주사위 개수 : " + value;
		}).AddTo(gameObject);

		// 플레이어 상금 텍스트 오브젝트 켜줌
		// 플레이어들의 상금 텍스트 표시
		PlayersPrizeList[player.Model.PlayerNumber].gameObject.SetActive(true);
		player.Model.Money.Subscribe(value =>
		{
			PlayersPrizeList[player.Model.PlayerNumber].text = $"플레이어{player.Model.PlayerNumber} 상금 : {value}";
		}).AddTo(gameObject);
	}

	public void ShowDiceUI(int dot, int diceAmount, int sDiceAmount) => pv.RPC(nameof(RPC_ShowDiceUI), RpcTarget.All, dot, diceAmount, sDiceAmount);

	[PunRPC]
	private void RPC_ShowDiceUI(int dot, int diceAmount, int sDiceAmount)
	{
		if (diceAmount == 0 && sDiceAmount == 0) return;
		if (!diceUI.activeSelf)
		{
			foreach (var diceUIObj in diceUIList)
			{
				for (int i = 0; i < diceUIObj.childCount; i++)
				{
					diceUIObj.transform.GetChild(i).gameObject.SetActive(false);
				}

				diceUIObj.gameObject.SetActive(false);
			}

			diceUI.SetActive(true);
		}

		if (!diceUIList[dot].gameObject.activeSelf)
			diceUIList[dot].gameObject.SetActive(true);

		for (int i = 0; i < diceAmount; i++)
			diceUIList[dot].GetChild(i).gameObject.SetActive(true);

		for (int j = 0; j < sDiceAmount; j++)
			diceUIList[dot].GetChild(diceUIList[dot].childCount - (j + 1)).gameObject.SetActive(true);
	}

	[PunRPC]
	private void RPC_TurnOffDiceUI() => diceUI.SetActive(false);
}