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

                // ü���� �ø� ��� ���� ü���� ������
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

    // ������ �ۼ�Ʈ�� �ٲ��ִ� �޼ҵ�
    private T ConvertPersent<T>(T value, T left)
    {
        dynamic val = value;

        if (val is int intValue && intValue < 0)
        {
            // value�� ������ ���
            double percent = Math.Abs(intValue) / (double)100;

            if (typeof(T) == typeof(int))
                return (T)(object)Convert.ToInt32(Convert.ToDouble(left) * percent);

            else if (typeof(T) == typeof(float))
                return (T)(object)((float)(Convert.ToDouble(left) * percent));

            else if (typeof(T) == typeof(double))
                return (T)(object)((double)(Convert.ToDouble(left) * percent));
        }

        // value�� ����̰ų� T�� int �Ǵ� float�� �ƴ� ���
        return value;
    }
}
