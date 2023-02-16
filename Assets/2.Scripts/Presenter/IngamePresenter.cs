using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IngamePresenter : MonoBehaviour
{
	[SerializeField] private Button rollDiceBtn;

	public Action DiceRollAction;

	private void Start()
	{
		rollDiceBtn.onClick.AddListener(() => DiceRollAction.Invoke());

	}

	public void SetButtonActivateEvent(Player player)
	{
		player.Model.IsMyTurn.Subscribe(value =>
		{
			rollDiceBtn.gameObject.SetActive(value);
		}).AddTo(gameObject);
	}
}