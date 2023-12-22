using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamageSound : MonoBehaviour
{
    public AudioClip pirellDamage;
    public AudioClip zeroDamage;

    private AudioSource audio;

    public void Start()
    {
        audio = GetComponent<AudioSource>();
        Debug.Assert(audio != null, "Error (Null Reference): ����� �ҽ��� �������� �ʽ��ϴ�.");
    }

    public void DamageSound(int damage)
    {
        if (damage <= 0)
        {
            audio.PlayOneShot(zeroDamage);
            return;
        }

        if (name.Contains("Pirell"))
            audio.PlayOneShot(pirellDamage);
    }
}
