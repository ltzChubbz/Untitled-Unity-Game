using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public IdleTutorialScript game;
    public Text eventTokensText;

    public BigDouble eventTokenBoost => (game.data.eventTokens / 100) + 1;

    public GameObject eventRewardPopUp;
    public Text eventRewardText;

    public GameObject[] events = new GameObject[7];
    public GameObject[] eventsUnlocked = new GameObject[7];
    public Text[] rewardText = new Text[7];
    public Text[] currencyText = new Text[7];
    public Text[] costText = new Text[7];
    public Text[] startText = new Text[7];

    public BigDouble[] reward = new BigDouble[7];
    public BigDouble[] currencies = new BigDouble[7];

    public BigDouble[] costs = new BigDouble[7];
    public int[] levels = new int[7];

    public bool eventActive;

    public string DayOfTheWeek()
    {
        var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        return dt.DayOfWeek.ToString();
    }

    public string previousDayChecked;

    public void StartEvents()
    {
        eventActive = false;
        previousDayChecked = DayOfTheWeek();
        if (game.data.eventActiveID != 0)
        {
            game.data.eventActiveID = 0;
            game.data.eventCooldown = new float[7];
        }

    }

    public void Update()
    {
        var data = game.data;
        eventTokensText.text = $"Event Tokens: {game.NotationMethod(data.eventTokens, "F2")} ({game.NotationMethod(eventTokenBoost, "F2")}x)";

        reward[0] = BigDouble.Log10(currencies[0] + 1);
        reward[1] = BigDouble.Log10(currencies[1] / 5 + 1);
        reward[2] = BigDouble.Log10(currencies[2] + 2);
        reward[3] = BigDouble.Log10(currencies[3] / 10 + 2);
        reward[4] = BigDouble.Log10(currencies[4] + 3);
        reward[5] = BigDouble.Log10(currencies[5] / 15 + 5);
        reward[6] = BigDouble.Log10(currencies[6] + 4);

        for (int i = 0; i < 7; i++)
            costs[i] = 10 * BigDouble.Pow(1.15, levels[i]);

        if(previousDayChecked != DayOfTheWeek() & eventActive)
        {
            data.eventActiveID = 0;
            for (var i = 0; 1 < 7; i++)
                data.eventCooldown[i] = 0;
        }

        switch (DayOfTheWeek())
        {
            case "Monday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(0);
                    RunEvent(0);
                break;
            case "Tuesday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(1);
                    RunEvent(1);
                break;
            case "Wednesday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(2);
                    RunEvent(2);
                break;
            case "Thursday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(3);
                    RunEvent(3);
                break;
            case "Friday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(4);
                    RunEvent(4);
                break;
            case "Saturday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(5);
                    RunEvent(5);
                break;
            case "Sunday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(6);
                    RunEvent(6);
                break;
        }

        if (data.eventActiveID == 0 & game.data.eventCooldown[CurrentDay()] > 0)
            game.data.eventCooldown[CurrentDay()] -= Time.deltaTime;
        else if (data.eventActiveID != 0 & game.data.eventCooldown[CurrentDay()] > 0)
            game.data.eventCooldown[CurrentDay()] -= Time.deltaTime;
        else if (data.eventActiveID != 0 & game.data.eventCooldown[CurrentDay()] <= 0)
            CompleteEvent(CurrentDay());


        previousDayChecked = DayOfTheWeek();

    }

    public int CurrentDay()
    {
        switch (DayOfTheWeek())
        {
            case "Monday": return 0;
            case "Tuesday": return 1;
            case "Wednesday": return 2;
            case "Thursday": return 3;
            case "Friday": return 4;
            case "Saturday": return 5;
            case "Sunday": return 6;
        }
        return 0;
    }


    public void Click(int id)
    {
        switch (id)
        {
            case 0:
                currencies[id] += 1 + levels[id];
                break;
            case 1:
                currencies[id] += 1;
                break;
            case 2:
                currencies[id] += 1;
                break;
            case 3:
                currencies[id] += 1;
                break;
            case 4:
                currencies[id] += 1;
                break;
            case 5:
                currencies[id] += 1;
                break;
            case 6:
                currencies[id] += 1;
                break;

        }
    }

    public void Buy(int id)
    {
        if (currencies[id] < costs[id]) return;
        {
            currencies[id] -= costs[id];
            levels[id]++;
        }
    }

    public void ToggleEvent(int id)
    {
        var id2 = id;
        var data = game.data;
        DateTime now = DateTime.Now;
        //Start
        if (data.eventActiveID == 0 & data.eventCooldown[id2] <= 0 & !(now.Hour == 23 & now.Minute >= 55))
        {
            data.eventActiveID = id;
            data.eventCooldown[id2] = 300;

            currencies[id2] = 0;
            levels[id2] = 0;
        }
        else if (now.Hour == 23 & now.Minute >= 55 & data.eventActiveID == 0) return;
        else if (data.eventCooldown[id] > 0 & data.eventActiveID == 0) return;
        else//Exit
        {
            CompleteEvent(id2);
        }
    }

    public void CompleteEvent(int id)
    {
        var data = game.data;

        data.eventTokens += reward[id];
        eventRewardText.text = $"+{Methods.NotationMethod(reward[id], "F2")} Event Tokens";

        currencies[id] = 0;
        levels[id] = 0;
        data.eventActiveID = 0;
        data.eventCooldown[id] = 7200;

        eventRewardPopUp.gameObject.SetActive(true);
    }

  
    public void CloseEventReward()
    {
        eventRewardPopUp.gameObject.SetActive(false);
    }

    public void RunEventUI(int id)
    {
        var data = game.data;

        for (var i = 0; i < 7; i++)
            if (i == id)
                events[id].gameObject.SetActive(true);
            else
                events[i].gameObject.SetActive(false);


        var time = TimeSpan.FromSeconds(data.eventCooldown[id]);
        if (data.eventActiveID == 0)
        {
            startText[id].text = data.eventCooldown[id] > 0 ? time.ToString(@"hh\:mm\:ss") : "Start Event";
        }
        else
            startText[id].text = $"Exit Event ({time.ToString(@"hh\:mm\:ss")})";


        if (data.eventActiveID != id) return;
        eventsUnlocked[id].gameObject.SetActive(true);
        rewardText[id].text = $"+{Methods.NotationMethod(reward[id], "F2")} Event Tokens";
            
        if (id == 0)
            currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} Mondollars";
        else if (id == 1)
            currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} Tuesdollars";
        else if (id == 2)
            currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} Wednesdollars";
        else if (id == 3)
            currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} Thursdollars";
        else if (id == 4)
            currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} Fridollars";
        else if (id == 5)
            currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} Saturdollars";
        else
            currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} Sundollars";

        costText[id].text = $"Cost: {Methods.NotationMethod(costs[id], "F2")}";

    }

    public void RunEvent(int id)
    {
        switch (id)
        {
            case 0:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
            case 1:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
            case 2:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
            case 3:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
            case 4:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
            case 5:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
            case 6:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
        }
    }
}
