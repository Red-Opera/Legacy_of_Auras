using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestSelect : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject detail; // ���� ���� UI�� ��� ������Ʈ

    private static List<Transform> detailList;  // ���� ���� UI�� ��� �迭
    private int index = -1;                     // �ش� ���� ���� UI�� ���°�� �ش��ϴ��� �˷��ִ� �ε���

    private void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        if (detailList == null)
        {
            Debug.Assert(detail != null, "���� (Null Reference): ���� ������ ��� UI�� �������� �ʽ��ϴ�.");

            detailList = new List<Transform>();

            for (int i = 0; i < detail.transform.childCount; i++)
                detailList.Add(detail.transform.GetChild(i));
        }    

        if (index == -1)
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i).gameObject == gameObject)
                {
                    index = i;
                    break;
                }
            }
        }
    }

    // ��� ���������� �ݴ� �޼ҵ�
    private void CloseAddDetail()
    {
        for (int i = 0; i < detailList.Count; i++)
            detailList[i].gameObject.SetActive(false);
    }

    private void RenewalDetail()
    {
        GameObject detail = detailList[index].gameObject;

        QuestDetail questDetail = detail.GetComponent<QuestDetail>();

        if (questDetail == null)
            return;

        detail.GetComponent<QuestDetail>().RenewalDetail();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ��� ���������� ����
        CloseAddDetail();

        // ������ ���������� ����
        detailList[index].gameObject.SetActive(true);
        RenewalDetail();
    }
}
