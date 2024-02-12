using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterDropItem : MonoBehaviour
{
    public GameObject[] goldBoxs;           // 골드 박스 오브젝트
    public GameObject[] dropItems;          // 아이템 오브젝트

    public float dropItemRange = 8.0f;      // 드랍되는 영역 반지름
    public float yUpDrop = 1.0f;            // 처음에 오부잭투애소 얼마나 위에서 생성되는지 여부
    public int dropMoneyRange = 20;         // 돈이 드랍되는 돈의 가치 범위
    public MonsterState monsterState;       // 아이템을 드랍하는 몬스터의 상태

    public void DropItem()
    {
        // y 축으로 yUpDrop만큼 위의 y 축 좌표 계산
        float dropHeight = transform.position.y + yUpDrop;

        // 반지름이 dropItemRange인 영역 내에서 아이템 생성
        float randomRadius = Random.Range(0f, dropItemRange);
        float randomAngle = Random.Range(0f, 360f);

        Vector3 randomPosition = new Vector3(
            transform.position.x + randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            dropHeight,
            transform.position.z + randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        );

        int monsterMoney = monsterState.dropMoney;
        int randomGold = (int)Random.Range(monsterMoney * (1 - (dropMoneyRange * 0.5f) / 100), monsterMoney * (1 + dropMoneyRange * 0.5f / 100));

        GameObject goldBox;

        if (randomGold < monsterMoney * (1 + (dropMoneyRange * 0.25) / 100))
            goldBox = Instantiate(goldBoxs[0], randomPosition, Quaternion.identity);

        else
            goldBox = Instantiate(goldBoxs[1], randomPosition, Quaternion.identity);

        goldBox.transform.GetChild(0).GetComponent<CoinBox>().SetDropGold(randomGold);

        if (SceneManager.GetActiveScene().name == "Forest")
            goldBox.transform.localScale *= 0.3f;

        // 아이템을 생성하고 위치를 랜덤으로 조정
        foreach (GameObject dropItemPrefab in dropItems)
        {
            dropHeight = transform.position.y + yUpDrop;

            randomRadius = Random.Range(0f, dropItemRange);
            randomAngle = Random.Range(0f, 360f);

            randomPosition = new Vector3(
                transform.position.x + randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                dropHeight,
                transform.position.z + randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad)
            );

            Instantiate(dropItemPrefab, randomPosition, Quaternion.identity);
        }
    }
}
