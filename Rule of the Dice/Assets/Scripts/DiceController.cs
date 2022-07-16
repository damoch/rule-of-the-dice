using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour
{
    public Texture2D[] Images;
    public Image DiceImage;
    private Dictionary<int, int> NumberToTextureId;
    public int Value;
    void Start()
    {
        NumberToTextureId = new Dictionary<int, int>();
        for(int i = 1; i < 7; i++)
        {
            NumberToTextureId.Add(i, i - 1);
        }
        DiceRoll();
    }

    private void DiceRoll()
    {
        Value = UnityEngine.Random.Range(1,6);
        DiceImage.sprite = Sprite.Create(Images[Value - 1], DiceImage.sprite.rect, DiceImage.sprite.pivot);
        Debug.Log($"Dice result: {Value}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
