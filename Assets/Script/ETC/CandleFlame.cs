using UnityEngine;

public class CandleFlame : MonoBehaviour
{
    public GameObject[] flames;
    public float amplitude = 2.0f;
    public float frequency = 2.5f;

    private Vector3 position;
    private Vector3[] candlePos;

    void Start()
    {
        //transform.position = new Vector3(0.18f, 10.47f, -5.930125f);
        position = transform.position;
        transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

        transform.position = position;

        candlePos = new Vector3[flames.Length];
        for (int i = 0; i < flames.Length; i++)
            candlePos[i] = flames[i].transform.localPosition;
    }

    void Update()
    {
        for (int i = 0; i < flames.Length; i++)
        {
            // 각 flame에 대해 sin 파 형태로 x축 이동
            float xPosition = -Mathf.Sin(Time.time * frequency + i) * amplitude;

            // 현재 flame의 위치를 새로 계산된 위치로 설정 (y와 z 축은 변하지 않음)
            Vector3 newPosition = new Vector3(candlePos[i].x + xPosition, flames[i].transform.localPosition.y, flames[i].transform.localPosition.z);

            // 현재 flame의 위치를 새로 계산된 위치로 설정
            flames[i].transform.localPosition = newPosition;
        }
    }
}
