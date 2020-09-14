using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using static BreakInfinity.BigDouble;

public class PrestigeManager : MonoBehaviour
{
    public IdleTutorialScript game;
    public Text[] costText = new Text[3];
    public Text buyMax1Text;
    public Text buyMax2Text;
    public Text buyMax3Text;



    public Text gemsText;
    public Text gemBoostText;
    public Text gemsToGetText;


    public Image[] costBars = new Image[3];

    public string[] costDesc;

    public BigDouble[] costs;
    public BigDouble[] levels;
    
    private BigDouble cost1 => 5 * BigDouble.Pow(1.5, game.data.prestigeULevel1);
    private BigDouble cost2 => 10 * BigDouble.Pow(1.5, game.data.prestigeULevel2);
    private BigDouble cost3 => 100 * BigDouble.Pow(2.5, game.data.prestigeULevel3);


    public void StartPrestige()
    {
        costs = new BigDouble[3];
        levels = new BigDouble[3];
        costDesc = new[] {"Click is 50% more effective", "You gain 10% more coins/s", "Gems are +1.01x better"};
    }

    public void Run()
    {
        ArrayManager();
        UI();

        gemsText.text = "Gems " + Methods.NotationMethod(Floor(data.gems), y: "F0");
        gemBoostText.text = Methods.NotationMethod(TotalGemBoost(), y: "F2") + "x boost";

        if (mainMenuGroup.gameObject.activeSelf)
        {
            gemsToGetText.text = "Prestige:\n+" + Methods.NotationMethod(Floor(data.gemsToGet), y: "F0") + " Gems";
        }

        void UI()
        {
            if (game.prestigeGroup.gameObject.activeSelf)
            {
                for (var i = 0; i < costText.Length; i++)
                {
                    costText[i].text = $"Level {levels[i]}\n{costDesc[i]}\nCost: {Methods.NotationMethod(costs[i], "F0")} Gems";
                    buyMax1Text.text = "Buy Max (" + Methods.NotationMethod(BuyUpgrade1MaxCount(), y: "F0") + ")";
                    buyMax2Text.text = "Buy Max (" + Methods.NotationMethod(BuyUpgrade2MaxCount(), y: "F0") + ")";
                    buyMax3Text.text = "Buy Max (" + Methods.NotationMethod(BuyUpgrade3MaxCount(), y: "F0") + ")";
                    Methods.BigDoubleFill(game.data.gems, costs[i], costBars[i]);
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
                Buy(ref data.prestigeULevel1);
                break;
            case 1:
                var b1 = 5; //Base cost
                var c1 = data.gems; //Currency
                var r1 = 1.5; //Multiplier
                var k1 = data.prestigeULevel1;
                var n1 = BigDouble.Floor(BigDouble.Log((c1 * (r1 - 1)) / (b1 * BigDouble.Pow(r1, k1)) + 1, r1));

                var Maxcost1 = b1 * (BigDouble.Pow(r1, k1) * (BigDouble.Pow(r1, n1.ToDouble()) - 1) / (r1 - 1));

                if (data.gems >= Maxcost1)
                {
                    data.prestigeULevel1 += (int)n1;
                    data.gems -= Maxcost1;
                    //levels += n1;
                }
                break;
            case 2:
                Buy(ref data.prestigeULevel2);
                break;
            case 3:
                var b2 = 10; //Base cost
                var c2 = data.gems; //Currency
                var r2 = 1.5; //Multiplier
                var k2 = data.prestigeULevel2;
                var n2 = BigDouble.Floor(BigDouble.Log((c2 * (r2 - 1)) / (b2 * BigDouble.Pow(r2, k2)) + 1, r2));

                var Maxcost2 = b2 * (BigDouble.Pow(r2, k2) * (BigDouble.Pow(r2, n2.ToDouble()) - 1) / (r2 - 1));

                if (data.gems >= Maxcost2)
                {
                    data.prestigeULevel2 += (int)n2;
                    data.gems -= Maxcost2;
                }
                break;
            case 4:
                Buy(ref data.prestigeULevel3);
                break;
            case 5:
                var b3 = 100; //Base cost
                var c3 = data.gems; //Currency
                var r3 = 2.5; //Multiplier
                var k3 = data.prestigeULevel3;
                var n3 = BigDouble.Floor(BigDouble.Log((c3 * (r3 - 1)) / (b3 * BigDouble.Pow(r3, k3)) + 1, r3));

                var Maxcost3 = b3 * (BigDouble.Pow(r3, k3) * (BigDouble.Pow(r3, n3.ToDouble()) - 1) / (r3 - 1));

                if (data.gems >= Maxcost3)
                {
                    data.prestigeULevel3 += (int)n3;
                    data.gems -= Maxcost3;
                }
                break;

        }
        void Buy(ref int levels)
        {
            if (data.gems >= costs[id])
            {
                data.gems -= costs[id];
                levels++;
            }
        }
        
    }
    public void ArrayManager()
    {
        var data = game.data;

        costs[0] = cost1;
        costs[1] = cost2;
        costs[2] = cost3;

        levels[0] = data.prestigeULevel1;
        levels[1] = data.prestigeULevel2;
        levels[2] = data.prestigeULevel3;
    }

    public BigDouble BuyUpgrade1MaxCount()
    {
        var data = game.data;
        var b = 5; //Base cost
        var c = data.gems; //Currency
        var r = 1.5; //Multiplier
        var k = data.prestigeULevel1;
        var n = BigDouble.Floor(BigDouble.Log((c * (r - 1)) / (b * BigDouble.Pow(r, k)) + 1, r));
        return n;
    }

    public BigDouble BuyUpgrade2MaxCount()
    {
        var data = game.data;
        var b = 10; //Base cost
        var c = data.gems; //Currency
        var r = 1.5; //Multiplier
        var k = data.prestigeULevel2;
        var n = BigDouble.Floor(BigDouble.Log((c * (r - 1)) / (b * BigDouble.Pow(r, k)) + 1, r));
        return n;
    }

    public BigDouble BuyUpgrade3MaxCount()
    {
        var data = game.data;
        var b = 100; //Base cost
        var c = data.gems; //Currency
        var r = 2.5; //Multiplier
        var k = data.prestigeULevel3;
        var n = BigDouble.Floor(BigDouble.Log((c * (r - 1)) / (b * BigDouble.Pow(r, k)) + 1, r));
        return n;
    }
}
