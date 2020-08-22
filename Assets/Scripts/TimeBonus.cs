using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GamePiece))]
public class TimeBonus : MonoBehaviour
{
    [Range(0,5)]
    public int BonusValue = 5;

    [Range(0f, 1f)]
    public float ChanceForBonus = 0.1f;

    public Material[] bonusMaterials;
    public GameObject BonusGlow;
    public GameObject RingGlow;

    private void Start()
    {
        float random = Random.Range(0f, 1f);
        if (random > ChanceForBonus)
            BonusValue = 0;

        if(GameManager.Instance!=null)
        {
            if(GameManager.Instance.LevelGoal.LevelCounter==LevelCounter.Moves)
            {
                BonusValue = 0;
            }
        }

        SetActive(BonusValue != 0);

        if(BonusValue!=0)
        {
            SetupMaterial(BonusValue - 1, BonusGlow);
        }
    }
    void SetActive(bool state)
    {
        if (BonusGlow != null)
            BonusGlow.SetActive(state);
        if (RingGlow != null)
            RingGlow.SetActive(state);
    }
    void SetupMaterial(int value, GameObject bonusGlow)
    {
        int clampedValue = Mathf.Clamp(value, 0, bonusMaterials.Length - 1);
        if(bonusMaterials[clampedValue]!=null)
        {
            if(BonusGlow!=null)
            {
                ParticleSystemRenderer bonuseGlowRenderer = BonusGlow.GetComponent<ParticleSystemRenderer>();
                bonuseGlowRenderer.material = bonusMaterials[clampedValue];
            }
        }
    }
}
