using MySqlConnector;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void SaveDefault()
    {
        // INSERT 쿼리 생성
        string infoQuery = "UPDATE PlayerInfo SET Level=@Level, HP=@HP, Attack=@Attack, AttackSpeed=@AttackSpeed, Def=@Def, Money=@Money, Exp=@Exp, " +
                           "BulletCount=@BulletCount, BulletCurrentMax=@BulletCurrentMax, PlayTotalTime=@PlayTotalTime, Kills=@Kills, Prologue=@Prologue WHERE Name=@Name;";

        MySqlCommand cmd = new MySqlCommand(infoQuery, Login.conn);
        cmd.Parameters.AddWithValue("@Name", Login.currentLoginName);
        cmd.Parameters.AddWithValue("@Prologue", Login.playPrologue ? 1 : 0);

        PlayerState playerState = Resources.Load<PlayerState>("Quest/PlayerState");
        Debug.Assert(playerState != null, "Error (Null Reference) : 플레이어 정보가 존재하지 않습니다.");

        // Reflection을 사용하여 필드를 파라미터로 추가
        foreach (FieldInfo field in typeof(PlayerState).GetFields(BindingFlags.Public | BindingFlags.Instance))
            cmd.Parameters.AddWithValue("@" + (char.ToUpper(field.Name[0]) + field.Name[1..]), field.GetValue(playerState));

        try
        {
            Login.conn.Open();
            cmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute query for PlayerInfo: " + ex.Message);
        }

        finally
        {
            Login.conn.Close();
        }
    }

    private void SaveItem()
    {
        // INSERT 쿼리 생성
        string infoQuery = "INSERT INTO PlayerItem (ID, Name, ItemNum1, Count1, ItemNum2, Count2, ItemNum3, Count3, ItemNum4, Count4, ItemNum5, Count5) " +
                           "VALUES(@ID, @Name, @ItemNum1, @Count1, @ItemNum2, @Count2, @ItemNum3, @Count3, @ItemNum4, @Count4, @ItemNum5, @Count5) " +
                           "ON DUPLICATE KEY UPDATE " +
                           "Name = VALUES(Name), " +
                           "ItemNum1 = VALUES(ItemNum1), " + "Count1 = VALUES(Count1), " +
                           "ItemNum2 = VALUES(ItemNum2), " + "Count2 = VALUES(Count2), " +
                           "ItemNum3 = VALUES(ItemNum3), " + "Count3 = VALUES(Count3), " +
                           "ItemNum4 = VALUES(ItemNum4), " + "Count4 = VALUES(Count4), " +
                           "ItemNum5 = VALUES(ItemNum5), " + "Count5 = VALUES(Count5);";

        MySqlCommand cmd = new MySqlCommand(infoQuery, Login.conn);
        cmd.Parameters.AddWithValue("@ID", Login.currentLoginID);
        cmd.Parameters.AddWithValue("@Name", Login.currentLoginName);

        if (InventroyPosition.inventory == null)
            return;

        // 기본적으로 모든 아이템 번호와 수량을 NULL로 설정
        for (int i = 1; i <= 5; i++)
        {
            cmd.Parameters.AddWithValue("@ItemNum" + i, DBNull.Value);
            cmd.Parameters.AddWithValue("@Count" + i, DBNull.Value);
        }

        // Reflection을 사용하여 필드를 파라미터로 추가
        for (int i = 0; i < InventroyPosition.inventory.transform.childCount; i++)
        {
            Transform slot = InventroyPosition.inventory.transform.GetChild(i);

            if (slot.childCount <= 0)
                continue;

            string itemName = slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite.name;
            int itemCount = int.Parse(slot.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text);

            int itemID = -1;
            if (itemName == "512x_beaker_red")
                itemID = 1;

            else if (itemName == "512x_beaker_blue")
                itemID = 2;

            cmd.Parameters["@ItemNum" + (i + 1)].Value = itemID;
            cmd.Parameters["@Count" + (i + 1)].Value = itemCount;
        }

        try
        {
            Login.conn.Open();
            cmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute query for PlayerItem: " + ex.Message);
        }

        finally
        {
            Login.conn.Close();
        }
    }

    private void OnApplicationQuit()
    {
        if (Login.currentLoginName == "")
            return;

        SaveDefault();
        SaveItem();
    }

    private void OnDestroy()
    {
        OnApplicationQuit();
    }
}
