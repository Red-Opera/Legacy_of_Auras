using System.Collections;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;     // ȸ�� �ӵ�
    [SerializeField] private float startDuration;   // ���ؼ� ũ�� ȸ�� �ð�

    static private GameObject player;               // �÷��̾�

    private Transform child;            // ���ؼ� ������Ʈ
    private Vector3 originalCrossSize;  // �ʱ� ���ؼ� ũ��
    private bool isEnable = false;      // ���� Ȱ��ȭ ����

    private void Start()
    {
        if (player == null)
            player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference): �÷��̾ �������� �ʽ��ϴ�.");

        child = transform.GetChild(0);
    }

    private void Update()
    {
        Debug.Assert(player != null, "Error (Null Reference): �÷��̾ �������� �ʽ��ϴ�.");

        if (child.gameObject.activeSelf)
        {
            if (!isEnable)
            {
                isEnable = true;
                StartCoroutine(StartCrossHair());
            }

            // �÷��̾ �׻� �ٶ󺸰� õõ�� ȸ����
            transform.LookAt(player.transform.position);
            child.localRotation *= Quaternion.Euler(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        }

        else
            isEnable = false;
    }

    // ���ؼ� ������ �� �����ϴ� �޼ҵ�
    private IEnumerator StartCrossHair()
    {
        // �÷��� �ð��� �ʱ�ȭ�ϰ� ���� ũ�⸦ ������
        float elapsedTime = 0f;
        originalCrossSize = transform.localScale;

        // ������ �ð����� ũ�⸦ �ø�
        while (elapsedTime < startDuration)
        {
            child.localScale = Vector3.Lerp(Vector3.zero, originalCrossSize, elapsedTime / startDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        child.localScale = originalCrossSize; // ���� ũ�� ����
    }
}
