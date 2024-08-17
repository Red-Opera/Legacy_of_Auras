using UnityEngine;

public class InfoLookAtPlayer : MonoBehaviour
{
    public GameObject infoUI;       // 플레이어에게 정보를 알려주는 변수

    private GameObject player;      // 플레이어 오브젝트

    void Start()
    {
        player = GameObject.Find("MainCamera").transform.GetChild(0).GetChild(0).gameObject;
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");
    }

    private void FixedUpdate()
    {
        LookAtPlayer();
    }

    // 체력 바가 플레이어를 바라보도록 해주는 메소드
    private void LookAtPlayer()
    {
        Vector3 rotation = infoUI.transform.rotation.eulerAngles;

        infoUI.transform.LookAt(player.transform.position);

        if (transform.childCount >= 1 && transform.GetChild(0).name == "MaxSize")
            infoUI.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
    }
}