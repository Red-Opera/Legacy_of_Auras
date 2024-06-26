using TMPro;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinRenderer;  // 캐릭터 Rederer
    [SerializeField] private Transform minimapCamerapos;        // 미니맵 카메라 위치
    [SerializeField] private TextMeshProUGUI multiple;          // 배율을 표시할 텍스트
    [SerializeField] private TextMeshProUGUI mapName;           // 맵 이름 텍스트
    [SerializeField] private TextMeshProUGUI mapEngName;        // 맵 이름 영어 텍스트

    [SerializeField] private float scaleFactor;                 // 조절할 스케일 팩터
    [SerializeField] private float maxDistance;                 // 최소 미니맵 배율
    [SerializeField] private float minDistance;                 // 최대 미니맵 배율

    private Transform playerTransform;
    private string currentMapName;
    private float upYPos = 100.0f;
    private float currentYPos;

    private void Start()
    {
        Debug.Assert(skinRenderer != null, "Error (Null Reference) : 플레이어 skinRenderer가 존재하지 않습니다.");

        playerTransform = GameObject.Find("Model").transform;

        currentYPos = skinRenderer.bounds.center.y + upYPos;
        multiple.text = (100 / upYPos).ToString("X#0.#");
    }

    private void Update()
    {
        if (QuestManager.isQuestUIOn)
            return;

        // 마우스 휠 입력 받기
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // 휠을 아래로 스크롤할 때
        if (scroll < 0 && upYPos <= maxDistance)
        {
            upYPos += scaleFactor;   // 스케일 팩터만큼 카메라를 위로 이동

            multiple.text = (100 / upYPos).ToString("X#0.#");
        }

        else if (scroll > 0 && upYPos >= minDistance)
        {
            upYPos -= scaleFactor;

            multiple.text = (100 / upYPos).ToString("X#0.#");
        }

        currentYPos = skinRenderer.bounds.center.y + upYPos;

        if (mapName.text != currentMapName)
            ChangeMapName();
    }

    private void ChangeMapName()
    {
        currentMapName = mapEngName.text;

        if (currentMapName == "Village")
            mapName.text = "황폐화된 마을";

        else if (currentMapName == "Desolate Emporium")
            mapName.text = "황폐화된 상점";

        else if (currentMapName == "Library")
            mapName.text = "도서관";

        else if (currentMapName == "Endless Barrens")
            mapName.text = "끝없는 불모지";

        else if (currentMapName == "Verdant Vale")
            mapName.text = "푸른 골짜기";

        else if (currentMapName == "Infernal Abyss")
            gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        minimapCamerapos.position = new Vector3(playerTransform.position.x, currentYPos, playerTransform.position.z);

        minimapCamerapos.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
    }
}
