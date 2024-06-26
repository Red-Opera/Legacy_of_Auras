using UnityEngine;
using UnityEngine.UI;

public class GoalSpot : MonoBehaviour
{
    [SerializeField] private float amplitude = 1.0f;      // 곡선의 진폭 (높이)
    [SerializeField] private float frequency = 1.0f;      // 곡선의 주파수 (반복 횟수)
    [SerializeField] private float speed = 1.0f;          // 이동 속도

    [SerializeField] private string whatQuest = "";       // 화살표를 실행하는데 어떠한 조건이 필요한지 알려주는 변수

    private Vector3 initialPosition;    // 처음 위치
    private Image image;                // 투명하게 바꾸기 위한 변수

    void Start()
    {
        image = GetComponent<Image>();
        Debug.Assert(image != null, "Error (Null Reference) : 애니메이션 컴포넌트가 존재하지 않습니다.");

        // 현재 위치를 저장함
        initialPosition = transform.position;

        // 해당 퀘스트를 통과하거나 이번 퀘스트가 아닌 경우 유도 화살표를 숨김
        if ((bool)PlayerQuest.quest.questList[whatQuest] || !PlayerQuest.quest.nowQuest.Equals(whatQuest))
        {
            Color toColor = image.color;
            toColor.a = 0.0f;

            image.color = toColor;
        }
    }

    void Update()
    {
        bool isClear = (bool)PlayerQuest.quest.questList[whatQuest];        // 해당 퀘스트를 클리어했는지 여부
        bool isThisQuest = PlayerQuest.quest.nowQuest.Equals(whatQuest);    // 이번 퀘스트가 이 퀘스트인지 여부

        // 퀘스트를 클리어하거나 이번 퀘스트가 아니고 퀘스트가 켜져 있는 경우
        if ((isClear || !isThisQuest) && image.color.a > 0.9f)
        {
            Color toColor = image.color;
            toColor.a = 0.0f;

            image.color = toColor;
        }

        // 이번 퀘스트가 아닌 경우
        else if (isClear || !isThisQuest)
            return;

        // 퀘스트를 클리어 하지 않았고 이번 퀘스트이며 아직 퀘스트를 키지 않은 경우
        if (!isClear && isThisQuest && image.color.a < 0.9f)
        {
            Color toColor = image.color;
            toColor.a = 1.0f;

            image.color = toColor;
        }

        // 현재 시간에 따라 y축 위치를 조절
        float newY = initialPosition.y + Mathf.Sin(Time.time * speed * frequency) * amplitude;

        // 새로운 위치를 설정
        Vector3 newPosition = transform.position;
        newPosition.y = newY;

        transform.position = newPosition;
    }
}
