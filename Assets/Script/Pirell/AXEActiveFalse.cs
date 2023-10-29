using UnityEngine;

public class AXEActiveFalse : MonoBehaviour
{
    public GameObject leftAXE;
    public GameObject rightAXE;
    private MonsterHPBar hpBar;

    public void Start()
    {
        Debug.Assert(leftAXE != null, "Error (Null Reference) : �޼� ������ �������� ����");
        Debug.Assert(rightAXE != null, "Error (Null Reference) : ������ ������ �������� ����");

        hpBar = GetComponent<MonsterHPBar>();
        Debug.Assert(hpBar != null, "Error (Null Reference) : ü�� �� ��ũ��Ʈ�� �������� ����");
    }

    public void Update()
    {
        // ���� ��� ĳ���Ͱ� ���� �ʰ� �����ϱ� ���� ��ġ
        if (hpBar.currentHP <= 0)
        {
            leftAXE.GetComponent<MeshCollider>().enabled = false;
            rightAXE.GetComponent<MeshCollider>().enabled = false;
        }
    }
}
