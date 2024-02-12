using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class InventroyPosition : MonoBehaviour
{
    public GameObject itemDisplay;              // �������� �����ϱ� ���� ������Ʈ
    public static float swithTime = 0.1f;       // �� �����۵��� �ٲ�µ� �ɸ��� �ð�

    private List<GameObject> displayData;       // ���� �������� ������
    private Transform[] displayPos;             // �������� ������ �� �ִ� ������Ʈ
    private InventroyPosition instance;         // static �Լ��� StartCoroutine�� �� �� �ֵ��� �����ִ� ����
    
    public static event System.Action<string, int> OnAddItem;   // �� ��ũ��Ʈ�� ������ �ִ� ��� ������Ʈ�� ������ �̺�Ʈ
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
            // �������� ��ġ�� ���� ������Ʈ���� �˷���
            displayPos[i].GetComponent<MoveInventory>().displayIndex = i;

            // ������Ʈ�� ���ʴ�� �κ��丮�� ����
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
        // ���� ��ҷ� �̵��ϴ� ��� ����
        if (displayIndex == dragIndex)
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
        displayPos[moveIndex].GetChild(0).localPosition = Vector3.zero; // ������ ���� ���� ��ġ ����
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