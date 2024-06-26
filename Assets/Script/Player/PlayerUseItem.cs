using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemToStat
{
    public GameObject item;
    public PlayerState statUp;
}

public class PlayerUseItem : MonoBehaviour
{
    [SerializeField] private List<ItemToStat> itemToStat;   // 아이템을 스탯으로 변환하는 리스트
    [SerializeField] private GameObject itemContainer;      // 선택 가능한 아이템이 있는 컨테이너
    [SerializeField] private PlayerState playerState;       // 플레이어 상태

    private Transform[] items;      // 선택 가능한 아이템들


    private Animator animator;      // 플레이어 애니메이터
    private ItemSelect itemSelect;  // 아이템 선택 컴포넌트

    private void Start()
    {
        Debug.Assert(itemContainer != null, "Error (Null Reference) : 플레이어가 가지는 아이템 컨테이너가 존재하지 않습니다.");

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이터가 존재하지 않습니다.");

        itemSelect = GetComponent<ItemSelect>();
        Debug.Assert(itemSelect != null, "Error (Null Reference) : 아이템 선택 컴포넌트가 존재하지 않습니다.");

        items = new Transform[itemContainer.transform.childCount];

        for (int i = 0; i < items.Length; i++)
            items[i] = itemContainer.transform.GetChild(i);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            StartCoroutine(ApplyStat());
    }

    // 들고 있는 아이템을 적용하는 메소드
    private IEnumerator ApplyStat()
    {
        // 아이템을 들고 있는지 확인함
        GameObject selectedItem = null;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].gameObject.activeSelf)
            {
                selectedItem = items[i].gameObject;
                break;
            }
        }

        // 들고 있는 아이템이 없는 경우
        if (selectedItem == null)
            yield break;

        PlayerState selectStat = null;

        // 아이템을 능력치로 변환함
        for (int i = 0; i < itemToStat.Count; i++)
        {
            if (itemToStat[i].item == selectedItem)
            {
                selectStat = itemToStat[i].statUp;
                break;
            }
        }

        // 해당 아이템이 능력치를 높일 수 없을 경우
        if (selectStat == null) 
            yield break;

        animator.SetTrigger("Drinking");

        while (true)
        {
            float currentPersent = animator.GetCurrentAnimatorStateInfo(1).normalizedTime;
            bool isDrinking = animator.GetCurrentAnimatorStateInfo(1).IsName("Drinking");

            if (!animator.IsInTransition(1) && isDrinking &&  currentPersent >= 0.5f)
                break;

            yield return null;
        }

        // 아이템 사용 반영 후 플레이어 능력치 반영
        itemSelect.ItemUseReflection();
        playerState.Add(selectStat);
    }
}