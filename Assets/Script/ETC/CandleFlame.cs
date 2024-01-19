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
            // �� flame�� ���� sin �� ���·� x�� �̵�
            float xPosition = -Mathf.Sin(Time.time * frequency + i) * amplitude;

            // ���� flame�� ��ġ�� ���� ���� ��ġ�� ���� (y�� z ���� ������ ����)
            Vector3 newPosition = new Vector3(candlePos[i].x + xPosition, flames[i].transform.localPosition.y, flames[i].transform.localPosition.z);

            // ���� flame�� ��ġ�� ���� ���� ��ġ�� ����
            flames[i].transform.localPosition = newPosition;
        }
    }
}
