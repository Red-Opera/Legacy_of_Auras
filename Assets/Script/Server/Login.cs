using System;
using System.Reflection;
using MySqlConnector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public string loginUrl = "";
    public static bool isStart = false;         // 현재 로그인 중인지 확인
    public static bool playPrologue = false;    // 프롤로그 실행 여부
    public static int currentLoginID = -1;      // 현재 로그인 중인 ID  
    public static string currentLoginName = ""; // 현재 로그인 중인 플레이어
    public static MySqlConnection conn;         // MySql 연결 컴포넌트

    [Header("Login Input")]
    [SerializeField] private TMP_InputField idText;         // ID 텍스트
    [SerializeField] private TMP_InputField passWordText;   // 비밀번호 텍스트

    [Header("Default Player Info")]
    [SerializeField] private PlayerState playerState;       // 초기 플레이어 상태

    private static bool isDisConnect = false;               // MySql 연결 여부
    private static Login loginObj = null;

    private Image idTextImage;                              // ID 텍스트 배경
    private Image passwordTextImage;                        // PassWord 텍스트 배경
    private TextMeshProUGUI idHintText;                     // ID 힌트 텍스트
    private TextMeshProUGUI passwordHintText;               // PassWord 텍스트 배경

    private int newId = 0;                          // 다음에 추가될 ID 값

    private void OnEnable()
    {
        Debug.Assert(idText != null, "Error (Null Reference) : 아이디 텍스트가 존재하지 않습니다.");
        Debug.Assert(passWordText != null, "Error (Null Reference) : 비밀번호 텍스트가 존재하지 않습니다.");

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
        // SQL 쿼리 문자열을 작성하여 PlayerInfo 테이블에서 플레이어 정보를 가져옴
        string query = "SELECT * FROM PlayerInfo WHERE Name = @Name";
        MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Name", idText.text);

        try
        {
            conn.Open();

            // 쿼리를 실행하고 MySqlDataReader 객체를 생성하여 결과를 읽어옴
            MySqlDataReader playerData = cmd.ExecuteReader();

            PlayerState playerState = Resources.Load<PlayerState>("Quest/PlayerState");
            Debug.Assert(playerState != null, "Error (Null Reference) : 플레이어 정보가 존재하지 않습니다.");

            if (playerData.Read())
            {
                currentLoginID = (int)playerData["ID"];
                playPrologue = Convert.ToBoolean(playerData["Prologue"]);

                // Reflection을 사용하여 필드를 파라미터로 추가
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

    // 게임 시작을 눌렸을 경우
    public void GameStart()
    {
        ResetColor();

        string query = "SELECT * FROM PlayerLogin WHERE Name = @Name";   // SQL 쿼리 문자열을 작성하여 PlayerLogin 테이블에서 특정 ID를 검색
        MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Name", idText.text);

        bool idExists = false;          // ID가 존재하는지 여부를 저장할 변수
        bool passwordMatches = false;   // 비밀번호가 일치하는지 여부를 저장할 변수

        try
        {
            conn.Open();

            // 쿼리를 실행하고 MySqlDataReader 객체를 생성하여 결과를 읽어옴
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                // ID가 존재함을 표시
                idExists = true;

                string password = dataReader["PassWord"].ToString().Replace("\u200B", "");
                string input = passWordText.text;

                // 데이터베이스의 비밀번호와 입력된 비밀번호를 비교
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

    // 해당 이름의 테이블의 다음 ID 값을 반환하는 메소드
    private int GetNextIdForTable(string tableName)
    {
        // 해당 테이블의 개수를 가져옴
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

    // 아이디 또는 비밀번호를 입력했는지 알려주는 메소드
    private bool IsNoIDOrPassWord()
    {
        // 아이디를 입력하지 않았을 경우
        if (idText.text == "")
        {
            idTextImage.color = new Color(1, idTextImage.color.b, idTextImage.color.g, idTextImage.color.a);

            return true;
        }

        // 비밀번호를 입력하지 않은 경우
        if (passWordText.text == "")
        {
            passwordTextImage.color = new Color(1, passwordTextImage.color.b, passwordTextImage.color.g, passwordTextImage.color.a);

            return true;
        }

        return false;
    }

    // 해당 이름의 플레이어가 이미 존재하는지 확인하는 메소드
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

        // 등록 성공
        if (currentCount == 0)
            return false;

        // 이미 있는 경우 반영
        idText.text = "";
        idHintText.text = "ID already exists..";
        idTextImage.color = new Color(1, idTextImage.color.b, idTextImage.color.g, idTextImage.color.a);

        return true;
    }

    // 플레이어 정보를 생성하는 메소드
    private MySqlCommand PlayerInfoCreate()
    {
        newId = GetNextIdForTable("PlayerLogin");

        // INSERT 쿼리 생성
        string infoQuery = "INSERT INTO PlayerInfo (ID, Name, Level, HP, Attack, AttackSpeed, Def, Money, Exp, BulletCount, BulletCurrentMax, PlayTotalTime, Kills, Prologue) " +
                       "VALUES(@ID, @Name, @Level, @HP, @Attack, @AttackSpeed, @Def, @Money, @Exp, @BulletCount, @BulletCurrentMax, @PlayTotalTime, @Kills, @Prologue);";
        MySqlCommand cmd = new MySqlCommand(infoQuery, conn);

        // Reflection을 사용하여 필드를 파라미터로 추가
        foreach (FieldInfo field in typeof(PlayerState).GetFields(BindingFlags.Public | BindingFlags.Instance))
            cmd.Parameters.AddWithValue("@" + (char.ToUpper(field.Name[0]) + field.Name[1..]), field.GetValue(playerState));

        // ID와 Name 파라미터 추가
        cmd.Parameters.AddWithValue("@ID", newId);
        cmd.Parameters.AddWithValue("@Name", idText.text);
        cmd.Parameters.AddWithValue("@Prologue", 0);

        return cmd;
    }

    // 플레이어 로그인 정보를 생성하는 메소드
    private MySqlCommand PlayerLoginCreate()
    {
        string loginQuery = "INSERT INTO PlayerLogin (ID, Name, PassWord) VALUES(@ID, @Name, @PassWord);";
        MySqlCommand cmd = new MySqlCommand(loginQuery, conn);

        cmd.Parameters.AddWithValue("@ID", newId);
        cmd.Parameters.AddWithValue("@Name", idText.text);
        cmd.Parameters.AddWithValue("@PassWord", passWordText.text);

        return cmd;
    }

    // 정보를 등록하는 메소드
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