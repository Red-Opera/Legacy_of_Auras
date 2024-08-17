using System;
using System.Reflection;
using MySqlConnector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public string loginUrl = "";
    public static bool isStart = false;         // ���� �α��� ������ Ȯ��
    public static bool playPrologue = false;    // ���ѷα� ���� ����
    public static int currentLoginID = -1;      // ���� �α��� ���� ID  
    public static string currentLoginName = ""; // ���� �α��� ���� �÷��̾�
    public static MySqlConnection conn;         // MySql ���� ������Ʈ

    [Header("Login Input")]
    [SerializeField] private TMP_InputField idText;         // ID �ؽ�Ʈ
    [SerializeField] private TMP_InputField passWordText;   // ��й�ȣ �ؽ�Ʈ

    [Header("Default Player Info")]
    [SerializeField] private PlayerState playerState;       // �ʱ� �÷��̾� ����

    private static bool isDisConnect = false;               // MySql ���� ����
    private static Login loginObj = null;

    private Image idTextImage;                              // ID �ؽ�Ʈ ���
    private Image passwordTextImage;                        // PassWord �ؽ�Ʈ ���
    private TextMeshProUGUI idHintText;                     // ID ��Ʈ �ؽ�Ʈ
    private TextMeshProUGUI passwordHintText;               // PassWord �ؽ�Ʈ ���

    private int newId = 0;                          // ������ �߰��� ID ��

    private void OnEnable()
    {
        Debug.Assert(idText != null, "Error (Null Reference) : ���̵� �ؽ�Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(passWordText != null, "Error (Null Reference) : ��й�ȣ �ؽ�Ʈ�� �������� �ʽ��ϴ�.");

        idTextImage = idText.GetComponent<Image>();
        passwordTextImage = passWordText.GetComponent<Image>();

        idHintText = idText.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        passwordHintText = passWordText.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

        loginObj = this;
    }

    private void SQLConnect()
    {
        conn = new MySqlConnection(loginUrl);

        try
        {
            conn.Open();
            Debug.Log("Database connection established successfully.");
        }

        catch (Exception e)
        {
            Debug.LogError("Failed to connect to database: " + e.Message);
        }

        finally
        {
            conn.Close();
        }
    }

    private bool HasColumn(MySqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private void SetPlayerStat()
    {
        // SQL ���� ���ڿ��� �ۼ��Ͽ� PlayerInfo ���̺��� �÷��̾� ������ ������
        string query = "SELECT * FROM PlayerInfo WHERE Name = @Name";
        MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Name", idText.text);

        try
        {
            conn.Open();

            // ������ �����ϰ� MySqlDataReader ��ü�� �����Ͽ� ����� �о��
            MySqlDataReader playerData = cmd.ExecuteReader();

            PlayerState playerState = Resources.Load<PlayerState>("Quest/PlayerState");
            Debug.Assert(playerState != null, "Error (Null Reference) : �÷��̾� ������ �������� �ʽ��ϴ�.");

            if (playerData.Read())
            {
                currentLoginID = (int)playerData["ID"];
                playPrologue = Convert.ToBoolean(playerData["Prologue"]);

                // Reflection�� ����Ͽ� �ʵ带 �Ķ���ͷ� �߰�
                foreach (FieldInfo field in typeof(PlayerState).GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    string fieldName = char.ToUpper(field.Name[0]) + field.Name[1..];

                    if (HasColumn(playerData, fieldName) && playerData[fieldName] != DBNull.Value)
                    {
                        object value = playerData[fieldName];
                        field.SetValue(playerState, Convert.ChangeType(value, field.FieldType));
                    }
                }
            }
        }

        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute query for PlayerInfo: " + ex.Message);
        }

        finally
        {
            conn.Close();
        }
    }

    // ���� ������ ������ ���
    public void GameStart()
    {
        ResetColor();

        string query = "SELECT * FROM PlayerLogin WHERE Name = @Name";   // SQL ���� ���ڿ��� �ۼ��Ͽ� PlayerLogin ���̺��� Ư�� ID�� �˻�
        MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Name", idText.text);

        bool idExists = false;          // ID�� �����ϴ��� ���θ� ������ ����
        bool passwordMatches = false;   // ��й�ȣ�� ��ġ�ϴ��� ���θ� ������ ����

        try
        {
            conn.Open();

            // ������ �����ϰ� MySqlDataReader ��ü�� �����Ͽ� ����� �о��
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                // ID�� �������� ǥ��
                idExists = true;

                string password = dataReader["PassWord"].ToString().Replace("\u200B", "");
                string input = passWordText.text;

                // �����ͺ��̽��� ��й�ȣ�� �Էµ� ��й�ȣ�� ��
                if (password.Equals(input))
                    passwordMatches = true;
            }

            if (!idExists)
            {
                idText.text = "";
                idHintText.text = "This ID does not exist...";
                idTextImage.color = new Color(1, idTextImage.color.b, idTextImage.color.g, idTextImage.color.a);
            }

            else if (!passwordMatches)
            {
                passWordText.text = "";
                passwordHintText.text = "Password does not match...";
                passwordTextImage.color = new Color(1, passwordTextImage.color.b, passwordTextImage.color.g, passwordTextImage.color.a);
            }
        }

        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute query for PlayerInfo: " + ex.Message);
        }

        finally
        {
            conn.Close();

            if (idExists && passwordMatches)
            {
                SetPlayerStat();
                currentLoginName = idText.text;
                isStart = true;
            }
        }
    }

    // �ش� �̸��� ���̺��� ���� ID ���� ��ȯ�ϴ� �޼ҵ�
    private int GetNextIdForTable(string tableName)
    {
        // �ش� ���̺��� ������ ������
        string countQuery = $"SELECT COUNT(*) FROM {tableName};";
        MySqlCommand countCmd = new MySqlCommand(countQuery, conn);

        int currentCount = 0;

        try
        {
            conn.Open();
            currentCount = Convert.ToInt32(countCmd.ExecuteScalar());
        }

        catch (Exception e)
        {
            Debug.LogError($"Failed to execute count query for {tableName}: " + e.Message);
        }

        finally
        {
            conn.Close();
        }

        return currentCount + 1;
    }

    private void ResetText()
    {
        idText.text = "";
        passWordText.text = "";

        ResetColor();
    }

    private void ResetColor()
    {
        idTextImage.color = new Color(idTextImage.color.b, idTextImage.color.b, idTextImage.color.g, idTextImage.color.a);
        idHintText.text = "Enter ID...";

        passwordTextImage.color = new Color(passwordTextImage.color.b, passwordTextImage.color.b, passwordTextImage.color.g, passwordTextImage.color.a);
        passwordHintText.text = "Enter PassWord...";
    }

    // ���̵� �Ǵ� ��й�ȣ�� �Է��ߴ��� �˷��ִ� �޼ҵ�
    private bool IsNoIDOrPassWord()
    {
        // ���̵� �Է����� �ʾ��� ���
        if (idText.text == "")
        {
            idTextImage.color = new Color(1, idTextImage.color.b, idTextImage.color.g, idTextImage.color.a);

            return true;
        }

        // ��й�ȣ�� �Է����� ���� ���
        if (passWordText.text == "")
        {
            passwordTextImage.color = new Color(1, passwordTextImage.color.b, passwordTextImage.color.g, passwordTextImage.color.a);

            return true;
        }

        return false;
    }

    // �ش� �̸��� �÷��̾ �̹� �����ϴ��� Ȯ���ϴ� �޼ҵ�
    private bool IsExistPlayer(string playerName)
    {
        string loginQuery = "SELECT COUNT(*) FROM PlayerLogin WHERE Name='" + playerName + "';";
        MySqlCommand cmd = new MySqlCommand(loginQuery, conn);

        int currentCount = -1;
        try
        {
            conn.Open();

            currentCount = Convert.ToInt32(cmd.ExecuteScalar());
        }

        catch (Exception e)
        {
            Debug.LogError($"Failed to execute count query for PlayerLogin : " + e.Message);
        }

        finally
        {
            conn.Close();
        }

        // ��� ����
        if (currentCount == 0)
            return false;

        // �̹� �ִ� ��� �ݿ�
        idText.text = "";
        idHintText.text = "ID already exists..";
        idTextImage.color = new Color(1, idTextImage.color.b, idTextImage.color.g, idTextImage.color.a);

        return true;
    }

    // �÷��̾� ������ �����ϴ� �޼ҵ�
    private MySqlCommand PlayerInfoCreate()
    {
        newId = GetNextIdForTable("PlayerLogin");

        // INSERT ���� ����
        string infoQuery = "INSERT INTO PlayerInfo (ID, Name, Level, HP, Attack, AttackSpeed, Def, Money, Exp, BulletCount, BulletCurrentMax, PlayTotalTime, Kills, Prologue) " +
                       "VALUES(@ID, @Name, @Level, @HP, @Attack, @AttackSpeed, @Def, @Money, @Exp, @BulletCount, @BulletCurrentMax, @PlayTotalTime, @Kills, @Prologue);";
        MySqlCommand cmd = new MySqlCommand(infoQuery, conn);

        // Reflection�� ����Ͽ� �ʵ带 �Ķ���ͷ� �߰�
        foreach (FieldInfo field in typeof(PlayerState).GetFields(BindingFlags.Public | BindingFlags.Instance))
            cmd.Parameters.AddWithValue("@" + (char.ToUpper(field.Name[0]) + field.Name[1..]), field.GetValue(playerState));

        // ID�� Name �Ķ���� �߰�
        cmd.Parameters.AddWithValue("@ID", newId);
        cmd.Parameters.AddWithValue("@Name", idText.text);
        cmd.Parameters.AddWithValue("@Prologue", 0);

        return cmd;
    }

    // �÷��̾� �α��� ������ �����ϴ� �޼ҵ�
    private MySqlCommand PlayerLoginCreate()
    {
        string loginQuery = "INSERT INTO PlayerLogin (ID, Name, PassWord) VALUES(@ID, @Name, @PassWord);";
        MySqlCommand cmd = new MySqlCommand(loginQuery, conn);

        cmd.Parameters.AddWithValue("@ID", newId);
        cmd.Parameters.AddWithValue("@Name", idText.text);
        cmd.Parameters.AddWithValue("@PassWord", passWordText.text);

        return cmd;
    }

    // ������ ����ϴ� �޼ҵ�
    public void GameRegister()
    {
        ResetColor();

        if (IsNoIDOrPassWord())
            return;

        if (IsExistPlayer(idText.text))
            return;

        for (int i = 0; i < 2; i++)
        {
            MySqlCommand cmd = null;

            if (i == 0)
                cmd = PlayerInfoCreate();

            else if (i == 1)
                cmd = PlayerLoginCreate();

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Debug.Log("Player registered successfully with ID: " + cmd.Parameters["@ID"].Value);
            }

            catch (Exception e)
            {
                Debug.LogError("Failed to execute query: " + e.Message);
            }

            finally
            {
                conn.Close();
            }
        }

        ResetText();
    }

    public static void GameLogin()
    {
        loginObj.SQLConnect();
    }
}