using System.Collections;
using UnityEngine;

public class PlayerGetAurasArrow : MonoBehaviour
{
    public static bool isGetting = false;   // ���� ȭ���� ��� �ִ��� ����
    public static bool isGetArrow = true;

    private Animator animator;              // �ִϸ����� ������Ʈ
    private float rotationSpeed = 0.5f;     // ȸ�� �ӵ�

    private GameObject camera;              // ī�޶� ������Ʈ
    private Vector2 startRotation;          // ȸ�� ���� ���� �÷��̾��� ȸ��
    private Vector2 endRotation;            // ��ǥ ȸ��
    private float rotationStartTime;        // ȸ�� ���� �ð�
    private bool isRotating = false;        // ȸ�� ������ ����


    public void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference): �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        camera = GameObject.Find("MainCamera");
    }

    public void Update()
    {
        
    }

    private IEnumerator GetAurasArrow(Transform targetTransform)
    {
        isGetting = true;

        if (isRotating)
            yield break;

        PlayerRotate playerRotate = GetComponent<PlayerRotate>();
        startRotation = playerRotate.currentRotation;
        endRotation = new Vector2(targetTransform.position.x - transform.position.x - 50, targetTransform.position.y - transform.position.y);
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

        animator.SetTrigger("GetAurasArrow");
        targetTransform.GetComponent<ParticleSystem>().Play();
        isGetArrow = true;

        yield return new WaitForSeconds(0.3f);

        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                targetTransform.GetComponent<BoxCollider>().enabled = true;
                isGetting = false;
                yield break;
            }

            yield return null;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.name == "AurasArrow" && Input.GetKeyDown(KeyCode.E))
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || isGetArrow)
                return;

            other.GetComponent<BoxCollider>().enabled = false;

            StartCoroutine(GetAurasArrow(other.transform));
        }
    }
}
