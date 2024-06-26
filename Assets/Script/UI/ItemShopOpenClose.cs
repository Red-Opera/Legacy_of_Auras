using UnityEngine;

public class ItemShopOpenClose : MonoBehaviour
{
    public GameObject shopUI;                   // 상점 UI
    public static bool isShopOpen = false;      // 현재 상점이 열려 있는지 여부

    private NPCLookAtPlayer lookAtPlayer;       // NPC가 플레이어를 바라보는 스크립트
    private CharacterController playerControl;  // 플레이어 오브젝트
    private GameObject levelUI;                 // level UI 오브젝트

    public void Start()
    {
        playerControl = GameObject.Find("Model").GetComponent<CharacterController>();
        Debug.Assert(playerControl != null, "Error (Null Reference) : 플레이어 콘트롤이 존재하지 않습니다.");

        levelUI = GameObject.Find("LevelUI").gameObject;
        Debug.Assert(levelUI != null, "Error (Null Reference) : 생성시 레벨 UI가 존재하지 않습니다.");

        lookAtPlayer = GetComponent<NPCLookAtPlayer>();

        shopUI.SetActive(false);
    }

    public void Update()
    {
        // 플레이어가 E키를 누르거나 상점을 열수 있는 거리이내로 들어온 경우, 상점을 열음
        if (Input.GetKeyDown(KeyCode.E) && (lookAtPlayer.distanceToPlayer < lookAtPlayer.interactionDistance))
            OpenShopUI();

        // 상점이 열려 있거나 범위를 벗어난 경우, 상점을 닫음
        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape) || (lookAtPlayer.distanceToPlayer > lookAtPlayer.interactionDistance))
            CloseShopUI();
    }

    // 상점을 여는 메소드
    private void OpenShopUI()
    {
        if (!isShopOpen)
        {
            shopUI.SetActive(true);
            levelUI.SetActive(false);

            isShopOpen = true;
        }
    }

    // 상점을 닫는 메소드
    private void CloseShopUI()
    {
        if (isShopOpen)
        {
            shopUI.SetActive(false);
            levelUI.SetActive(true);

            isShopOpen = false;
        }
    }
}
