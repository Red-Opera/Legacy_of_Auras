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
    [SerializeField] private List<ItemToStat> itemToStat;   // �������� �������� ��ȯ�ϴ� ����Ʈ
    [SerializeField] private GameObject itemContainer;      // ���� ������ �������� �ִ� �����̳�
    [SerializeField] private PlayerState playerState;       // �÷��̾� ����

    private Transform[] items;      // ���� ������ �����۵�


    private Animator animator;      // �÷��̾� �ִϸ�����
    private ItemSelect itemSelect;  // ������ ���� ������Ʈ

    private void Start()
    {
        Debug.Assert(itemContainer != null, "Error (Null Reference) : �÷��̾ ������ ������ �����̳ʰ� �������� �ʽ��ϴ�.");

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        itemSelect = GetComponent<ItemSelect>();
        Debug.Assert(itemSelect != null, "Error (Null Reference) : ������ ���� ������Ʈ�� �������� �ʽ��ϴ�.");

        items = new Transform[itemContainer.transform.childCount];

        for (int i = 0; i < items.Length; i++)
            items[i] = itemContainer.transform.GetChild(i);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            StartCoroutine(ApplyStat());
    }

    // ��� �ִ� �������� �����ϴ� �޼ҵ�
    private IEnumerator ApplyStat()
    {
        // �������� ��� �ִ��� Ȯ����
        GameObject selectedItem = null;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].gameObject.activeSelf)
            {
                selectedItem = items[i].gameObject;
                break;
            }
        }

        // ��� �ִ� �������� ���� ���
        if (selectedItem == null)
            yield break;

        PlayerState selectStat = null;

        // �������� �ɷ�ġ�� ��ȯ��
        for (int i = 0; i < itemToStat.Count; i++)
        {
            if (itemToStat[i].item == selectedItem)
            {
                selectStat = itemToStat[i].statUp;
                break;
            }
        }

        // �ش� �������� �ɷ�ġ�� ���� �� ���� ���
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

        // ������ ��� �ݿ� �� �÷��̾� �ɷ�ġ �ݿ�
        itemSelect.ItemUseReflection();
        playerState.Add(selectStat);
    }
}