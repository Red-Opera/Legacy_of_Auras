using System.Collections;
using UnityEngine;

public class PlayerGetGun : MonoBehaviour
{
    public GameObject gun;              // �� ������Ʈ

    private Animator animator;          // �ִϸ����� ������Ʈ
    private float rotationSpeed = 0.5f; // ȸ�� �ӵ�

    private GameObject camera;          // ī�޶� ������Ʈ
    private Vector2 startRotation;      // ȸ�� ���� ���� �÷��̾��� ȸ��
    private Vector2 endRotation;        // ��ǥ ȸ��
    private float rotationStartTime;    // ȸ�� ���� �ð�
    private bool isRotating = false;    // ȸ�� ������ ����

    public void Start()
    {
        animator = GetComponent<Animator>();
        camera = GameObject.Find("MainCamera");

        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.name == "GetGunBox" && Input.GetKeyDown(KeyCode.E))
        {
            if (gun.activeSelf || !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                return;

            other.GetComponent<BoxCollider>().enabled = false;

            StartCoroutine(GetGun(other.transform));
        }
    }

    private IEnumerator GetGun(Transform targetTransform)
    {
        if (isRotating)
            yield break;

        PlayerRotate playerRotate = GetComponent<PlayerRotate>();
        startRotation = playerRotate.currentRotation;
        endRotation = new Vector2(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y + 20f); 
        rotationStartTime = Time.time;

        isRotating = true;

        while (Time.time - rotationStartTime < rotationSpeed)
        {
            float journeyTime = Time.time - rotationStartTime;
            float fractionOfJourney = Mathf.Clamp01(journeyTime / rotationSpeed);

            playerRotate.currentRotation = Vector2.Lerp(startRotation, endRotation, fractionOfJourney);

            // ī�޶��� ȸ���� ���� (���Ʒ� ������ �������� �ʵ��� ��)
            Vector3 cameraEulerAngles = camera.transform.rotation.eulerAngles;
            camera.transform.rotation = Quaternion.Euler(0, cameraEulerAngles.y, cameraEulerAngles.z);

            yield return null;
        }

        playerRotate.currentRotation = endRotation; // ��ǥ ȸ�������� ����
        isRotating = false;

        animator.SetTrigger("GetGun");

        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95)
            {
                gun.SetActive(true);
                targetTransform.GetComponent<BoxCollider>().enabled = true;
                yield break;
            }

            yield return null;
        }
    }
}