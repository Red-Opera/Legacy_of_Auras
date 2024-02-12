using System.Collections;
using UnityEngine;

public class PlayerGetGun : MonoBehaviour
{
    public GameObject gun;              // 총 오브젝트

    private Animator animator;          // 애니메이터 컴포넌트
    private float rotationSpeed = 0.5f; // 회전 속도

    private GameObject camera;          // 카메라 오브젝트
    private Vector2 startRotation;      // 회전 시작 시의 플레이어의 회전
    private Vector2 endRotation;        // 목표 회전
    private float rotationStartTime;    // 회전 시작 시간
    private bool isRotating = false;    // 회전 중인지 여부

    public void Start()
    {
        animator = GetComponent<Animator>();
        camera = GameObject.Find("MainCamera");

        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이터가 존재하지 않습니다.");
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

            // 카메라의 회전을 수정 (위아래 방향은 움직이지 않도록 함)
            Vector3 cameraEulerAngles = camera.transform.rotation.eulerAngles;
            camera.transform.rotation = Quaternion.Euler(0, cameraEulerAngles.y, cameraEulerAngles.z);

            yield return null;
        }

        playerRotate.currentRotation = endRotation; // 목표 회전값으로 설정
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