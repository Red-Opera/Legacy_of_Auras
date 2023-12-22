using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    public PlayerState state;               // �÷��̾� ���� ����
    public TextMeshProUGUI currentHp;       // �÷��̾� ���� ü��
    public Slider currentUI;                // �÷��̾� ü�¹�
    public VolumeProfile volume;            // ����Ʈ ���μ��� ����

    public float minFilmGrain = 0.0f;
    public float maxFilmGrain = 0.6f;

    public float minVignette = 0.0f;
    public float maxVignette = 0.4f;

    private FilmGrain grain;
    private Vignette vignette;

    public void Start()
    {
        Debug.Assert(currentHp != null, "Error (Null Reference) : �÷��̾� ���� ü���� �������� �ʽ��ϴ�.");
        Debug.Assert(currentUI != null, "Error (Null Reference) : �÷��̾� ���� ü�¹ٰ� �������� �ʽ��ϴ�.");
        Debug.Assert(volume != null, "Error (Null Reference) : ����Ʈ ���μ����� ������ ������ �������� �ʽ��ϴ�.");

        volume.TryGet<FilmGrain>(out grain);
        volume.TryGet<Vignette>(out vignette);

        Debug.Assert(grain != null, "Error (Null Reference) : FileGrain�� �������� �ʽ��ϴ�.");
        Debug.Assert(vignette != null, "Error (Null Reference) : Vignette�� �������� �ʽ��ϴ�.");

        grain.intensity.value = 0.0f;
        vignette.intensity.value = 0.0f;
    }

    public void Update()
    {
        float persent = (float)Int32.Parse(currentHp.text) / state.HP;

        currentUI.value = persent;

        grain.intensity.value = Mathf.Lerp(maxFilmGrain, minFilmGrain, persent);
        vignette.intensity.value = Mathf.Lerp(maxVignette, minVignette, persent);
    }

    public void SetDamage(int damage)
    {
        int hp = Int32.Parse(currentHp.text);

        if (damage > hp)
            currentHp.SetText("0");

        else
            currentHp.SetText((hp - damage).ToString());
    }
}
