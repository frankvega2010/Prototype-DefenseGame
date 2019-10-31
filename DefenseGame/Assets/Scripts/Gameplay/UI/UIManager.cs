﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Gems System")]
    public Text gemsText;

    /*[Header("Milestones Panel")]
    public GameObject milestonePanel;
    private Animator milestoneAnimator;*/

    [Header("HighScore")]
    public Highscore highscore;
    public Text[] highscoreTexts;
    //public Text highscoreText;
    public Text newHighscoreText;

    [Header("Cheat System")]
    public CheatSystem cheatsComponent;
    public Image fairyButton;
    public Image towerButton;
    public Image timeButton;

    [Header("Upgrade System")]
    public Text upgradePointsText;

    [Header("Max Fairies Upgrade")]
    public Text fairiesUpgradeText;
    public Text meteorCooldownText;
    public Text fireRateUpgradeText;
    public Text fairySpeedUpgradeText;
    UpgradeSystem upgrades;

    [Header("Settings")]
    public SettingsScreen settings;
    public Toggle fullscreenButton;

    // Start is called before the first frame update
    void Start()
    {
        upgrades = UpgradeSystem.Get();
        highscore = Highscore.Get();
       // milestoneAnimator = milestonePanel.GetComponent<Animator>();
        if(GameManager.Get())
        {
            GameManager.Get().OnLevelGameOver = UpdateText;
        }
        

        if(newHighscoreText)
        {
            newHighscoreText.enabled = false;
        }

        UpdateText();

        if (Screen.fullScreen)
        {
            fullscreenButton.image.color = Color.green;
        }
        else
        {
            fullscreenButton.image.color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(cheatsComponent)
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                cheatsComponent.SwitchScreen();
            }
        }

        if (newHighscoreText)
        {
            if (highscoreTexts.Length > 0)
            {
                for (int i = 0; i < highscoreTexts.Length; i++)
                {
                    highscoreTexts[i].text = "" + highscore.highscore;

                }

                if (highscore.hasNewHighscore)
                {
                    newHighscoreText.enabled = true;
                }
            }
        }
    }

    public void CheatsReloadMeteor()
    {
        cheatsComponent.ReloadMeteor();
    }

    public void CheatsSwitchFairySInvincibility()
    {
        cheatsComponent.SwitchFairiesInmunity();

        if(Fairy.isInmunityOn)
        {
            fairyButton.color = Color.green;
        }
        else
        {
            fairyButton.color = Color.white;
        }
    }

    public void CheatsStopWave()
    {
        cheatsComponent.StopWave();
    }

    public void CheatsGivePoints()
    {
        cheatsComponent.GivePoints();
    }

    public void CheatsSwitchTowerDeletion()
    {
        cheatsComponent.SwitchTowerDeletion();

        if(TurretSpawner.Get().canDelete)
        {
            towerButton.color = Color.green;
        }
        else
        {
            towerButton.color = Color.white;
        }
    }

    public void CheatsKillAllEnemies()
    {
        cheatsComponent.KillAllEnemies();
    }

    public void CheatsKillAllFairies()
    {
        cheatsComponent.KillAllFairies();
    }

    public void CheatsStopTime()
    {
        cheatsComponent.StopTime();

        if(cheatsComponent.isTimeNormal)
        {
            timeButton.color = Color.white;
        }
        else
        {
            timeButton.color = Color.green;
        }
    }

    

    public void UpdateText()
    {
        if(upgradePointsText)
        {
            upgradePointsText.text = "Upgrade Points: " + upgrades.upgradePoints;

            UpdateSpecificText(fairiesUpgradeText, upgrades.fairiesUpgrade);
            UpdateSpecificText(meteorCooldownText, upgrades.meteorCooldownUpgrade);
            UpdateSpecificText(fireRateUpgradeText, upgrades.towersFireRateUpgrade);
            UpdateSpecificText(fairySpeedUpgradeText, upgrades.fairySpeedUpgrade);
        }

        if(gemsText)
        {
            gemsText.text = "Gems Collected: " + GameManager.Get().upgradePointsCurrentMatch;
        }
    }

    public void UpgradeBuyMaxFaires()
    {
        upgrades.BuyUpgrade(upgrades.fairiesUpgrade);
        UpdateText();
    }

    public void UpgradeBuyMeteorCooldown()
    {
        upgrades.BuyUpgrade(upgrades.meteorCooldownUpgrade);
        UpdateText();
    }

    public void UpgradeBuyFireRate()
    {
        upgrades.BuyUpgrade(upgrades.towersFireRateUpgrade);
        UpdateText();
    }

    public void UpgradeBuyFairySpeed()
    {
        upgrades.BuyUpgrade(upgrades.fairySpeedUpgrade);
        UpdateText();
    }

    private string ItExists(UpgradeSystem.Upgrade upgrade)
    {
        if(upgrades.GetNextLevelUpgradeCost(upgrade) == 0)
        {
            return "???";
        }
        else
        {
            return "" + upgrades.GetNextLevelUpgradeCost(upgrade);
        }
    }

    private void UpdateSpecificText(Text currentText, UpgradeSystem.Upgrade upgrade)
    {
        currentText.text = upgrades.GetUpgradeName(upgrade)
                + "\n\r Current Level: " + (upgrades.GetCurrentLevel(upgrade) + 1) + " /" + upgrades.GetMaxAmountOfUpgrades(upgrade)
                + "\n\r Next Level: " + upgrades.GetNextUpgradeLevel(upgrade)
                + "\n\r Cost: " + ItExists(upgrade);
    }

    /*private void SwitchPanelAnimation()
    {

    }*/

    public void SettingsSwitchFullscreen()
    {
        settings.SwitchFullscreen();

        if(Screen.fullScreen)
        {
            fullscreenButton.image.color = Color.green;
        }
        else
        {
            fullscreenButton.image.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        if (highscore)
        {
            highscore.ResetHighscoreBool();
        }
    }
}
