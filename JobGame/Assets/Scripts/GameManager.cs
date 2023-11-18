using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject JumpButton;
    public GameObject GameOverMenu;
    public GameObject WatchVideoMenu;
    public GameObject NewRecordBar;
    public GameObject BestScoreBar;
    public GameObject ScoreBar;
    public GameObject NameBar;
    public GameObject ContinueButton;
    public ParticleSystem UpgradeParticle;
    public static bool IsGameStarted = false;
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI Button1;
    public TextMeshProUGUI Button2;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI BestScoreText;
    public TextMeshProUGUI BestScoreTextMenu;
    public TextMeshProUGUI ScoreTextGame;

    public static int Score;
    public static int BestScore;
    public static float Money;
    public static int JumpLvl = 1;
    public static int JumpUpgrade;
    public static int IncomeLvl = 1;
    public static int IncomeUpgrade;

    public static int Income = 1;

    private float lastTimeScale;

    private bool CanContinue = true;
    private TimeScaleController timeScaleController;
    public bool IsOnPause = false;

    public GameObject player;
    public GameObject MainCamera;
    public Transform playerTransform;
    public Transform cameraTransform;
    private Vector3 initialPlayerPosition;
    private Vector3 initialCameraPosition;


    private void Start()
    {
        LoadData();
        IsGameStarted = false;
        if (IncomeUpgrade < 50) IncomeUpgrade = 50;
        if (JumpUpgrade < 50) JumpUpgrade = 50;
        WatchVideoMenu.SetActive(false);
        if (Income <= 1) Income = 1;
        timeScaleController = FindObjectOfType<TimeScaleController>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        cameraTransform = MainCamera.transform;
        playerTransform.position = player.transform.position;
        cameraTransform.position = MainCamera.transform.position;
        initialPlayerPosition = playerTransform.position;
        initialCameraPosition = cameraTransform.position;
        IsOnPause = false;
    }

    public void GoToMainMenu()
    {
        IsOnPause = false;
        playerTransform.position = initialPlayerPosition;
        cameraTransform.position = initialCameraPosition;
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Platform");
        for (int i = 0; i < bricks.Length; i++)
        {
            Destroy(bricks[i]);
        }
        Score = 0;
        IsGameStarted = false;
        MainMenu.SetActive(true);
        BestScoreBar.SetActive(true);
        ScoreBar.SetActive(false);
        NameBar.SetActive(true);
        JumpButton.SetActive(false);
        GameOverMenu.SetActive(false);
        player.GetComponent<PlayerController>().StopAllCoroutines();
        player.GetComponent<PlayerController>().blockSpeed = 3;
        player.GetComponent<PlayerController>().blockSpawnRate = 3;
        player.GetComponent<PlayerController>().PlusHeightLeft = 0.5f;
        player.GetComponent<PlayerController>().PlusHeightRight = 0.5f;
        player.GetComponent<PlayerController>().BlockPlace = true;
        player.GetComponent<PlayerController>().firstBlock = true;
        player.GetComponent<PlayerController>().firstBlockPlace = true;
        player.GetComponent<PlayerController>().firsJump = true;
        Time.timeScale = 1.3f;
    }


    public void WatchVideoAndContinue(PlayerController player)
    {
        ///whatch video code
        if (CanContinue)
        {
            player.Invisibility();
            player.Respawn();
            if (lastTimeScale >= 2) lastTimeScale -= 0.3f;
            Time.timeScale = lastTimeScale;
            timeScaleController.StartIncreasing();
            CanContinue = false;
            IsOnPause = false;
            ScoreBar.SetActive(true);
            JumpButton.SetActive(true);
            GameOverMenu.SetActive(false);
            Debug.Log("Continue");
        }
    }

    public static void DeleteAllData()
    {
        Money = 0;
        JumpLvl = 0;
        Income = 1;
        IncomeLvl = 0;
        JumpLvl = 0;
        JumpUpgrade = 10;
        IncomeUpgrade = 10;
    }

    public static void Save()
    {
        var saveData = new SaveData
        {
            Money = Money,
            JumpLvl = JumpLvl,
            JumpUpgrade = JumpUpgrade,
            IncomeLvl = IncomeLvl,
            IncomeUpgrade = IncomeUpgrade,
            Income = Income,
            BestScore = BestScore
        };

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = File.Create("save.dat");

        formatter.Serialize(fileStream, saveData);
        fileStream.Close();
    }

    private void LoadData()
    {
        if (File.Exists("save.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Open("save.dat", FileMode.Open);

            var saveData = (SaveData)formatter.Deserialize(fileStream);
            fileStream.Close();

            Money = saveData.Money;
            JumpLvl = saveData.JumpLvl;
            JumpUpgrade = saveData.JumpUpgrade;
            IncomeLvl = saveData.IncomeLvl;
            IncomeUpgrade = saveData.IncomeUpgrade;
            BestScore = saveData.BestScore;
            Income = saveData.Income;
        }
    }

    public void StartGame()
    {
        Score = 0;
        IsGameStarted = true;
        MainMenu.SetActive(false);
        BestScoreBar.SetActive(false);
        ScoreBar.SetActive(true);
        NameBar.SetActive(false);
        JumpButton.SetActive(true);
        timeScaleController.StartIncreasing();
        Time.timeScale = 1.3f;
        CanContinue = true;
        PlayerController player = GameObject.FindAnyObjectByType<PlayerController>();
        player.Jump();
    }


    private void FixedUpdate()
    {
        MoneyText.text = Mathf.Round(Money).ToString() + "$";
        ScoreText.text = Score.ToString();
        ScoreTextGame.text = Score.ToString();
        BestScoreText.text = BestScore.ToString();
        BestScoreTextMenu.text = BestScore.ToString();
        Button1.text = $"JUMP HEIGHT  LEVEL:{JumpLvl}  COST:{JumpUpgrade}";
        Button2.text = $"INCOME  LEVEL:{IncomeLvl}  COST:{IncomeUpgrade}";

        if (BlockController.newRec == 1)
        {
            BlockController.newRec = 2;
            ShowNewRecord();
        }
        if (IsOnPause) Time.timeScale = 0;
    }

    public static void RestartScene()
    {
        Save();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

   public void SetlastTime()
    {
        lastTimeScale = Time.timeScale;
    }
    public void Loose()
    {
        Time.timeScale = 0f;
        IsOnPause = true;
        timeScaleController.StopIncreasing();
        ScoreBar.SetActive(false);
        JumpButton.SetActive(false);
        GameOverMenu.SetActive(true);
        if (CanContinue == false) ContinueButton.SetActive(false);
    }

    public void UpgradeIncome()
    {
        if (Money >= IncomeUpgrade)
        {
            Money -= IncomeUpgrade;
            IncomeLvl++;
            Income += 1;
            IncomeUpgrade += 50;
            Save();
            UpgradeParticle.Play();
        }
        else
        {
            WatchVideoMenu.SetActive(true);
        }
    }

    public void UpgradeJump()
    {
        if (Money >= JumpUpgrade)
        {
            Money -= JumpUpgrade;
            JumpLvl++;
            JumpUpgrade += 50;
            Save();
            UpgradeParticle.Play();
        }
        else
        {
            WatchVideoMenu.SetActive(true);
        }
    }



    public void WatchVideoAndUpgrade()
    {
        // WATCH VIDEO CODE
        JumpLvl++;
        JumpUpgrade += 50;
        Save();
        IncomeLvl++;
        Income += 1;
        IncomeUpgrade += 50;
        Save();
        UpgradeParticle.Play();
    }

    public void ShowNewRecord()
    {
        NewRecordBar.SetActive(true);
        StartCoroutine(HideNewRecordBarAfterDelay());
    }



    private IEnumerator HideNewRecordBarAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        NewRecordBar.SetActive(false);
    }

    [System.Serializable]
    private class SaveData
    {
        public float Money;
        public int JumpLvl;
        public int Income;
        public int JumpUpgrade;
        public int IncomeLvl;
        public int IncomeUpgrade;
        public int BestScore;
    }


}
