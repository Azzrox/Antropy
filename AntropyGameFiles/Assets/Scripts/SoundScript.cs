using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public AudioClip clickClip;
    public AudioClip anthillUpgradeClip;
    public AudioClip tileUpgradeClip;
    public AudioClip springClip;
    public AudioClip summerClip;


    AudioSource clickSound;
    AudioSource anthillUpgradeSound;
    AudioSource tileUpgradeSound;
    AudioSource springSound;
    AudioSource summerSound;


    private void Start()
    {
        anthillUpgradeSound = gameObject.AddComponent<AudioSource>();
        anthillUpgradeSound.clip = anthillUpgradeClip;

        clickSound = gameObject.AddComponent<AudioSource>();
        clickSound.clip = clickClip;

        tileUpgradeSound = gameObject.AddComponent<AudioSource>();
        tileUpgradeSound.clip = tileUpgradeClip;

        springSound = gameObject.AddComponent<AudioSource>();
        springSound.clip = springClip;

    }


    public void playAnthillStorageUpgradeSound()
    {
        if (GameManager.Instance.resources >= GameManager.Instance.storageCost[GameManager.Instance.storageLevel])
        {
            Debug.Log("Playing Anthill upgrade sound.");

            if (anthillUpgradeSound != null)
            {
                anthillUpgradeSound.loop = false;
                anthillUpgradeSound.Play();
            }
            else
            {
                Debug.Log("anthillUpgradeSound variable is null.");
            }
        }
    }

    public void playAnthillHatcheryUpgradeSound()
    {
        if (GameManager.Instance.resources >= GameManager.Instance.hatcheryCost[GameManager.Instance.hatcheryLevel])
        {
            Debug.Log("Playing Anthill upgrade sound.");

            if (anthillUpgradeSound != null)
            {
                anthillUpgradeSound.loop = false;
                anthillUpgradeSound.Play();
            }
            else
            {
                Debug.Log("anthillUpgradeSound variable is null.");
            }
        }
    }

    public void playClickSound()
    {
        Debug.Log("Playing clicking sound.");

        if(clickSound != null)
        {
            clickSound.loop = false;
            clickSound.Play();
        }
        else
        {
            Debug.Log("clickSound variable is null.");
        }
    }


    public void playSpringSoundsOnLoop()
    {

    }

    public void playTileUpgradeSound()
    {
        Debug.Log("Playing Tile Upgrade upgrade sound.");

        if(true)
        {
            if (tileUpgradeSound != null)
            {
                AudioSource.PlayClipAtPoint(tileUpgradeClip, Vector3.zero);
            }
            else
            {
                Debug.Log("tileUpgrade variable is null.");
            }
        }
    }

}
