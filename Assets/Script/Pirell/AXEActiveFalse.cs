using UnityEngine;

public class AXEActiveFalse : MonoBehaviour
{
    public GameObject leftAXE;
    public GameObject rightAXE;
    private MonsterHPBar hpBar;

    public void Start()
    {
        Debug.Assert(leftAXE != null, "Error (Null Reference) : 왼손 도끼가 존재하지 않음");
        Debug.Assert(rightAXE != null, "Error (Null Reference) : 오른손 도끼가 존재하지 않음");

        hpBar = GetComponent<MonsterHPBar>();
        Debug.Assert(hpBar != null, "Error (Null Reference) : 체력 바 스크립트가 존재하지 않음");
    }

    public void Update()
    {
        // 죽은 경우 캐릭터가 뜨지 않게 방지하기 위해 설치
        if (hpBar.currentHP <= 0)
        {
            leftAXE.GetComponent<MeshCollider>().enabled = false;
            rightAXE.GetComponent<MeshCollider>().enabled = false;
        }
    }
}
