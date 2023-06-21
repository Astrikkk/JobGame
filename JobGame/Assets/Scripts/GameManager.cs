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
    public static bool IsGameStarted = false;
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI Button1;
    public TextMeshProUGUI Button2;
    public static double Money;
    public static int JumpLvl = 1;
    public static int JumpUpgrade;
    public static int IncomeLvl = 1;
    public static int IncomeUpgrade;

    public static double Income = 0.01;

    private void Start()
    {
        IsGameStarted = false;
        if (IncomeUpgrade <= 10) IncomeUpgrade = 10;
        if (JumpUpgrade <= 10) JumpUpgrade = 10;
    }

    private void Awake()
    {
        LoadData();
    }

    public static void Save()
    {
        var saveData = new SaveData
        {
            Money = Money,
            JumpLvl = JumpLvl,
            JumpUpgrade = JumpUpgrade,
            IncomeLvl = IncomeLvl,
            IncomeUpgrade = IncomeUpgrade
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
        }
    }

    public void StartGame()
    {
        IsGameStarted = true;
        MainMenu.SetActive(false);
        JumpButton.SetActive(true);
    }

    private void FixedUpdate()
    {
        MoneyText.text = Money.ToString() + "$";
        Button1.text = $"JUMP HEIGHT  LEVEL:{JumpLvl}  COST:{JumpUpgrade}";
        Button2.text = $"INCOME  LEVEL:{IncomeLvl}  COST:{IncomeUpgrade}";
    }

    public static void RestartScene()
    {
        Save();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
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
    }

    [System.Serializable]
    private class SaveData
    {
        public double Money;
        public int JumpLvl;
        public int JumpUpgrade;
        public int IncomeLvl;
        public int IncomeUpgrade;
    }
}
