using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboBarScript : MonoBehaviour
{
    public Sprite[] Combos;
    private Image comboImage;
    public Image comboImageMenu;

    private void Start()
    {
        comboImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (GameManager.ComboScore == 10) ShowCombo(0);
        if (GameManager.ComboScore == 20) ShowCombo(1);
        if (GameManager.ComboScore == 50) ShowCombo(2);
        if (GameManager.ComboScore == 100) ShowCombo(3);
        if (GameManager.ComboScore == 200) ShowCombo(4);
        if (GameManager.ComboScore == 500) ShowCombo(5);
        if (GameManager.ComboScore == 1000) ShowCombo(6);
        comboImageMenu.sprite = Combos[GameManager.BestComboScore];
    }

    public void ShowCombo(int i)
    {
        comboImage.enabled = true;
        comboImage.sprite = Combos[i];
        GameManager.BestComboScore = i;
        GameManager.Save();
        StartCoroutine(HideComboAfterDelay());
    }

    private IEnumerator HideComboAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        comboImage.sprite = null;
        comboImage.enabled = false;
    }
}
