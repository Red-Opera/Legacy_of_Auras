using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestSelect : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject detail; // 세부 정보 UI를 담는 오브젝트

    private static List<Transform> detailList;  // 세부 정보 UI를 담는 배열
    private int index = -1;                     // 해당 간략 정보 UI가 몇번째에 해당하는지 알려주는 인덱스

    private void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        if (detailList == null)
        {
            Debug.Assert(detail != null, "오류 (Null Reference): 세부 정보를 담는 UI가 존재하지 않습니다.");

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

    // 모든 세부정보를 닫는 메소드
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
        // 모든 세부정보를 닫음
        CloseAddDetail();

        // 선택한 세부정보를 열음
        detailList[index].gameObject.SetActive(true);
        RenewalDetail();
    }
}
