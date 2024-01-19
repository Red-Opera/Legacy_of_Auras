using UnityEngine;

public class CreateMonster : MonoBehaviour
{
    public GameObject createMonster;    // ������ ����
    public int maxCreateMonster = 200;  // �ִ�� ���������� ���� ��

    public static int nowCount = 0;     // ���� ���� ��

    public void Start()
    {
        // ���� ���� �޼��� ȣ��
        SpawnMonsters();
    }

    public void Update()
    {
        // �ִ�� ������� �� �ִ� ������ ������ ���� ��� ����
        if (nowCount < maxCreateMonster)
            SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        // Terrain ������Ʈ ��������
        Terrain terrain = GetComponent<Terrain>();
        Debug.Assert(terrain != null, "���� (Null Reference): Terrain ������Ʈ�� �������� �ʽ��ϴ�.");

        // Terrain�� ���ο� ���� ũ�� ��������
        float terrainWidth = terrain.terrainData.size.x;
        float terrainHeight = terrain.terrainData.size.z;

        for (int i = nowCount; i < maxCreateMonster; i++)
        {
            // Terrain ���ο� ������ 80% ���� ������ ������ ��ġ ���
            float randomX = Random.Range(0.1f * terrainWidth, 0.9f * terrainWidth);
            float randomZ = Random.Range(0.1f * terrainHeight, 0.9f * terrainHeight);

            // Terrain�� ��ġ�� ������ �������� ���� ���� ��ġ ���
            Vector3 spawnPosition = terrain.transform.position + new Vector3(randomX, 50f, randomZ);

            // ���� ��ġ�� ���� ������Ʈ ����
            GameObject monsterObject = Instantiate(createMonster, spawnPosition, Quaternion.identity);

            // ���� �Ʒ� �������� ����ĳ��Ʈ�Ͽ� ���� �Ǵ� �ٸ� ������Ʈ ã��
            RaycastHit hit;
            if (Physics.Raycast(monsterObject.transform.position, Vector3.down, out hit))
            {
                // ���̰� �浹�� ������Ʈ�� ���� ��ü�� ���
                if (hit.collider.gameObject == gameObject)
                {
                    // ������ y�� ���� ���� �浹 ������ y�� ������ ����
                    monsterObject.transform.position = new Vector3(monsterObject.transform.position.x, hit.point.y, monsterObject.transform.position.z);
                }
            }
        }

        nowCount = maxCreateMonster;
    }
}