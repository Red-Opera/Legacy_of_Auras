using UnityEngine;

public class CreateMonster : MonoBehaviour
{
    public GameObject createMonster;    // 생성할 몬스터
    public int maxCreateMonster = 200;  // 최대로 생성가능한 몬스터 수

    public static int nowCount = 0;     // 현재 몬스터 수

    public void Start()
    {
        // 몬스터 생성 메서드 호출
        SpawnMonsters();
    }

    public void Update()
    {
        // 최대로 만들어질 수 있는 몬스터의 수보다 적을 경우 생성
        if (nowCount < maxCreateMonster)
            SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        // Terrain 컴포넌트 가져오기
        Terrain terrain = GetComponent<Terrain>();
        Debug.Assert(terrain != null, "오류 (Null Reference): Terrain 컴포넌트가 존재하지 않습니다.");

        // Terrain의 가로와 세로 크기 가져오기
        float terrainWidth = terrain.terrainData.size.x;
        float terrainHeight = terrain.terrainData.size.z;

        for (int i = nowCount; i < maxCreateMonster; i++)
        {
            // Terrain 가로와 세로의 80% 범위 내에서 랜덤한 위치 계산
            float randomX = Random.Range(0.1f * terrainWidth, 0.9f * terrainWidth);
            float randomZ = Random.Range(0.1f * terrainHeight, 0.9f * terrainHeight);

            // Terrain의 위치에 랜덤한 오프셋을 더한 스폰 위치 계산
            Vector3 spawnPosition = terrain.transform.position + new Vector3(randomX, 50f, randomZ);

            // 스폰 위치에 몬스터 오브젝트 생성
            GameObject monsterObject = Instantiate(createMonster, spawnPosition, Quaternion.identity);

            // 몬스터 아래 방향으로 레이캐스트하여 지면 또는 다른 오브젝트 찾기
            RaycastHit hit;
            if (Physics.Raycast(monsterObject.transform.position, Vector3.down, out hit))
            {
                // 레이가 충돌한 오브젝트가 몬스터 자체인 경우
                if (hit.collider.gameObject == gameObject)
                {
                    // 몬스터의 y축 값을 레이 충돌 지점의 y축 값으로 설정
                    monsterObject.transform.position = new Vector3(monsterObject.transform.position.x, hit.point.y, monsterObject.transform.position.z);
                }
            }
        }

        nowCount = maxCreateMonster;
    }
}