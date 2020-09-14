using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BreakInfinity;
using static BreakInfinity.BigDouble;

[Serializable]
public class PlayerData
{
    public BigDouble coins;
    public BigDouble coinsCollected;
    public BigDouble coinsClickValue;
    public BigDouble gems;
    public BigDouble gemsToGet;
    public int clickUpgrade1Level;
    public int clickUpgrade2Level;
    public int productionUpgrade1Level;
    public int productionUpgrade2Level;
    public BigDouble productionUpgrade2Power;
    public float achLevel1;
    public float achLevel2;
    //Events
    public BigDouble eventTokens;
    public float[] eventCooldown = new float[7];
    public int eventActiveID;

    #region Prestige
    public int prestigeULevel1;
    public int prestigeULevel2;
    public int prestigeULevel3;
    #endregion

    #region Rebirth
    public BigDouble souls;
    #endregion

    #region Automators
    public int autolevel1;
    public int autolevel2;
    public int autolevel3;
    public int autolevel4;
    #endregion


    public PlayerData()
    {
        FullReset();
    }

    public void FullReset()
    {
        coins = 0;
        coinsCollected = 0;
        coinsClickValue = 1;
        gems = 0;
        gemsToGet = 0;
        clickUpgrade1Level = 0;
        clickUpgrade2Level = 0;
        productionUpgrade1Level = 0;
        productionUpgrade2Level = 0;
        productionUpgrade2Power = 5;
        achLevel1 = 0;
        achLevel2 = 0;
        eventTokens = 0;
        for (int i = 0; i < eventCooldown.Length - 1; i++)
            eventCooldown[i] = 0;
        eventActiveID = 0;
        prestigeULevel1 = 0;
        prestigeULevel2 = 0;
        prestigeULevel3 = 0;
        souls = 0;

        #region Autos
        autolevel1 = 0;
        autolevel2 = 0;
        autolevel3 = 0;
        autolevel4 = 0;
        #endregion

    }
}
