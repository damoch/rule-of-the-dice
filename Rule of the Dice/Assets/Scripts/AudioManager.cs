using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip Discard;
    public AudioClip BeginDiceroll;
    public AudioClip DiceRoll;
    public AudioClip RollSuccess;
    public AudioClip RollFailed;
    public AudioClip AddCardToQueue;
    public AudioSource MainSource;

    public void OnBeginDiceRoll()
    {
        MainSource.clip = BeginDiceroll;
        MainSource.Play();
    }

    public void OnDiceRoll()
    {
        MainSource.clip = DiceRoll;
        MainSource.Play();
    }

    public void PlayAddCard()
    {
        MainSource.clip = AddCardToQueue;
        MainSource.Play();
    }

    public void PlayRollSuccess()
    {
        MainSource.clip = RollSuccess;
        MainSource.Play();
    }

    public void PlayRollFailed()
    {
        MainSource.clip = RollFailed;
        MainSource.Play();
    }
    
    public void PlayDiscard()
    {
        MainSource.clip = Discard;
        MainSource.Play();
    }
}
