using UnityEngine;

public class ItemShopOpenClose : MonoBehaviour
{
    public GameObject shopUI;                   // ���� UI
    public static bool isShopOpen = false;      // ���� ������ ���� �ִ��� ����

    private NPCLookAtPlayer lookAtPlayer;       // NPC�� �÷��̾ �ٶ󺸴� ��ũ��Ʈ

    public void Start()
    {
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

            isShopOpen = true;
        }
    }

    // ������ �ݴ� �޼ҵ�
    private void CloseShopUI()
    {
        if (isShopOpen)
        {
            shopUI.SetActive(false);

            isShopOpen = false;
        }
    }
}
