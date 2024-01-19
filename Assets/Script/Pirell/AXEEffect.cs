using System.Collections.Generic;
using UnityEngine;

public class AXEEffect : MonoBehaviour
{
    public GameObject leftAXE;
    public GameObject rightAXE;
    public GameObject effect;
    public GameObject fireAXE;

    private Animator animator;
    private GameObject player;
    private bool isPlay = false;
    private bool isCreateAXE = false;

    private int destructionAXECount = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 피렐의 애니메이션이 존재하지 않습니다.");

        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");

        effect.transform.localScale = Vector3.one;
    }

    void Update()
    {
        AnimatorStateInfo currentAni = animator.GetCurrentAnimatorStateInfo(0);
        effect.transform.rotation = Quaternion.identity;

        if (currentAni.IsName("GroundStrike") && currentAni.normalizedTime >= 0.469 && !isPlay)
        {
            isPlay = true;

            AllActivateEffect();
        }

        else if (currentAni.IsName("BurningFlameSlash") && currentAni.normalizedTime >= 0.327 && !isPlay)
        {
            isPlay = true;
            RightActivateEffect(animator.GetCurrentAnimatorStateInfo(0));
        }

        else if (currentAni.IsName("DestructionAxe") && currentAni.normalizedTime >= 0.05f)
        {
            Vector3 defaultScale = effect.transform.localScale;

            effect.transform.localScale = effect.transform.localScale * 2f;
            isPlay = true;
            DestructionAXEEffect(currentAni);

            effect.transform.localScale = defaultScale;
        }

        else if (currentAni.normalizedTime < 0.1)
            isPlay = false;

        if (currentAni.IsName("AXECreate"))
            CreateAXEEffect();

        else
            isCreateAXE = false;
    }

    private void DestructionAXEEffect(AnimatorStateInfo currentAni)
    {
        Quaternion childRotate = effect.transform.GetChild(0).rotation;

        if (currentAni.normalizedTime >= 0.19 && destructionAXECount == 0)
        {
            effect.transform.GetChild(0).Rotate(-20.0f, 240.0f, -90.0f);
            destructionAXECount++;
            RightActivateEffect(currentAni);

            effect.transform.GetChild(0).rotation = childRotate;
        }

        else if (currentAni.normalizedTime >= 0.373 && destructionAXECount == 1)
        {
            effect.transform.GetChild(0).Rotate(0.0f, 180.0f, 0.0f);
            destructionAXECount++;
            RightActivateEffect(currentAni);

            effect.transform.GetChild(0).rotation = childRotate;
        }

        else if (currentAni.normalizedTime >= 0.587 && destructionAXECount == 2)
        {
            effect.transform.GetChild(0).Rotate(0.0f, 180.0f, 0.0f);
            destructionAXECount++;
            RightActivateEffect(currentAni);

            effect.transform.GetChild(0).rotation = childRotate;
        }
        
        else if (currentAni.normalizedTime < 0.15f)
            destructionAXECount = 0;
    }

    // 모든 도끼 공격 효과
    private void AllActivateEffect()
    {
        Vector3 leftPos = leftAXE.transform.position;
        Quaternion leftRo = leftAXE.transform.rotation;
        
        Vector3 rightPos = rightAXE.transform.position;
        Quaternion rightRo = rightAXE.transform.rotation;

        Destroy(Instantiate(effect, leftPos, leftRo), 1.5f);
        Destroy(Instantiate(effect, rightPos, rightRo), 1.5f);
    }

    // 왼쪽 도끼 공격 효과
    private void LeftActivateEffect()
    {
        Vector3 leftPos = leftAXE.transform.position;
        Quaternion leftRo = leftAXE.transform.rotation;

        Destroy(Instantiate(effect, leftPos, leftRo), 1.5f);
    }

    // 오른쪽 도끼 공격 효과
    private void RightActivateEffect(AnimatorStateInfo currentAni)
    {
        Vector3 rightPos = rightAXE.transform.position;
        Quaternion rightRo = rightAXE.transform.rotation;

        Quaternion childRotate = effect.transform.GetChild(0).rotation;

        if (currentAni.IsName("BurningFlameSlash"))
            effect.transform.GetChild(0).Rotate(new Vector3(-20, -10, -20));

        Destroy(Instantiate(effect, rightPos, rightRo), 1.5f);
        effect.transform.GetChild(0).rotation = childRotate;
    }

    private void CreateAXEEffect()
    {
        if (isCreateAXE)
            return;

        isCreateAXE = true;

        List<GameObject> newAxe = new List<GameObject>();

        for (int i = 0; i < 3; i++)
            newAxe.Add(Instantiate(fireAXE, transform));

        newAxe[0].transform.localPosition = new Vector3(25, 25, 3f);
        newAxe[1].transform.localPosition = new Vector3(0, 40, 3f);
        newAxe[2].transform.localPosition = new Vector3(-25, 25, 3f);

        for (int i = 0; i < 3; i++)
        {
            newAxe[i].transform.localScale = new Vector3(1000, 1000, 1000);
            newAxe[i].transform.SetParent(null);
            newAxe[i].transform.LookAt(player.transform);
        }
    }
}