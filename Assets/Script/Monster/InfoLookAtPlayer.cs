using UnityEngine;

public class InfoLookAtPlayer : MonoBehaviour
{
    public GameObject infoUI;       // �÷��̾�� ������ �˷��ִ� ����

    private GameObject player;      // �÷��̾� ������Ʈ

    void Start()
    {
        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");
    }

    void Update()
    {
        LookAtPlayer();
    }

    // ü�� �ٰ� �÷��̾ �ٶ󺸵��� ���ִ� �޼ҵ�
    private void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - infoUI.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        infoUI.transform.rotation = rotation;
    }
}
