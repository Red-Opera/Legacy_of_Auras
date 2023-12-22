using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerGunShotUI : MonoBehaviour
{
    public PlayerState playerState;         // �÷��̾��� ����

    public GameObject emptyCartridge;       // ź�� ���� �ִϸ��̼�
    public GameObject gunFire;              // �� ȭ�� �ִϸ��̼�
    public GameObject gunImg;               // �� �̹��� ������Ʈ

    public TextMeshProUGUI currentText;     // źâ�� �����ִ� ź�� ��
    public TextMeshProUGUI remainText;      // ���� �����ִ� ź�� ��

    public Color maxCurrentColor;           // ���� źâ�� ���� ���� �� ����
    public Color zeroCurrentColor;          // ���� źâ�� ������� �� ����

    public float endFontSize;               // ��� �� �ּҷ� �۾����� ��Ʈ ũ��
    public float resizeDuration;            // ��� �� �۾����� �ӵ�

    private float startFontSize;            // ��Ʈ�� �⺻ ũ��

    public void Start()
    {
        Debug.Assert(playerState != null, "Error (Null Reference) : �÷��̾� ���°� �������� �ʽ��ϴ�.");
        Debug.Assert(emptyCartridge != null, "Error (Null Reference) : ź�� ����Ʈ�� ����� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(gunFire != null, "Error (Null Reference) : ȭ���� ���� �ؽ�Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(currentText != null, "Error (Null Reference) : źâ�� �ִ� �Ѿ��� ����� �ؽ�Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(remainText != null, "Error (Null Reference) : �����ִ� �Ѿ��� ����� �ؽ�Ʈ�� �������� �ʽ��ϴ�.");

        remainText.text = playerState.bulletCount.ToString();
        currentText.text = playerState.bulletCurrentMax.ToString();

        startFontSize = currentText.fontSize;

        SetTextColor();
    }

    // �Ѿ� ���� ����Ʈ
    public IEnumerator ReduceBullet()
    {
        StopCoroutine(ReduceFontSize());
        currentText.fontSize = startFontSize;
        StartCoroutine(ReduceFontSize());

        // źâ�� �ִ� �Ѿ��� ���� ���ҽ�Ŵ
        currentText.text = (Int32.Parse(currentText.text) - 1).ToString();
        SetTextColor();

        // ȭ�� ����Ʈ�� Ȱ��ȭ�ϰ� �� �̹����� 35���� ��︲
        gunFire.SetActive(true);
        gunImg.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 35.0f);

        // ź�� ����
        GameObject newCartridge = Instantiate(emptyCartridge, gunImg.transform);
        newCartridge.layer = 6;
        newCartridge.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        newCartridge.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        newCartridge.GetComponent<ParticleSystem>().gravityModifier = 2.0f;
        Destroy(newCartridge, 1.0f);

        yield return new WaitForSeconds(0.1f);

        // ���� �ٽ� ����
        gunImg.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        yield return new WaitForSeconds(0.05f);
        gunFire.SetActive(false);
    }

    // ���� �Ѿ��� ũ�⸦ �����ϴ� �޼ҵ�
    private IEnumerator ReduceFontSize()
    {
        // ����� �ð�, ���� ���� ũ��
        float elapsedTime = 0, startSize = startFontSize;

        // ���ڸ� ���̴� ȿ��
        while (elapsedTime < resizeDuration / 2)
        {
            float newSize = Mathf.Lerp(startSize, endFontSize, elapsedTime / (resizeDuration / 2));
            currentText.fontSize = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���ڸ� Ű��� ȿ��
        while (elapsedTime < resizeDuration)
        {
            float newSize = Mathf.Lerp(endFontSize, startSize, (resizeDuration / 2 - (elapsedTime / 2)) / (resizeDuration / 2));
            currentText.fontSize = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ٽ� ���� ũ��� ����
        currentText.fontSize = startFontSize; // ��Ȯ�� ũ��� ����
    }

    // ���� źâ�� ������ ���� ������ ����
    public void SetTextColor()
    {
        float bulletRatio = (float)Int32.Parse(currentText.text) / playerState.bulletCurrentMax;

        Color redColor = zeroCurrentColor;
        Color greenColor = maxCurrentColor;

        // �߰� �� ���
        Color interpolatedColor = Color.Lerp(redColor, greenColor, bulletRatio);

        currentText.color = interpolatedColor;
    }
}