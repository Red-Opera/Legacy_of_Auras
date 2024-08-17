using TMPro;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public static string currentMapName;

    [SerializeField] private SkinnedMeshRenderer skinRenderer;  // ĳ���� Rederer
    [SerializeField] private Transform minimapCamerapos;        // �̴ϸ� ī�޶� ��ġ
    [SerializeField] private TextMeshProUGUI multiple;          // ������ ǥ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI mapName;           // �� �̸� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI mapEngName;        // �� �̸� ���� �ؽ�Ʈ

    [SerializeField] private float scaleFactor;                 // ������ ������ ����
    [SerializeField] private float maxDistance;                 // �ּ� �̴ϸ� ����
    [SerializeField] private float minDistance;                 // �ִ� �̴ϸ� ����

    private Transform playerTransform;
    private float upYPos = 100.0f;
    private float currentYPos;

    private void Awake()
    {
        Debug.Assert(skinRenderer != null, "Error (Null Reference) : �÷��̾� skinRenderer�� �������� �ʽ��ϴ�.");

        playerTransform = GameObject.Find("Model").transform;

        currentYPos = skinRenderer.bounds.center.y + upYPos;
        multiple.text = (100 / upYPos).ToString("X#0.#");

        ChangeMapName();
    }

    private void Update()
    {
        if (QuestManager.isQuestUIOn)
            return;

        // ���콺 �� �Է� �ޱ�
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // ���� �Ʒ��� ��ũ���� ��
        if (scroll < 0 && upYPos <= maxDistance)
        {
            upYPos += scaleFactor;   // ������ ���͸�ŭ ī�޶� ���� �̵�

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

        for (int i = 0; i < Loading.engToKoreaStatic.Count; i++)
        {
            if (currentMapName == Loading.engToKoreaStatic[i].eng)
                mapName.text = Loading.engToKoreaStatic[i].korea;
        }

        if (currentMapName == "Infernal Abyss")
            gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        minimapCamerapos.position = new Vector3(playerTransform.position.x, currentYPos, playerTransform.position.z);

        minimapCamerapos.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
    }
}
