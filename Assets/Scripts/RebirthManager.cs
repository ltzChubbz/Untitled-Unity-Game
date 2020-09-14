using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using static BreakInfinity.BigDouble;


public class RebirthManager : MonoBehaviour
{
    public IdleTutorialScript game;
    public Text soulsText;
    public Text soulsBoostText;
    public Text soulsToGetText;

    public BigDouble soulsToGet => 150 * Sqrt(game.data.gems / 1e7);

    public BigDouble soulsBoost => game.data.souls * 0.001 + 1;

    public void Run()
    {
        UI();
        void UI()
        {
            var data = game.data;
            if (!game.rebirthGroup.gameObject.activeSelf) return;
            soulsText.text = $"Souls: {Methods.NotationMethod(data.souls, "F2")}";
            soulsToGetText.text = $"+{Methods.NotationMethod(soulsToGet, "F2")} Souls";
            soulsBoostText.text = $"Gems are: {Methods.NotationMethod(soulsBoost, "F2")}x better";
        }
    }

    public void Rebirth()
    {
        var data = game.data;

        data.souls += soulsToGet;

        data.coins = 0;
        data.coinsCollected = 0;
        data.coinsClickValue = 1;
        data.gems = 0;
        data.clickUpgrade1Level = 0;
        data.clickUpgrade2Level = 0;
        data.productionUpgrade1Level = 0;
        data.productionUpgrade2Level = 0;
        data.productionUpgrade2Power = 5;
        data.prestigeULevel1 = 0;
        data.prestigeULevel2 = 0;
        data.prestigeULevel3 = 0;
        game.rebirthGroup.gameObject.SetActive(false);
        game.mainMenuGroup.gameObject.SetActive(true);

    }
}

