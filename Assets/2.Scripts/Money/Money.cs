using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
	[SerializeField] private GameObject cardFront;
	[SerializeField] private GameObject cardBack;

	[SerializeField] private Image charImage;
	[SerializeField] private Image bgImage;
	[SerializeField] private TextMeshProUGUI priceText;

	private PhotonView pv;
	public MoneyData MoneyData { get; private set; }
	public Canvas canv;
	
	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		canv = GetComponentInChildren<Canvas>();
	}

	public void Init(string moneyID, Vector3 v3Pos) => pv.RPC(nameof(Init_RPC), RpcTarget.All, moneyID, v3Pos);


	[PunRPC]
	private void Init_RPC(string moneyID, Vector3 v3Pos)
	{
		MoneyData = Resources.Load<MoneyData>(moneyID);

		charImage.sprite = MoneyData.CharacterImage;
		bgImage.sprite = MoneyData.CardImage;
		priceText.text = MoneyData.Price.ToString();

		transform.SetPositionAndRotation(v3Pos, Quaternion.Euler(-90f, 90f, 90f));
		cardBack.SetActive(true);
	}

	public void MoveToPostition(Vector3 v3TargetPos) => pv.RPC(nameof(CardMove_RPC),RpcTarget.All,v3TargetPos);
	
	[PunRPC]
	private async UniTaskVoid CardMove_RPC(Vector3 v3TargetPos)
	{
		 await UniTask.Delay(1000);
		 await transform.DOMove(v3TargetPos, 1f);
		 await transform.DORotate(new Vector3(0,90,90), 0.5f).OnComplete(() =>
		 {
		 	cardFront.SetActive(true);
		 	cardBack.SetActive(false);
		 });
		await transform.DORotate(new Vector3(90, 90, 90), 0.5f);
	}

	public void ChangeSortingOrder(int order) => pv.RPC(nameof(ChangeSoringOrder_RPC), RpcTarget.All,order);

	[PunRPC]
	private void ChangeSoringOrder_RPC(int order)
	{
		canv.sortingOrder = order;
	}
}