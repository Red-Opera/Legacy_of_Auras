using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AXEEffect : MonoBehaviour
{
    public GameObject leftAXE;
    public GameObject rightAXE;
    public GameObject effect;

    private Animator animator;
    private bool isPlay = false;

    private int destructionAXECount = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 피렐의 애니메이션이 존재하지 않습니다.");

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
    }

    private void DestructionAXEEffect(AnimatorStateInfo currentAni)
    {
        if (currentAni.normalizedTime >= 0.19 && destructionAXECount == 0)
        {
            destructionAXECount++;
            RightActivateEffect(currentAni);
        }

        else if (currentAni.normalizedTime >= 0.373 && destructionAXECount == 1)
        {
            destructionAXECount++;
            RightActivateEffect(currentAni);
        }

        else if (currentAni.normalizedTime >= 0.587 && destructionAXECount == 2)
        {
            destructionAXECount++;
            RightActivateEffect(currentAni);
        }
        
        else if (currentAni.normalizedTime < 0.15f)
            destructionAXECount = 0;
    }

    private void AllActivateEffect()
    {
        Vector3 leftPos = leftAXE.transform.position;
        Quaternion leftRo = leftAXE.transform.rotation;
        
        Vector3 rightPos = rightAXE.transform.position;
        Quaternion rightRo = rightAXE.transform.rotation;

        Destroy(Instantiate(effect, leftPos, leftRo), 1.5f);
        Destroy(Instantiate(effect, rightPos, rightRo), 1.5f);
    }

    private void LeftActivateEffect()
    {
        Vector3 leftPos = leftAXE.transform.position;
        Quaternion leftRo = leftAXE.transform.rotation;

        Destroy(Instantiate(effect, leftPos, leftRo), 1.5f);
    }

    private void RightActivateEffect(AnimatorStateInfo currentAni)
    {
        Vector3 rightPos = rightAXE.transform.position;
        Quaternion rightRo = rightAXE.transform.rotation;

        if (currentAni.IsName("BurningFlameSlash"))
            effect.transform.localRotation = Quaternion.Euler(160f, 100f, -80f);

        Destroy(Instantiate(effect, rightPos, rightRo), 1.5f);
    }
}
