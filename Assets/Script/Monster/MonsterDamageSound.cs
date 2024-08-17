using UnityEngine;

public class MonsterDamageSound : MonoBehaviour
{
    public AudioClip damageSound;
    public AudioClip zeroDamage;

    private AudioSource audio;

    public void Start()
    {
        audio = GetComponent<AudioSource>();
        Debug.Assert(audio != null, "Error (Null Reference): ����� �ҽ��� �������� �ʽ��ϴ�.");
    }

    public void DamageSound(int damage)
    {
        audio.volume = GameManager.info.soundVolume;

        if (damage <= 0)
        {
            audio.PlayOneShot(zeroDamage);
            return;
        }

        if (damageSound != null)
            audio.PlayOneShot(damageSound);
    }
}
