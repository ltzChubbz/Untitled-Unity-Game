using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using BreakInfinity;
using static BreakInfinity.BigDouble;
using System.Collections.Generic;


public class IdleTutorialScript : MonoBehaviour
{
    public PlayerData data;
    public EventManager events;
    public PrestigeManager prestige;
    public RebirthManager rebirth;
    public AutomatorManager auto;

    public float saveTimer;

    //Episode 19
    public GameObject clickUpgrade1;
    public GameObject clickUpgrade2;
    public GameObject productionUpgrade1;
    public GameObject productionUpgrade2;


    public Text clickValueText;

    public Text coinsText;
    public Text coinsPerSecText;

    public Text clickUpgrade1Text;
    public Text clickUpgrade1MaxText;

    public Text clickUpgrade2Text;
    public Text clickUpgrade2MaxText;

    public Text productionUpgrade1Text;
    public Text productionUpgrade1MaxText;

    public Text productionUpgrade2Text;
    public Text productionUpgrade2MaxText;

    public Image clickUpgrade1Bar;
    public Image clickUpgrade2Bar;
    public Image productionUpgrade1Bar;
    public Image productionUpgrade2Bar;

    //Canvas Groups
    public Canvas mainMenuGroup;
    public Canvas upgradesGroup;
    public Canvas achievementsGroup;
    public Canvas eventsGroup;
    public Canvas prestigeGroup;
    public Canvas rebirthGroup;
    public Canvas autoGroup;
    public BigDouble tabSwitcher;

    public GameObject settings;
    public GameObject achievementScreen;

    public List<Achievement> achievementList = new List<Achievement>();

    public void Start()
    {
        Application.targetFrameRate = 60; //Can be 15, 30, 60, 120, 144

        foreach (var x in achievementScreen.GetComponentsInChildren<Achievement>())
            achievementList.Add(x);

        mainMenuGroup.gameObject.SetActive(true);
        upgradesGroup.gameObject.SetActive(false);
        achievementsGroup.gameObject.SetActive(false);
        eventsGroup.gameObject.SetActive(false);
        prestigeGroup.gameObject.SetActive(false);
        rebirthGroup.gameObject.SetActive(false);

        tabSwitcher = 0;

        SaveSystem.LoadPlayer(ref data);
        events.StartEvents();
        prestige.StartPrestige();
        auto.StartAutomators();
    }

    public void Update()
    {
        RunAchievements();
        prestige.Run();
        rebirth.Run();
        auto.Run();
        


        //Gems calc//
        data.gemsToGet = 150 * Sqrt(data.coins / 1e7);

        /////////////

        //Coins text//
        coinsPerSecText.text = Methods.NotationMethod(TotalCoinsPerSecond(), y: "F0") + " coins/s";
        coinsText.text = "Coins: " + Methods.NotationMethod(data.coins, y: "F0");

        clickValueText.text = "Click\n+" + Methods.NotationMethod(TotalClickValue(), y: "F0") + " Coins";
        //////////////

        //Upgrades Cost Calc//
        var clickUpgrade1Cost = 10 * Pow(1.07, data.clickUpgrade1Level);
        var clickUpgrade1CostString = Methods.NotationMethod(clickUpgrade1Cost, "F2");
        var clickUpgrade1LevelString = Methods.NotationMethod(data.clickUpgrade1Level, y: "F0");

        var clickUpgrade2Cost = 100 * Pow(1.07, data.clickUpgrade2Level);
        var clickUpgrade2CostString = Methods.NotationMethod(clickUpgrade2Cost, y: "F0");
        var clickUpgrade2LevelString = Methods.NotationMethod(data.clickUpgrade2Level, y: "F0");

        var productionUpgrade1Cost = 25 * Pow(1.07, data.productionUpgrade1Level);
        var productionUpgrade1CostString = Methods.NotationMethod(productionUpgrade1Cost, y: "F0");
        var productionUpgrade1LevelString = Methods.NotationMethod(data.productionUpgrade1Level, y: "F0");
        var production1TotalPower = (TotalBoost() * Pow(1.1, prestige.levels[1])) * data.productionUpgrade1Level * 1;

        var productionUpgrade2Cost = 250 * Pow(1.07, data.productionUpgrade2Level);
        var productionUpgrade2CostString = Methods.NotationMethod(productionUpgrade2Cost, y: "F0");
        var productionUpgrade2LevelString = Methods.NotationMethod(data.productionUpgrade2Level, y: "F0");
        var production2TotalPower = (TotalBoost() * Pow(1.1, prestige.levels[1])) * data.productionUpgrade2Level * 5;

       

        if (upgradesGroup.gameObject.activeSelf)
        {
            clickUpgrade1Text.text = "Click Upgrade 1\nCost: " + clickUpgrade1CostString + "\n Power: +1 Click\nLevel: " + clickUpgrade1LevelString;
            clickUpgrade1MaxText.text = "Buy Max (" + Methods.NotationMethod(BuyClickUpgrade1MaxCount(), y: "F0") + ")";
            Methods.BigDoubleFill(data.coins, clickUpgrade1Cost, clickUpgrade1Bar);

            clickUpgrade2Text.text = "Click Upgrade 2\nCost: " + clickUpgrade2CostString + "\n Power: +5 Click\nLevel: " + clickUpgrade2LevelString;
            clickUpgrade2MaxText.text = "Buy Max (" + Methods.NotationMethod(BuyClickUpgrade2MaxCount(), y: "F0") + ")";
            Methods.BigDoubleFill(data.coins, clickUpgrade2Cost, clickUpgrade2Bar);

            productionUpgrade1Text.text = "Production Upgrade 1\nCost: " + productionUpgrade1CostString + "coins\n Power: +" + Methods.NotationMethod(production1TotalPower, y: "F2") + " coins/s\nLevel: " + productionUpgrade1LevelString;
            productionUpgrade1MaxText.text = "Buy Max (" + Methods.NotationMethod(BuyProductionUpgrade1MaxCount(), y: "F0") + ")";
            Methods.BigDoubleFill(data.coins, productionUpgrade1Cost, productionUpgrade1Bar);

            productionUpgrade2Text.text = "Production Upgrade 2\nCost: " + productionUpgrade2CostString + "coins\n Power: +" + Methods.NotationMethod(production2TotalPower, y: "F2") + " coins/s\nLevel: " + productionUpgrade2LevelString;
            productionUpgrade2MaxText.text = "Buy Max (" + Methods.NotationMethod(BuyProductionUpgrade2MaxCount(), y: "F0") + ")";
            Methods.BigDoubleFill(data.coins, productionUpgrade2Cost, productionUpgrade2Bar);

            clickUpgrade1.gameObject.SetActive(data.coinsCollected >= 10);
            clickUpgrade2.gameObject.SetActive(data.coinsCollected >= 100);
            productionUpgrade1.gameObject.SetActive(data.coinsCollected >= 25);
            productionUpgrade2.gameObject.SetActive(data.coinsCollected >= 250);
        }

        data.coins += TotalCoinsPerSecond() * Time.deltaTime;
        data.coinsCollected += TotalCoinsPerSecond() * Time.deltaTime;

        saveTimer += Time.deltaTime;
        if (!(saveTimer >= 15)) return;
            SaveSystem.SavePlayer(data);
            saveTimer = 0;
    }

    private static string[] AchievementStrings => new string[] {"Current Coins", "Total Coins Collected"};
    private BigDouble[] AchievementNumbers => new BigDouble[] {data.coins, data.coinsCollected};


   private void RunAchievements()
    {
        UpdateAchievement(AchievementStrings[0], AchievementNumbers[0], ref data.achLevel1, ref achievementList[0].fill, ref achievementList[0].title, ref achievementList[0].progress);
        UpdateAchievement(AchievementStrings[1], AchievementNumbers[1], ref data.achLevel2, ref achievementList[1].fill, ref achievementList[1].title, ref achievementList[1].progress);
    }


    private void UpdateAchievement(string name, BigDouble number, ref float level, ref Image fill, ref Text title, ref Text progress)
    {
        var cap = BigDouble.Pow(10, level);
        if (achievementsGroup.gameObject.activeSelf)
        {
            title.text = $"{name}\n({Methods.NotationMethod(level, "F0")})";
            progress.text = $"{Methods.NotationMethod(number, "F2")} / {Methods.NotationMethod(cap, "F2")}";

            Methods.BigDoubleFill(number, cap, fill);
        }

        if (number < cap) return;
        BigDouble levels = 0;
        if (number / cap >= 1)
            levels = Floor(Log10(number / cap)) + 1;
        level += (float)levels;
    }


     public void Prestige()
    {
        if (data.coins > 1000)
        {
            data.coins = 0;
            data.coinsClickValue = 1;
            data.productionUpgrade2Power = 5;

            data.productionUpgrade1Level = 0;
            data.productionUpgrade2Level = 0;
            data.clickUpgrade1Level = 0;
            data.clickUpgrade2Level = 0;

            data.gems += data.gemsToGet;
        }
    }

    public BigDouble TotalBoost()
    {
        BigDouble temp = TotalGemBoost();
        temp *= events.eventTokenBoost;
        return temp;
    }

    public BigDouble TotalGemBoost()
    {
        BigDouble temp = data.gems;
        temp *= 0.05 + (prestige.levels[2] * 0.01);
        temp *= rebirth.soulsBoost;
        return temp + 1;
    }
    private BigDouble TotalCoinsPerSecond()
    {
        BigDouble temp = 0;
        temp += data.productionUpgrade1Level;
        temp += data.productionUpgrade2Power * data.productionUpgrade2Level;
        temp *= TotalBoost();
        temp *= Pow(1.1, prestige.levels[1]);
        return temp;
    }

    private BigDouble TotalClickValue()
    {
        var temp = data.coinsClickValue;
        temp *= TotalBoost();
        temp *= Pow(1.5, prestige.levels[0]);
        return temp;
    }
    
    public void Click()
    {
        data.coins += TotalClickValue();
        data.coinsCollected += TotalClickValue();
    }

    //Buy Max Counts
    public BigDouble BuyClickUpgrade1MaxCount()
    {
        var b = 10; //Base cost
        var c = data.coins; //Currency
        var r = 1.07; //Multiplier
        var k = data.clickUpgrade1Level;
        var n = Floor(Log((c * (r - 1)) / (b * Pow(r, k)) + 1, r));
        return n;
    }

    public BigDouble BuyClickUpgrade2MaxCount()
    {
        var b1 = 100; //Base cost
        var c1 = data.coins; //Currency
        var r1 = 1.07; //Multiplier
        var k1 = data.clickUpgrade2Level;
        var n1 = Floor(Log((c1 * (r1 - 1)) / (b1 * Pow(r1, k1)) + 1, r1));
        return n1;
    }

    public BigDouble BuyProductionUpgrade1MaxCount()
    {
        var b2 = 25; //Base cost
        var c2 = data.coins; //Currency
        var r2 = 1.07; //Multiplier
        var k2 = data.productionUpgrade1Level;
        var n2 = Floor(Log((c2 * (r2 - 1)) / (b2 * Pow(r2, k2)) + 1, r2));
        return n2;
    }

    public BigDouble BuyProductionUpgrade2MaxCount()
    {
        var b3 = 250; //Base cost
        var c3 = data.coins; //Currency
        var r3 = 1.07; //Multiplier
        var k3 = data.productionUpgrade2Level;
        var n3 = Floor(Log((c3 * (r3 - 1)) / (b3 * Pow(r3, k3)) + 1, r3));
        return n3;
    }

    public void BuyUpgrade(string upgradeID)
    {
        switch (upgradeID)
        {
            case "C1":
                var cost1 = 10 * Pow(1.07, data.clickUpgrade1Level);
                if (data.coins >= cost1)
                {
                    data.clickUpgrade1Level++;
                    data.coins -= cost1;
                    data.coinsClickValue++;
                }
                break;

            case "C1Max":
                var b = 10; //Base cost
                var c = data.coins; //Currency
                var r = 1.07; //Multiplier
                var k = data.clickUpgrade1Level;
                var n = Floor(Log((c * (r - 1)) / (b * Pow(r, k)) + 1, r));

                var cost2 = b * (Pow(r, k) * (Pow(r, n.ToDouble()) - 1) / (r - 1));

                if (data.coins >= cost2)
                {
                    data.clickUpgrade1Level += (int)n;
                    data.coins -= cost2;
                    data.coinsClickValue += n;
                }
                break;

            case "C2":
                var cost3 = 100 * Pow(1.07, data.clickUpgrade2Level);
                if (data.coins >= cost3)
                {
                    data.clickUpgrade2Level++;
                    data.coins -= cost3;
                    data.coinsClickValue += 5;
                }
                break;

            case "C2Max":
                var b1 = 100; //Base cost
                var c1 = data.coins; //Currency
                var r1 = 1.07; //Multiplier
                var k1 = data.clickUpgrade2Level;
                var n1 = Floor(Log((c1 * (r1 - 1)) / (b1 * Pow(r1, k1)) + 1, r1));

                var cost4 = b1 * (Pow(r1, k1) * (Pow(r1, n1.ToDouble()) - 1) / (r1 - 1));

                if (data.coins >= cost4)
                {
                    data.clickUpgrade2Level += (int)n1;
                    data.coins -= cost4;
                    data.coinsClickValue += n1 * 5;
                }
                break;

            case "P1":
                var cost5 = 25 * Pow(1.07, data.productionUpgrade1Level);
                if (data.coins >= cost5)
                {
                    data.productionUpgrade1Level++;
                    data.coins -= cost5;
                }
                break;

            case "P1Max":
                var b2 = 25; //Base cost
                var c2 = data.coins; //Currency
                var r2 = 1.07; //Multiplier
                var k2 = data.productionUpgrade1Level;
                var n2 = Floor(Log((c2 * (r2 - 1)) / (b2 * Pow(r2, k2)) + 1, r2));

                var cost6 = b2 * (Pow(r2, k2) * (Pow(r2, n2.ToDouble()) - 1) / (r2 - 1));

                if (data.coins >= cost6)
                {
                    data.productionUpgrade1Level += (int)n2;
                    data.coins -= cost6;
                    data.coinsClickValue += n2;
                }
                break;

            case "P2":
                var cost7 = 250 * Pow(1.07, data.productionUpgrade2Level);
                if (data.coins >= cost7)
                {
                    data.productionUpgrade2Level++;
                    data.coins -= cost7;
                }
                break;

            case "P2Max":
                var b3 = 250; //Base cost
                var c3 = data.coins; //Currency
                var r3 = 1.07; //Multiplier
                var k3 = data.productionUpgrade2Level;
                var n3 = Floor(Log((c3 * (r3 - 1)) / (b3 * Pow(r3, k3)) + 1, r3));

                var cost8 = b3 * (Pow(r3, k3) * (Pow(r3, n3.ToDouble()) - 1) / (r3 - 1));

                if (data.coins >= cost8)
                {
                    data.productionUpgrade2Level += (int)n3;
                    data.coins -= cost8;
                    data.coinsClickValue += n3;
                }
                break;
            default:
                Debug.Log(message: "Not assigned to upgrade");
                break;
        }
    }

    public void ChangeWindows(string id)
    {
        DisableAll();
        switch (id)
        {
            case "upgrades":
                upgradesGroup.gameObject.SetActive(true);
                break;
            case "main":
                mainMenuGroup.gameObject.SetActive(true);
                break;
            case "achievements":
                achievementsGroup.gameObject.SetActive(true);
                break;
            case "events":
                eventsGroup.gameObject.SetActive(true);
                break;
            case "prestige":
                prestigeGroup.gameObject.SetActive(true);
                break;
            case "rebirth":
                rebirthGroup.gameObject.SetActive(true);
                break;
            case "auto":
                autoGroup.gameObject.SetActive(true);
                break;
        }

        void DisableAll()
        {
            mainMenuGroup.gameObject.SetActive(false);
            upgradesGroup.gameObject.SetActive(false);
            achievementsGroup.gameObject.SetActive(false);
            eventsGroup.gameObject.SetActive(false);
            prestigeGroup.gameObject.SetActive(false);
            rebirthGroup.gameObject.SetActive(false);
            autoGroup.gameObject.SetActive(false);
        }
    }

    public void GoToSettings()
    {
        settings.gameObject.SetActive(true);
    }

    public void GoBackFromSettings()
    {
        settings.gameObject.SetActive(false);
    }

    public void FullReset()
    {
        ChangeWindows("main");
        data.FullReset();
    }

}

public class Methods : MonoBehaviour
{
    public static void CanvasGroupChanger(bool x, CanvasGroup y)
    {
        y.alpha = x ? 1 : 0;
        y.interactable = x;
        y.blocksRaycasts = x;
    }

    public static void BigDoubleFill(BigDouble x, BigDouble y, Image fill)
    {
        float z;
        var a = x / y;
        if (a < 0.001)
            z = 0;
        else if (a > 10)
            z = 1;
        else
            z = (float)a.ToDouble();
        fill.fillAmount = z;
    }

    public static string NotationMethod(BigDouble x, string y)
    {
        if (x > 1000)
        {
            var exponent = Floor(Log10(Abs(x)));
            var mantissa = (x / Pow(10, exponent));
            return mantissa.ToString("F2") + "e" + exponent;
        }
        return x.ToString(y);
    }

}