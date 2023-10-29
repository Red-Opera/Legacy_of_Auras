using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamageSound : MonoBehaviour
{
    public AudioClip pirellDamage;

    private AudioSource audio;

    public void Start()
    {
        audio = GetComponent<AudioSource>();
        Debug.Assert(audio != null, "Error (Null Reference): 오디오 소스가 존재하지 않습니다.");
    }

    public void DamageSound()
    {
        if (name.Contains("Pirell"))
            audio.PlayOneShot(pirellDamage);
    }
}
