using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class InventroyPosition : MonoBehaviour
{
    public GameObject itemDisplay;              // 아이템을 전시하기 위한 오브젝트
    public static float swithTime = 0.1f;       // 두 아이템들이 바뀌는데 걸리는 시간

    private List<GameObject> displayData;       // 현재 전시중인 아이템
    private Transform[] displayPos;             // 아이템을 전시할 수 있는 오브젝트
    private InventroyPosition instance;         // static 함수를 StartCoroutine을 할 수 있도록 도와주는 변수
    
    public static event System.Action<string, int> OnAddItem;   // 이 스크립트를 가지고 있는 모든 오브젝트가 실행할 이벤트
    public List<Sprite> spriteData = new List<Sprite>();
    private Dictionary<string, Sprite> sprites;

    public void Awake()
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

        OnAddItem += AddItem;
    }

    public void Update()
    {

    }

    public void ChangePos(int displayIndex, int dragIndex)
    {
        // 같은 장소로 이동하는 경우 중지
        if (displayIndex == dragIndex)
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
        if (displayPos[moveIndex].childCount <= 0)
            yield break;

        float elapsedTime = 0f;
        Vector3 initialPosition = displayPos[moveIndex].GetChild(0).localPosition;

        while (elapsedTime < swithTime)
        {
            if (displayPos[moveIndex].childCount <= 0)
                yield break;

            displayPos[moveIndex].GetChild(0).localPosition = Vector3.Lerp(initialPosition, Vector3.zero, elapsedTime / swithTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (displayPos[moveIndex].childCount <= 0)
            yield break;
        displayPos[moveIndex].GetChild(0).localPosition = Vector3.zero; // 보장을 위해 최종 위치 설정
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