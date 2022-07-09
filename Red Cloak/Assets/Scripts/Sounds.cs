using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioSource CoinSound;
    public AudioSource GameOver;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            CoinSound.Play();
        }

        //if(GameManager.I.isPlay == true && GameManager.I.isDead == false)
        {
            if (other.tag == "Enemy" || other.tag == "KillingPlant")
            {
                GameOver.Play();
            }
        }
        

    }
}
