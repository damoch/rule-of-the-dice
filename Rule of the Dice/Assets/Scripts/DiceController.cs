using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour
{
    public AudioManager AM;
    public Texture2D[] Images;
    public Image DiceImage;
    private Dictionary<int, int> NumberToTextureId;
    public int Value;
    public int NumberOfThrowsBeforeValueIsDetermined = 20;
    [Range(0,1)]
    public float FirstRollPauseSeconds = 0.1f;
    [Range(0, 1)]
    public float LastRollPauseSeconds = 1f;
    public GameController GameController;

    private float _currentPause;
    private float _pauseIncrease;
    private float _pauseElapsed;
    private int _currentRoll = 0;

    private bool _isRolling;
    void Start()
    {
        GameController.RegisterDice(this);
        _isRolling = false;

        NumberToTextureId = new Dictionary<int, int>();
        for(int i = 1; i < 7; i++)
        {
            NumberToTextureId.Add(i, i - 1);
        }
        _pauseIncrease = (LastRollPauseSeconds - FirstRollPauseSeconds) / NumberOfThrowsBeforeValueIsDetermined;
    }

    public void BeginDiceRoll()
    {
        _isRolling = true;
        _currentRoll = 0;
        _currentPause = FirstRollPauseSeconds;
    }

    private void EndDiceRoll()
    {
        _isRolling = false;
        Debug.Log($"Dice result: {Value}");
        GameController.NotifyDicerollResult(Value, this);
    }

    private void DiceRoll()
    {
        AM.OnDiceRoll();
        Value = UnityEngine.Random.Range(1,6);
        DiceImage.sprite = Sprite.Create(Images[Value - 1], DiceImage.sprite.rect, DiceImage.sprite.pivot);
        _pauseElapsed = 0;
        _currentPause += _pauseIncrease;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRolling)
        {
            _pauseElapsed += Time.deltaTime;
            if(_pauseElapsed >= _currentPause)
            {
                _currentRoll++;
                DiceRoll();
                if (_currentRoll >= NumberOfThrowsBeforeValueIsDetermined)
                {
                    EndDiceRoll();
                }
            }

        }
    }
}
