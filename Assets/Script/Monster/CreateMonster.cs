using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateMonster : MonoBehaviour
{
    public static CreateMonster instance;
    public float createSize;                // ���� ����
     
    [SerializeField] private GameObject createMonster;  // ������ ����
    [SerializeField] private int maxCreateMonster; // �ִ�� ���������� ���� ��

    public static int nowCount = 0;         // ���� ���� ��

    private GameObject player;
    private Terrain terrain;

    public void Start()
    {
        player = GameObject.Find("Model");

        // Terrain ������Ʈ ��������
        terrain = GetComponent<Terrain>();
        Debug.Assert(terrain != null, "���� (Null Reference): Terrain ������Ʈ�� �������� �ʽ��ϴ�.");

        SceneManager.sceneLoaded += Setting;
        Setting(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        // ���� ���� �޼��� ȣ��
        StartCoroutine(AutoSpawnMonsters());
    }

    public bool MoveMonster(bool isCreate, Transform target = null)
    {
        // Terrain�� ���ο� ���� ũ�� ��������
        float terrainWidth = terrain.terrainData.size.x;
        float terrainHeight = terrain.terrainData.size.z;

        for (int failStack = 0; failStack < maxCreateMonster * 2; failStack++)
        {
            // Terrain ���ο� ������ 80% ���� ������ ������ ��ġ ���
            float randomX = Random.Range(player.transform.position.x - createSize, player.transform.position.x + createSize);
            float randomZ = Random.Range(player.transform.position.z - createSize, player.transform.position.z + createSize);

            randomX = Mathf.Clamp(randomX, terrainWidth / 2 - 0.45f * terrainWidth, terrainWidth / 2 + 0.45f * terrainWidth);
            randomZ = Mathf.Clamp(randomZ, terrainWidth / 2 - 0.45f * terrainWidth, terrainWidth / 2 + 0.45f * terrainWidth);

            // Terrain�� ��ġ�� ������ �������� ���� ���� ��ġ ���
            Vector3 spawnPosition = new Vector3(randomX, 50f, randomZ);

            // ���� �Ʒ� �������� ����ĳ��Ʈ�Ͽ� ���� �Ǵ� �ٸ� ������Ʈ ã��
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit))
            {
                // ���̰� �浹�� ������Ʈ�� Terrian�� ��� �浹 �������� ��ġ��
                if (hit.collider.gameObject == gameObject)
                    spawnPosition = new Vector3(spawnPosition.x, hit.point.y, spawnPosition.z);
            }

            // ���������� ��ġ�� �� �ִ� ��ġ�� ���
            if (hit.collider != null && hit.collider.gameObject.tag == "Terrain")
            {
                // ���� ��ġ�� ���� ������Ʈ ����
                if (isCreate)
                    Instantiate(createMonster, spawnPosition, Quaternion.identity);

                else
                    target.position = spawnPosition;

                return true;
            }

            // �̵� ������ ���
            else if (!isCreate)
            {
                Destroy(target.gameObject);
                nowCount--;
            }
        }

        return false;
    }

    private IEnumerator AutoSpawnMonsters()
    {
        while (true)
        {
            for (; nowCount <= maxCreateMonster; nowCount++)
            {
                if (!MoveMonster(true))
                    break;
            }

            yield return new WaitForSeconds(10.0f);
        }
    }

    private void Setting(Scene scene, LoadSceneMode mode)
    {
        instance = this;
        nowCount = 0;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= Setting;
    }
}