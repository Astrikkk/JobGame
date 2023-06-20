using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int Money;
    public static int HightScore;
    public static int Score;

    public GameObject MainMenu;

    public static bool IsGameStarted = true;


    public void StartGame()
    {
        IsGameStarted = true;
    }
}
