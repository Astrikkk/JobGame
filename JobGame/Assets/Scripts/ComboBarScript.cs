using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboBarScript : MonoBehaviour
{
    public Sprite[] Combos;
    private Image comboImage;

    private void Start()
    {
        comboImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (GameManager.Score == 10) ShowCombo(0);
        if (GameManager.Score == 20) ShowCombo(1);
        if (GameManager.Score == 50) ShowCombo(2);
        if (GameManager.Score == 100) ShowCombo(3);
        if (GameManager.Score == 200) ShowCombo(4);
        if (GameManager.Score == 500) ShowCombo(5);
        if (GameManager.Score == 1000) ShowCombo(6);
    }

    public void ShowCombo(int i)
    {
        comboImage.enabled = true;
        comboImage.sprite = Combos[i];
        StartCoroutine(HideComboAfterDelay());
    }

    private IEnumerator HideComboAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        comboImage.sprite = null;
        comboImage.enabled = false;
    }
}
