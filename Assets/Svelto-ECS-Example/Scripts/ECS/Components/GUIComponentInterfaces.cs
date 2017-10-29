using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Components.HUD
{
    public interface IAnimatorHUDComponent: IComponent
    {
        Animator hudAnimator { get; }
    }

    public interface IDamageHUDComponent: IComponent
    {
        Image damageImage { get; }
        float flashSpeed { get; }
        Color flashColor { get; }
    }

    public interface IHealthSliderComponent: IComponent
    {
        Slider healthSlider { get; }
    }

    public interface IScoreComponent: IComponent
    {
        int score { set; get; }
    }

    // Component used from the GUI to hold information about the current wave index and if the wave is the last wave
    // - such information is known only to the EnemyWavesEngine
    public interface IWaveComponent: IComponent
    {
        int wave { get; set; }
        bool isLastWave { get; set; }
    }

    public struct WaveInfo: IWaveComponent
    {
        public int wave { get; set; }
        public bool isLastWave { get; set; }

        public WaveInfo(int waveIndex, bool lastWave)
        {
            wave = waveIndex; isLastWave = lastWave;
        }
    }

    // Component used from the GUI to hold information about the current Ammo of the player
    public interface IAmmoComponent : IComponent
    {
        int currentAmmo { get; set; }
    }


    // Component used to share information between the logic of a skill and the HUD
    public interface ISkillCooldownComponent : IComponent
    {
        float cooldownPerc { get; set; }
    }

    public struct PushAbilityCooldownInfo : ISkillCooldownComponent
    {
        private float _cooldownPerc;

        public float cooldownPerc { get { return _cooldownPerc; } set { _cooldownPerc = value; } }
        public PushAbilityCooldownInfo(float cooldownPerc)
        {
            _cooldownPerc = cooldownPerc;
        }
    }
}
