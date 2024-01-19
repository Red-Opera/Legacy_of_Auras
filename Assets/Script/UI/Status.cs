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
        Debug.Assert(state != null, "Error (Null Reference): �÷��̾� ���°� �������� �ʽ��ϴ�.");
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

    // �ʸ� ��, ��, �� ���·� ��ȯ�ϴ� �Լ�
    string FormatPlayTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int hours = Mathf.FloorToInt(minutes / 60);

        int remainingHours = hours % 24;
        int remainingMinutes = minutes % 60;
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", remainingHours, remainingMinutes, remainingSeconds);
    }
}
