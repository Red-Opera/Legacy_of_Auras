using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using MySqlConnector;
using System;

public class InventroyPosition : MonoBehaviour
{
    public static GameObject inventory = null;  // ������ �κ��丮 ����
    public GameObject itemDisplay;              // �������� �����ϱ� ���� ������Ʈ
    public static float switchTime = 0.1f;      // �� �����۵��� �ٲ�µ� �ɸ��� �ð�
    public bool isSwitch = false;               // ���� �� �������� �ٲ�� ����

    private List<GameObject> displayData;       // ���� �������� ������
    private Transform[] displayPos;             // �������� ������ �� �ִ� ������Ʈ
    private InventroyPosition instance;         // static �Լ��� StartCoroutine�� �� �� �ֵ��� �����ִ� ����
    
    public static event System.Action<string, int> OnAddItem;   // �� ��ũ��Ʈ�� ������ �ִ� ��� ������Ʈ�� ������ �̺�Ʈ
    public List<Sprite> spriteData = new List<Sprite>();
    private Dictionary<string, Sprite> sprites;

    private static bool isItemAdd = false;
    private bool isOn = true;

    private void Awake()
    {
        instance = this;

        displayPos = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            displayPos[i] = transform.GetChild(i);

        displayData = new List<GameObject>();

        for (int i = 0; i < displayData.Count; i++)
        {
            // �������� ��ġ�� ���� ������Ʈ���� �˷���
            displayPos[i].GetComponent<MoveInventory>().displayIndex = i;

            // ������Ʈ�� ���ʴ�� �κ��丮�� ����
            displayPos[i].transform.SetParent(displayPos[i]);
            displayPos[i].transform.localPosition = Vector3.zero;
        }

        sprites = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in spriteData)
            sprites.Add(sprite.name, sprite);

        inventory = gameObject;

        OnAddItem += AddItem;
    }

    private void Start()
    {
        StartGameAddItem();
    }

    private void Update()
    {
        if (isOn && !BossSceneFilm.isFilmEnd)
        {
            isOn = false;
            
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        else if (!isOn && BossSceneFilm.isFilmEnd)
        {
            isOn = true;

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void ChangePos(int displayIndex, int dragIndex)
    {
        // ���� ��ҷ� �̵��ϴ� ��� ����
        if (displayIndex == dragIndex || isSwitch)
            return;

        // �κ��丮�� ������ �ѱ� ���
        if (displayData == null || displayIndex < 0 || dragIndex < 0 || displayIndex >= displayPos.Length || dragIndex >= displayPos.Length)
        {
            Debug.Assert(false, "Error (Out of Range) : �߸��� �ε����� ���޵Ǿ����ϴ�.");
            return;
        }

        // ������ ��ġ�� ��������� �ű�� ���
        if (displayPos[dragIndex].childCount == 0)
        {
            Transform moveInventory = displayPos[displayIndex].GetChild(0);

            moveInventory.GetComponent<MoveInventory>().displayIndex = dragIndex;
            moveInventory.SetParent(displayPos[dragIndex]);
        }

        // displayIndex�� dragIndex�� ������Ʈ�� ��ȯ
        else
        {
            if (displayPos[displayIndex].childCount <= 0 || displayPos[dragIndex].childCount <= 0)
                return;

            MoveInventory aMoveInventory = displayPos[displayIndex].GetChild(0).GetComponent<MoveInventory>();
            MoveInventory bMoveInventory = displayPos[dragIndex].GetChild(0).GetComponent<MoveInventory>();

            int a = aMoveInventory.displayIndex;
            int b = bMoveInventory.displayIndex;

            bMoveInventory.displayIndex = a;
            displayPos[dragIndex].GetChild(0).SetParent(displayPos[a]);

            aMoveInventory.displayIndex = b;
            displayPos[displayIndex].GetChild(0).SetParent(displayPos[b]);
        }

        // �̵���ų ������Ʈ�� �з��� ������ ��ġ�� õõ�� �߾����� �ű�
        instance.StartCoroutine(UpdateDisplayPositions(displayIndex));
        instance.StartCoroutine(UpdateDisplayPositions(dragIndex));
    }

    // ������ ��ġ�� �������� ��ġ�� õõ�� �̵���Ű�� �޼ҵ�
    private IEnumerator UpdateDisplayPositions(int moveIndex)
    {
        isSwitch = true;

        if (displayPos[moveIndex].childCount <= 0)
            yield break;

        float elapsedTime = 0f;
        Vector3 initialPosition = displayPos[moveIndex].GetChild(0).localPosition;

        while (elapsedTime < switchTime)
        {
            if (displayPos[moveIndex].childCount <= 0)
                yield break;

            displayPos[moveIndex].GetChild(0).localPosition = Vector3.Lerp(initialPosition, Vector3.zero, elapsedTime / switchTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (displayPos[moveIndex].childCount <= 0)
            yield break;
        displayPos[moveIndex].GetChild(0).localPosition = Vector3.zero; // ������ ���� ���� ��ġ ����

        isSwitch = false;
    }

    private void StartGameAddItem()
    {
        if (Login.currentLoginName == "" || isItemAdd)
            return;

        isItemAdd = true;

        string query = "SELECT * FROM PlayerItem WHERE Name = @Name";   // SQL ���� ���ڿ��� �ۼ��Ͽ� PlayerLogin ���̺��� Ư�� ID�� �˻�
        MySqlCommand cmd = new MySqlCommand(query, Login.conn);
        cmd.Parameters.AddWithValue("@Name", Login.currentLoginName);

        int[] itemNums = { 0, 0, 0, 0, 0, 0 };
        int[] counts = { 0, 0, 0, 0, 0, 0 };

        try
        {
            Login.conn.Open();

            // ������ �����ϰ� MySqlDataReader ��ü�� �����Ͽ� ����� �о��
            using MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.Read())
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (dataReader["ItemNum" + i] == DBNull.Value)
                        continue;

                    int itemNum = dataReader.GetInt32("ItemNum" + i);
                    int count = dataReader.GetInt32("Count" + i);

                    itemNums[i] = itemNum;
                    counts[i] = count;
                }
            }

            else
            {
                Debug.LogWarning("No player item data found for current login name.");
            }
        }

        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute query for PlayerInfo: " + ex.Message);
        }

        finally
        {
            Login.conn.Close();
        }

        for (int i = 1; i <= 5; i++)
        {
            if (itemNums[i] == 0)
                continue;

            string itemName = "";
            if (itemNums[i] == 1)
                itemName = "HPPotion";

            else if (itemNums[i] == 2)
                itemName = "ManaPotion";

            CallAddItem(itemName, counts[i]);
        }
    }

    // �������� �߰��ϰų� �����ϴ� �Լ�
    public void AddItem(string name, int count)
    {
        List<string> hasImage = new List<string>();

        foreach (GameObject item in displayData)
            hasImage.Add(item.transform.GetChild(0).GetComponent<Image>().sprite.name);

        string replace = "";

        if (name == "HPPotion")
            replace = "512x_beaker_red";

        else if (name == "ManaPotion")
            replace = "512x_beaker_blue";

        bool hasItem = false;
        int whatIndex = 0, hasCount = 0;
        for (int i = 0; i < hasImage.Count; i++)
        {
            if (hasImage[i] == replace)
            {
                hasItem = true;
                whatIndex = i;
                hasCount = int.Parse(displayData[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);

                break;
            }
        }

        if (hasItem)
        {
            hasCount += count;
            displayData[whatIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = hasCount.ToString();
        }

        else
        {
            foreach (Transform slot in displayPos)
            {
                if (slot.childCount <= 0)
                {
                    GameObject newItem = Instantiate(itemDisplay);
                    newItem.transform.GetChild(0).GetComponent<Image>().sprite = sprites[replace];
                    newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = count.ToString();

                    newItem.transform.SetParent(slot);
                    newItem.transform.localPosition = Vector3.zero;

                    displayData.Add(newItem);
                    return;
                }
            }

            Debug.Log("������� �����ϴ�.");
        }
    }

    public static void CallAddItem(string name, int count)
    {
        OnAddItem(name, count);
    }
}