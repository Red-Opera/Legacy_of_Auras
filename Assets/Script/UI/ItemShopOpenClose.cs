using UnityEngine;

public class ItemShopOpenClose : MonoBehaviour
{
    public GameObject shopUI;                   // ���� UI
    public static bool isShopOpen = false;      // ���� ������ ���� �ִ��� ����

    private NPCLookAtPlayer lookAtPlayer;       // NPC�� �÷��̾ �ٶ󺸴� ��ũ��Ʈ
    private CharacterController playerControl;  // �÷��̾� ������Ʈ
    private GameObject levelUI;                 // level UI ������Ʈ

    public void Start()
    {
        playerControl = GameObject.Find("Model").GetComponent<CharacterController>();
        Debug.Assert(playerControl != null, "Error (Null Reference) : �÷��̾� ��Ʈ���� �������� �ʽ��ϴ�.");

        levelUI = GameObject.Find("LevelUI").gameObject;
        Debug.Assert(levelUI != null, "Error (Null Reference) : ������ ���� UI�� �������� �ʽ��ϴ�.");

        lookAtPlayer = GetComponent<NPCLookAtPlayer>();

        shopUI.SetActive(false);
    }

    public void Update()
    {
        // �÷��̾ EŰ�� �����ų� ������ ���� �ִ� �Ÿ��̳��� ���� ���, ������ ����
        if (Input.GetKeyDown(KeyCode.E) && (lookAtPlayer.distanceToPlayer < lookAtPlayer.interactionDistance))
            OpenShopUI();

        // ������ ���� �ְų� ������ ��� ���, ������ ����
        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape) || (lookAtPlayer.distanceToPlayer > lookAtPlayer.interactionDistance))
            CloseShopUI();
    }

    // ������ ���� �޼ҵ�
    private void OpenShopUI()
    {
        if (!isShopOpen)
        {
            shopUI.SetActive(true);
            levelUI.SetActive(false);

            isShopOpen = true;
        }
    }

    // ������ �ݴ� �޼ҵ�
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
