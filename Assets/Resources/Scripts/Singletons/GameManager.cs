using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton name
    public static GameManager instance;

    public event Action OnLanternCollected;
    public event Action OnPlayerRespawn;

    public int currentLanternCount;
    public PlayerController activePlayer;

    private void Awake()
    {
        //Singleton design
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("Two GameManagers detected. Deleting newest one.");
            Destroy(gameObject);
        }
    }

    public void LanternCollected(int newLanternCount)
    {
        currentLanternCount = newLanternCount;
        OnLanternCollected?.Invoke();
    }

    public void PlayerRespawn()
    {
        OnPlayerRespawn?.Invoke();
    }
}
