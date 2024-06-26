using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SpriteToItem
{
    public Sprite sprite;
    public GameObject item;
}

public class ItemSelect : MonoBehaviour
{
    [SerializeField] private GameObject inventory;              // �κ��丮
    [SerializeField] private GameObject itemContainer;          // �տ� �� �� �ִ� �������� �����̳�
    [SerializeField] private List<SpriteToItem> imageToItem;    // ��������Ʈ�� ������Ʈ�� ��ȯ�ϴ� Ŭ����

    private Transform[] items;                  // �տ� �� �� �ִ� ������ ��ġ
    private Transform[] inventorySlots;         // ������ ���� ��ġ
    private Image[] slotsImage;                 // ���� �̹���
    private PlayerWeaponChanger playerAttack;   // �÷��̾� ���� ��ũ��Ʈ
    private Animator animator;                  // �÷��̾� �ִϸ�����
    private int currentSelect = -1;             // ���� ������ �̹���

    private void Start()
    {
        Debug.Assert(inventory != null, "Error (Null Reference) : �κ��丮�� �������� �ʽ��ϴ�.");

        playerAttack = gameObject.GetComponent<PlayerWeaponChanger>();
        Debug.Assert(playerAttack != null, "Error (Null Reference) : �÷��̾� ���� ��ũ��Ʈ�� �������� �ʽ��ϴ�.");

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �÷��̾� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        items = new Transform[itemContainer.transform.childCount];
        inventorySlots = new Transform[inventory.transform.childCount];
        slotsImage = new Image[inventorySlots.Length];

        for (int i = 0; i < items.Length; i++)
            items[i] = itemContainer.transform.GetChild(i);

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i] = inventory.transform.GetChild(i);
            slotsImage[i] = inventorySlots[i].GetComponent<Image>();
        }
    }

    private void Update()
    {
        bool isEquidItem = animator.GetCurrentAnimatorStateInfo(1).IsName("EquidItem") || animator.IsInTransition(1);

        // �̹� �����ӿ� ���� ���� ��ȣ
        int select = -1;

        // �÷��̾ �޴��� �� ���
        if (PlayerMenu.isMenu)
        {
            if (currentSelect < 0)
                return;

            // �÷��̾ ������ ��ġ�� �ٲ㼭 �������� ���� ���
            if (inventorySlots[currentSelect].childCount == 0)
                select = currentSelect;

            else
                ShowItem();
        }

        else if (playerAttack.weaponType > WeaponType.NULL && playerAttack.weaponType < WeaponType.ITEM1 || isEquidItem)
            return;

        if (Input.GetKeyDown(KeyCode.Z))
            select = 0;

        else if (Input.GetKeyDown(KeyCode.X))
            select = 1;

        else if (Input.GetKeyDown(KeyCode.C))
            select = 2;

        else if (Input.GetKeyDown(KeyCode.V))
            select = 3;

        else if (Input.GetKeyDown(KeyCode.B))
            select = 4;

        // ���� ������ ������ ���
        if (select >= 0 && select == currentSelect)
        {
            currentSelect = -1;
            playerAttack.weaponType = WeaponType.NULL;

            // ��� �ִ� ������ ��� ��Ȱ��ȭ
            for (int i = 0; i < items.Length; i++)
                items[i].gameObject.SetActive(false);

            // ������ ���� ���� ����
            for (int i = 0; i < slotsImage.Length; i++)
                slotsImage[i].color = Color.white;

            animator.SetTrigger("EquidItem");
        }

        // �ٸ� ������ ������ ���
        else if (select >= 0 && select != currentSelect)
        {
            currentSelect = select;
            playerAttack.weaponType = WeaponType.ITEM1 + select;

            Select();
        }
    }

    private void Select()
    {
        // �ش� ��ġ�� �������� ���� ���
        if (inventorySlots[currentSelect].childCount <= 0)
            return;

        for (int i = 0; i < slotsImage.Length; i++)
            slotsImage[i].color = Color.white;
        slotsImage[currentSelect].color = Color.red;

        animator.SetTrigger("EquidItem");

        ShowItem();
    }

    private void ShowItem()
    {
        Transform inventoryItem = inventorySlots[currentSelect].GetChild(0);

        Sprite itemSprite = inventoryItem.GetChild(0).GetComponent<Image>().sprite;

        // ��������Ʈ�� ���������� ��ȯ��
        GameObject showObject = null;
        for (int i = 0; i < imageToItem.Count; i++)
        {
            if (imageToItem[i].sprite == itemSprite)
            {
                showObject = imageToItem[i].item;
                break;
            }
        }

        Debug.Assert(showObject != null, "Error (Null Reference) : �ش� �̹����� �������� �������� �ʽ��ϴ�.");

        // ��� �ִ� ������ ��� ��Ȱ��ȭ
        for (int i = 0; i < items.Length; i++)
            items[i].gameObject.SetActive(false);

        // ��ȯ�� �������� ��� �ִ� ������ �� �ϳ��� Ȱ��ȭ ��
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].gameObject == showObject)
            {
                items[i].gameObject.SetActive(true);
                break;
            }
        }    
    }

    public void ItemUseReflection()
    {
        Transform inventoryItem = inventorySlots[currentSelect].GetChild(0);
        TextMeshProUGUI remainText = inventoryItem.GetChild(1).GetComponent<TextMeshProUGUI>();

        int count = int.Parse(remainText.text);
        count--;

        if (count == 0)
        {
            Destroy(inventorySlots[currentSelect].GetChild(0).gameObject);

            currentSelect = -1;
            playerAttack.weaponType = WeaponType.NULL;

            // ��� �ִ� ������ ��� ��Ȱ��ȭ
            for (int i = 0; i < items.Length; i++)
                items[i].gameObject.SetActive(false);

            // ������ ���� ���� ����
            for (int i = 0; i < slotsImage.Length; i++)
                slotsImage[i].color = Color.white;
        }

        else
            remainText.text = count.ToString();
    }
}