using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardSo",fileName = "CardSo")]
public class CardData :  ScriptableObject
{
    [SerializeField] private Sprite characterImage;
    [SerializeField] private Sprite cardImage;
    [SerializeField] private int price;
   
    public Sprite CharacterImage { get => characterImage; }
    public Sprite CardImage { get => cardImage; }
    public int Price { get => price; }
}
