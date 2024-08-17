using System;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState", menuName = "State/PlayerState")]
public class PlayerState : State
{
    public int money;
    public int exp;

    public int bulletCount;
    public int bulletCurrentMax;

    public float playTotalTime;
    public int kills;

    public void Add(PlayerState otherState)
    {
        FieldInfo[] fields = typeof(PlayerState).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.Name == "exp")
                continue;

            if (field.FieldType == typeof(int))
            {
                int value = (int)field.GetValue(otherState);
                value = ConvertPersent<int>(value, (int)field.GetValue(this));

                // 체력을 늘릴 경우 현재 체력이 증가함
                if (field.Name == "HP")
                    GameObject.Find("UI").GetComponent<PlayerHPBar>().Heal(value);

                else
                    field.SetValue(this, (int)field.GetValue(this) + value);
            }

            else if (field.FieldType == typeof(float))
            {
                float value = (float)field.GetValue(otherState);
                value = ConvertPersent<float>(value, (float)field.GetValue(this));
                field.SetValue(this, (float)field.GetValue(this) + value);
            }

            else if (field.FieldType == typeof(double))
            {
                double value = (double)field.GetValue(otherState);
                value = ConvertPersent<double>(value, (double)field.GetValue(this));
                field.SetValue(this, (double)field.GetValue(this) + value);
            }
        }
    }

    // 음수를 퍼센트로 바꿔주는 메소드
    private T ConvertPersent<T>(T value, T left)
    {
        dynamic val = value;

        if (val is int intValue && intValue < 0)
        {
            // value가 음수인 경우
            double percent = Math.Abs(intValue) / (double)100;

            if (typeof(T) == typeof(int))
                return (T)(object)Convert.ToInt32(Convert.ToDouble(left) * percent);

            else if (typeof(T) == typeof(float))
                return (T)(object)((float)(Convert.ToDouble(left) * percent));

            else if (typeof(T) == typeof(double))
                return (T)(object)((double)(Convert.ToDouble(left) * percent));
        }

        // value가 양수이거나 T가 int 또는 float가 아닌 경우
        return value;
    }
}
