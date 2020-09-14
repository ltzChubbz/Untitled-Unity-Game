using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using static BreakInfinity.BigDouble;

public class AutomatorManager : MonoBehaviour
{
    public IdleTutorialScript game;
    public Text[] costText = new Text[4];

    public Image[] costBars = new Image[4];

    public string[] costDesc;

    public BigDouble[] costs;
    public int[] levels;
    public int[] levelsCap;
    public float[] intervals;
    public float[] timer;

    private BigDouble cost1 => 1e4 * BigDouble.Pow(1.5, game.data.autolevel1);
    private BigDouble cost2 => 1e5 * BigDouble.Pow(1.5, game.data.autolevel2);
    private BigDouble cost3 => 1e7 * BigDouble.Pow(1.5, game.data.autolevel3);
    private BigDouble cost4 => 1e8 * BigDouble.Pow(1.5, game.data.autolevel4);

    public void StartAutomators()
    {
        costs = new BigDouble[4];
        levels = new int[4];
        levelsCap = new[] { 21, 21, 21, 21 };
        intervals = new float[4];
        timer = new float[4];
        costDesc = new[] { "Click Upgrade 1 Autobuyer", "Production Upgrade 1 Autobuyer", "Click Upgrade 2 Autobuyer", "Production Upgrade 2 Autobuyer" };
    }

    public void Run()
    {
        ArrayManager();
        UI();
        RunAuto();

        void UI()
        {
            if (game.autoGroup.gameObject.activeSelf)
            {
                for (var i = 0; i < costText.Length; i++)
                {
                    costText[i].text = $"{costDesc[i]}\nCost: {Methods.NotationMethod(costs[i], "F0")} coins\nInterval: {(levels[i] >= levelsCap[i] ? "Instant" : intervals[i].ToString("F1"))}";
                    Methods.BigDoubleFill(game.data.coins, costs[i], costBars[i]);
                }
            }
        }

        void RunAuto()
        {
            Auto(0, "C1");
            Auto(1, "P1");
            Auto(2, "C2");
            Auto(3, "P2");

            void Auto(int id, string name)
            {
                if (levels[id] > 0)
                {
                    if (levels[id] != levelsCap[id])
                    {
                        timer[id] += Time.deltaTime;
                        if (timer[id] >= intervals[id])
                        {
                            game.BuyUpgrade(name);
                            timer[id] = 0;
                        }
                    }
                    else
                    {
                        switch (name)
                        {
                            case "C1":
                                if (game.BuyClickUpgrade1MaxCount() != 0)
                                    game.BuyUpgrade("C1Max");
                            break;
                            case "P1":
                                if (game.BuyProductionUpgrade1MaxCount() != 0)
                                    game.BuyUpgrade("P1Max");
                                break;
                            case "C2":
                                if (game.BuyClickUpgrade1MaxCount() != 0)
                                    game.BuyUpgrade("C2Max");
                                break;
                            case "P2":
                                if (game.BuyProductionUpgrade1MaxCount() != 0)
                                    game.BuyUpgrade("P2Max");
                                break;
                        }
                    }
                }
            }
        }
    }

                        


    public void BuyUpgrade(int id)
    {
        var data = game.data;
        
        switch (id)
        {
            case 0:
                Buy(ref data.autolevel1);
                break;
            case 1:
                Buy(ref data.autolevel2);
                break;
            case 2:
                Buy(ref data.autolevel3);
                break;
            case 3:
                Buy(ref data.autolevel4);
                break;
        }
        void Buy(ref int levels)
        {
            if (data.coins >= costs[id] & levels <= levelsCap[id])
            {
                data.coins -= costs[id];
                levels++;
            }
        }

    }

    private void ArrayManager()
    {
        var data = game.data;

        costs[0] = cost1;
        costs[1] = cost2;
        costs[2] = cost3;
        costs[3] = cost4;

        levels[0] = data.autolevel1;
        levels[1] = data.autolevel2;
        levels[2] = data.autolevel3;
        levels[3] = data.autolevel4;

        if (data.autolevel1 > 0)
            intervals[0] = 10 - (data.autolevel1 - 1) * 0.5f;
        if (data.autolevel2 > 0)
            intervals[1] = 10 - (data.autolevel2 - 1) * 0.5f;
        if (data.autolevel3 > 0)
            intervals[2] = 10 - (data.autolevel3 - 1) * 0.5f;
        if (data.autolevel4 > 0)
            intervals[3] = 10 - (data.autolevel4 - 1) * 0.5f;
    }

    


}
