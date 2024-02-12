using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterDropItem : MonoBehaviour
{
    public GameObject[] goldBoxs;           // ��� �ڽ� ������Ʈ
    public GameObject[] dropItems;          // ������ ������Ʈ

    public float dropItemRange = 8.0f;      // ����Ǵ� ���� ������
    public float yUpDrop = 1.0f;            // ó���� ���������ּ� �󸶳� ������ �����Ǵ��� ����
    public int dropMoneyRange = 20;         // ���� ����Ǵ� ���� ��ġ ����
    public MonsterState monsterState;       // �������� ����ϴ� ������ ����

    public void DropItem()
    {
        // y ������ yUpDrop��ŭ ���� y �� ��ǥ ���
        float dropHeight = transform.position.y + yUpDrop;

        // �������� dropItemRange�� ���� ������ ������ ����
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

        // �������� �����ϰ� ��ġ�� �������� ����
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
