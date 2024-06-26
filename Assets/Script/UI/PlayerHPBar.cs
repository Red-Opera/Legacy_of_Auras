using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    public TextMeshProUGUI currentHp;       // 플레이어 현재 체력
    public TextMeshProUGUI maxHp;           // 플레이어 최대 체력

    [SerializeField] private PlayerState state;     // 플레이어 현재 상태
    [SerializeField] private Slider currentUI;      // 플레이어 체력바
    [SerializeField] private VolumeProfile volume;  // 포스트 프로세스 볼륨

    [SerializeField] private float minFilmGrain = 0.0f;
    [SerializeField] private float maxFilmGrain = 0.6f;

    [SerializeField] private float minVignette = 0.0f;
    [SerializeField] private float maxVignette = 0.4f;

    private FilmGrain grain;
    private Vignette vignette;

    public void Start()
    {
        Debug.Assert(currentHp != null, "Error (Null Reference) : 플레이어 현재 체력이 존재하지 않습니다.");
        Debug.Assert(maxHp != null, "Error (Null Reference) : 플레이어 최대 체력이 존재하지 않습니다.");
        Debug.Assert(currentUI != null, "Error (Null Reference) : 플레이어 현재 체력바가 존재하지 않습니다.");
        Debug.Assert(volume != null, "Error (Null Reference) : 포스트 프로세스를 실행할 볼륨이 존재하지 않습니다.");

        volume.TryGet<FilmGrain>(out grain);
        volume.TryGet<Vignette>(out vignette);

        Debug.Assert(grain != null, "Error (Null Reference) : FileGrain이 존재하지 않습니다.");
        Debug.Assert(vignette != null, "Error (Null Reference) : Vignette가 존재하지 않습니다.");

        grain.intensity.value = 0.0f;
        vignette.intensity.value = 0.0f;

        currentHp.text = state.HP.ToString("#,##0");
        maxHp.text = state.HP.ToString("#,##0");
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

    public bool Heal(int heal)
    {
        int currentHP = int.Parse(currentHp.text);

        if (currentHP >= state.HP)
            return false;

        int resultHP = currentHP + heal;
        if (resultHP >= state.HP)
            currentHp.text = state.HP.ToString();

        else
            currentHp.text = resultHP.ToString();

        return true;
    }
}
