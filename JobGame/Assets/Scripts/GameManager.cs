using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject JumpButton;
    public GameObject GameOverMenu;
    public GameObject WatchVideoMenu;
    public GameObject NewRecordBar;
    public GameObject BestScoreBar;
    public GameObject BestComboBar;
    public GameObject ScoreBar;
    public static bool IsGameStarted = false;
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI Button1;
    public TextMeshProUGUI Button2;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI BestScoreText;
    public TextMeshProUGUI BestScoreTextMenu;
    public TextMeshProUGUI ScoreTextGame;
    public static int Score;
    public static int ComboScore;
    public static int BestScore;
    public static int BestComboScore = -1;
    public static float Money;
    public static int JumpLvl = 1;
    public static int JumpUpgrade;
    public static int IncomeLvl = 1;
    public static int IncomeUpgrade;

    public static float Income = 0.01f;


    private float lastTimeScale;

    private void Start()
    {
        LoadData();
        IsGameStarted = false;
        if (IncomeUpgrade <= 10) IncomeUpgrade = 10;
        if (JumpUpgrade <= 10) JumpUpgrade = 10;
            WatchVideoMenu.SetActive(false);
        if (Income <= 0) Income = 0.01f;
    }
    public void WatchVideoAndContinue(CharacterController player)
    {
        ///whatch video
        if (lastTimeScale >= 2) lastTimeScale -= 0.5f;
        Time.timeScale = lastTimeScale;
        player.Invisibility();
        if (player.BlockPlace)
        {
            player.transform.position = new Vector3(player.leftColumn.transform.position.x, transform.position.y + 10 + player.PlusHeightLeft, transform.position.z);
        }
        else
        {
            player.transform.position = new Vector3(player.rightColumn.transform.position.x, transform.position.y + 10 + player.PlusHeightLeft, transform.position.z);
        }
        ScoreBar.SetActive(true);
        GameOverMenu.SetActive(false);
    }

    public static void DeleteAllData()
    {
        Money = 0;
        JumpLvl = 0;
        Income = 0.01f;
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
            BestComboScore = BestComboScore,
            BestScore = BestScore
        };
        string jsonData = JsonUtility.ToJson(saveData);
        File.WriteAllText("save.json", jsonData);
    }

    private void LoadData()
    {
        if (File.Exists("save.json"))
        {
            string jsonData = File.ReadAllText("save.json");
            var saveData = JsonUtility.FromJson<SaveData>(jsonData);

            Money = saveData.Money;
            JumpLvl = saveData.JumpLvl;
            JumpUpgrade = saveData.JumpUpgrade;
            IncomeLvl = saveData.IncomeLvl;
            IncomeUpgrade = saveData.IncomeUpgrade;
            BestComboScore = saveData.BestComboScore;
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
        BestComboBar.SetActive(false);
        JumpButton.SetActive(true);
        ScoreBar.SetActive(true);
    }

    private void FixedUpdate()
    {
        MoneyText.text = Money.ToString() + "$";
        ScoreText.text = Score.ToString();
        ScoreTextGame.text = Score.ToString();
        BestScoreText.text = BestScore.ToString();
        BestScoreTextMenu.text = BestScore.ToString();
        Button1.text = $"JUMP HEIGHT  LEVEL:{JumpLvl}  COST:{JumpUpgrade}";
        Button2.text = $"INCOME  LEVEL:{IncomeLvl}  COST:{IncomeUpgrade}";

        if(BlockController.newRec == 1)
        {
            BlockController.newRec = 2;
            ShowNewRecord();
        }
    }

    public static void RestartScene()
    {
        Save();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void Loose()
    {
        ScoreBar.SetActive(false);
        GameOverMenu.SetActive(true);
        lastTimeScale = Time.timeScale;
        ComboScore = 0;
        Time.timeScale = 0f;
    }
    public void UpgradeIncome()
    {
        if (Money >= IncomeUpgrade)
        {
            Money -= IncomeUpgrade;
            IncomeLvl++;
            Income++;
            IncomeUpgrade += 10;
            Save();
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
            JumpUpgrade += 10;
            Save();
        }
        else
        {
            WatchVideoMenu.SetActive(true);
        }
    }

    public void WatchVideoAndUpgrade()
    {
        //WATCH VIDEO CODE
        IncomeLvl++;
        Income++;
        IncomeUpgrade += 10;
        JumpLvl++;
        JumpUpgrade += 10;
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
        public float Income;
        public int JumpUpgrade;
        public int IncomeLvl;
        public int IncomeUpgrade;
        public int BestComboScore;
        public int BestScore;
    }
}
