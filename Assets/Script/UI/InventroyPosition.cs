using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using MySqlConnector;
using System;

public class InventroyPosition : MonoBehaviour
{
    public static GameObject inventory = null;  // 아이템 인벤토리 슬롯
    public GameObject itemDisplay;              // 아이템을 전시하기 위한 오브젝트
    public static float switchTime = 0.1f;      // 두 아이템들이 바뀌는데 걸리는 시간
    public bool isSwitch = false;               // 현재 두 아이템이 바뀌는 여부

    private List<GameObject> displayData;       // 현재 전시중인 아이템
    private Transform[] displayPos;             // 아이템을 전시할 수 있는 오브젝트
    private InventroyPosition instance;         // static 함수를 StartCoroutine을 할 수 있도록 도와주는 변수
    
    public static event System.Action<string, int> OnAddItem;   // 이 스크립트를 가지고 있는 모든 오브젝트가 실행할 이벤트
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
            // 아이템의 위치을 각각 오브젝트별로 알려줌
            displayPos[i].GetComponent<MoveInventory>().displayIndex = i;

            // 오브젝트를 차례대로 인벤토리에 넣음
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
        // 같은 장소로 이동하는 경우 중지
        if (displayIndex == dragIndex || isSwitch)
            return;

        // 인벤토리의 범위를 넘긴 경우
        if (displayData == null || displayIndex < 0 || dragIndex < 0 || displayIndex >= displayPos.Length || dragIndex >= displayPos.Length)
        {
            Debug.Assert(false, "Error (Out of Range) : 잘못된 인덱스가 전달되었습니다.");
            return;
        }

        // 아이템 위치를 빈공간으로 옮기는 경우
        if (displayPos[dragIndex].childCount == 0)
        {
            Transform moveInventory = displayPos[displayIndex].GetChild(0);

            moveInventory.GetComponent<MoveInventory>().displayIndex = dragIndex;
            moveInventory.SetParent(displayPos[dragIndex]);
        }

        // displayIndex와 dragIndex의 오브젝트를 교환
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

        // 이동시킬 오브젝트와 밀러난 아이템 위치를 천천히 중앙으로 옮김
        instance.StartCoroutine(UpdateDisplayPositions(displayIndex));
        instance.StartCoroutine(UpdateDisplayPositions(dragIndex));
    }

    // 아이템 위치를 정상적인 위치로 천천히 이동시키는 메소드
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
        displayPos[moveIndex].GetChild(0).localPosition = Vector3.zero; // 보장을 위해 최종 위치 설정

        isSwitch = false;
    }

    private void StartGameAddItem()
    {
        if (Login.currentLoginName == "" || isItemAdd)
            return;

        isItemAdd = true;

        string query = "SELECT * FROM PlayerItem WHERE Name = @Name";   // SQL 쿼리 문자열을 작성하여 PlayerLogin 테이블에서 특정 ID를 검색
        MySqlCommand cmd = new MySqlCommand(query, Login.conn);
        cmd.Parameters.AddWithValue("@Name", Login.currentLoginName);

        int[] itemNums = { 0, 0, 0, 0, 0, 0 };
        int[] counts = { 0, 0, 0, 0, 0, 0 };

        try
        {
            Login.conn.Open();

            // 쿼리를 실행하고 MySqlDataReader 객체를 생성하여 결과를 읽어옴
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

    // 아이템을 추가하거나 생성하는 함수
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

            Debug.Log("빈공간이 없습니다.");
        }
    }

    public static void CallAddItem(string name, int count)
    {
        OnAddItem(name, count);
    }
}