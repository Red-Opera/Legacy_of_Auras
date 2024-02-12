using UnityEngine;

public class InfoLookAtPlayer : MonoBehaviour
{
    public GameObject infoUI;       // 플레이어에게 정보를 알려주는 변수

    private GameObject player;      // 플레이어 오브젝트

    void Start()
    {
        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");
    }

    void Update()
    {
        LookAtPlayer();
    }

    // 체력 바가 플레이어를 바라보도록 해주는 메소드
    private void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - infoUI.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        infoUI.transform.rotation = rotation;
    }
}
