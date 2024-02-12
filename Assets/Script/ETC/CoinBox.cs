using UnityEngine;

public class CoinBox : MonoBehaviour
{
    public float upPower = 5000.0f;     // 생성될 때 위로 올라가는 힘
    public float rotationSpeed = 25.0f; // 회전 속도
    public PlayerState playerState;     // 플레이어 상태

    private Rigidbody rigidbody;
    private int dropMoney;              // 이 골드 박스의 돈

    private Alert alert;                // 플레이어 알림 UI

    public void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null, "Error (Null Reference) : Rigidbody가 존재하지 않습니다.");

        rigidbody.AddForce(new Vector3(0, upPower, 0));
    }

    public void Update()
    {
        // 오브젝트를 천천히 회전시킴
        float rotation = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation);
    }

    public void SetDropGold(int gold) { dropMoney = gold; }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (alert == null)
                alert = GameObject.Find("UI").GetComponent<Alert>();

            playerState.money += dropMoney;

            alert.PushAlert(dropMoney.ToString() + "G를 획득 했습니다. (현재 골드 : " + playerState.money.ToString() + ")");
            Destroy(transform.parent.gameObject);
        }
    }
}
