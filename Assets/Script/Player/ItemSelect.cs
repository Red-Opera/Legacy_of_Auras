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
    [SerializeField] private GameObject inventory;              // 인벤토리
    [SerializeField] private GameObject itemContainer;          // 손에 들 수 있는 아이템의 컨테이너
    [SerializeField] private List<SpriteToItem> imageToItem;    // 스프라이트를 오브젝트로 변환하는 클래스

    private Transform[] items;                  // 손에 들 수 있는 아이템 위치
    private Transform[] inventorySlots;         // 아이템 슬롯 위치
    private Image[] slotsImage;                 // 슬롯 이미지
    private PlayerWeaponChanger playerAttack;   // 플레이어 공격 스크립트
    private Animator animator;                  // 플레이어 애니메이터
    private int currentSelect = -1;             // 현재 선택한 이미지

    private void Start()
    {
        Debug.Assert(inventory != null, "Error (Null Reference) : 인벤토리가 존재하지 않습니다.");

        playerAttack = gameObject.GetComponent<PlayerWeaponChanger>();
        Debug.Assert(playerAttack != null, "Error (Null Reference) : 플레이어 공격 스크립트가 존재하지 않습니다.");

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 플레이어 애니메이터가 존재하지 않습니다.");

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

        // 이번 프레임에 누른 슬롯 번호
        int select = -1;

        // 플레이어가 메뉴를 열 경우
        if (PlayerMenu.isMenu)
        {
            if (currentSelect < 0)
                return;

            // 플레이어가 아이템 위치를 바꿔서 아이템이 없을 경우
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

        // 같은 슬롯을 선택한 경우
        if (select >= 0 && select == currentSelect)
        {
            currentSelect = -1;
            playerAttack.weaponType = WeaponType.NULL;

            // 들고 있는 아이템 모두 비활성화
            for (int i = 0; i < items.Length; i++)
                items[i].gameObject.SetActive(false);

            // 선택한 슬롯 선택 해제
            for (int i = 0; i < slotsImage.Length; i++)
                slotsImage[i].color = Color.white;

            animator.SetTrigger("EquidItem");
        }

        // 다른 슬롯을 선택한 경우
        else if (select >= 0 && select != currentSelect)
        {
            currentSelect = select;
            playerAttack.weaponType = WeaponType.ITEM1 + select;

            Select();
        }
    }

    private void Select()
    {
        // 해당 위치에 아이템이 있을 경우
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

        // 스프라이트를 아이템으로 변환함
        GameObject showObject = null;
        for (int i = 0; i < imageToItem.Count; i++)
        {
            if (imageToItem[i].sprite == itemSprite)
            {
                showObject = imageToItem[i].item;
                break;
            }
        }

        Debug.Assert(showObject != null, "Error (Null Reference) : 해당 이미지의 아이템이 존재하지 않습니다.");

        // 들고 있는 아이템 모두 비활성화
        for (int i = 0; i < items.Length; i++)
            items[i].gameObject.SetActive(false);

        // 변환한 아이템이 들고 있는 아이템 중 하나를 활성화 함
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

            // 들고 있는 아이템 모두 비활성화
            for (int i = 0; i < items.Length; i++)
                items[i].gameObject.SetActive(false);

            // 선택한 슬롯 선택 해제
            for (int i = 0; i < slotsImage.Length; i++)
                slotsImage[i].color = Color.white;
        }

        else
            remainText.text = count.ToString();
    }
}