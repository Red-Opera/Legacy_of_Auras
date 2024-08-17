using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateMonster : MonoBehaviour
{
    public static CreateMonster instance;
    public float createSize;                // 생성 범위
     
    [SerializeField] private GameObject createMonster;  // 생성할 몬스터
    [SerializeField] private int maxCreateMonster; // 최대로 생성가능한 몬스터 수

    public static int nowCount = 0;         // 현재 몬스터 수

    private GameObject player;
    private Terrain terrain;

    public void Start()
    {
        player = GameObject.Find("Model");

        // Terrain 컴포넌트 가져오기
        terrain = GetComponent<Terrain>();
        Debug.Assert(terrain != null, "오류 (Null Reference): Terrain 컴포넌트가 존재하지 않습니다.");

        SceneManager.sceneLoaded += Setting;
        Setting(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        // 몬스터 생성 메서드 호출
        StartCoroutine(AutoSpawnMonsters());
    }

    public bool MoveMonster(bool isCreate, Transform target = null)
    {
        // Terrain의 가로와 세로 크기 가져오기
        float terrainWidth = terrain.terrainData.size.x;
        float terrainHeight = terrain.terrainData.size.z;

        for (int failStack = 0; failStack < maxCreateMonster * 2; failStack++)
        {
            // Terrain 가로와 세로의 80% 범위 내에서 랜덤한 위치 계산
            float randomX = Random.Range(player.transform.position.x - createSize, player.transform.position.x + createSize);
            float randomZ = Random.Range(player.transform.position.z - createSize, player.transform.position.z + createSize);

            randomX = Mathf.Clamp(randomX, terrainWidth / 2 - 0.45f * terrainWidth, terrainWidth / 2 + 0.45f * terrainWidth);
            randomZ = Mathf.Clamp(randomZ, terrainWidth / 2 - 0.45f * terrainWidth, terrainWidth / 2 + 0.45f * terrainWidth);

            // Terrain의 위치에 랜덤한 오프셋을 더한 스폰 위치 계산
            Vector3 spawnPosition = new Vector3(randomX, 50f, randomZ);

            // 몬스터 아래 방향으로 레이캐스트하여 지면 또는 다른 오브젝트 찾기
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit))
            {
                // 레이가 충돌한 오브젝트가 Terrian인 경우 충돌 지정으로 배치함
                if (hit.collider.gameObject == gameObject)
                    spawnPosition = new Vector3(spawnPosition.x, hit.point.y, spawnPosition.z);
            }

            // 성공적으로 배치할 수 있는 위치인 경우
            if (hit.collider != null && hit.collider.gameObject.tag == "Terrain")
            {
                // 스폰 위치에 몬스터 오브젝트 생성
                if (isCreate)
                    Instantiate(createMonster, spawnPosition, Quaternion.identity);

                else
                    target.position = spawnPosition;

                return true;
            }

            // 이동 실패한 경우
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