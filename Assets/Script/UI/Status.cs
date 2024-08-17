using System;
using TMPro;
using UnityEngine;

public class Status : MonoBehaviour
{
    public PlayerState state;

    public TextMeshProUGUI level;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI defense;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI kills;

    public TextMeshProUGUI totalTime;

    public void Start()
    {
        Debug.Assert(state != null, "Error (Null Reference): 플레이어 상태가 존재하지 않습니다.");
    }

    public void Update()
    {
        level.SetText(state.Level.ToString());
        hp.SetText(state.HP.ToString());
        attack.SetText(state.attack.ToString());
        defense.SetText(state.def.ToString());
        gold.SetText(state.money.ToString());
        kills.SetText(state.kills.ToString());
        totalTime.SetText(FormatPlayTime(state.playTotalTime));
    }

    // 초를 시, 분, 초 형태로 변환하는 함수
    string FormatPlayTime(double seconds)
    {
        int totalSeconds = (int)Math.Floor(seconds);
        int minutes = totalSeconds / 60;
        int hours = minutes / 60;

        int remainingHours = hours;
        int remainingMinutes = minutes % 60;
        int remainingSeconds = totalSeconds % 60;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", remainingHours, remainingMinutes, remainingSeconds);
    }
}
