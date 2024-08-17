using UnityEngine;

public class InfoLookAtPlayer : MonoBehaviour
{
    public GameObject infoUI;       // �÷��̾�� ������ �˷��ִ� ����

    private GameObject player;      // �÷��̾� ������Ʈ

    void Start()
    {
        player = GameObject.Find("MainCamera").transform.GetChild(0).GetChild(0).gameObject;
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");
    }

    private void FixedUpdate()
    {
        LookAtPlayer();
    }

    // ü�� �ٰ� �÷��̾ �ٶ󺸵��� ���ִ� �޼ҵ�
    private void LookAtPlayer()
    {
        Vector3 rotation = infoUI.transform.rotation.eulerAngles;

        infoUI.transform.LookAt(player.transform.position);

        if (transform.childCount >= 1 && transform.GetChild(0).name == "MaxSize")
            infoUI.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
    }
}