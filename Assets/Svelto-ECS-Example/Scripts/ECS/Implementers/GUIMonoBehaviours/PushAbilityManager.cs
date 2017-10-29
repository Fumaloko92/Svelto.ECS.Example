using Svelto.ECS.Example.Components.HUD;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Implementers.HUD
{
    // Class used in the HUD to show the cooldown of the pushing skill
    public class PushAbilityManager : MonoBehaviour, ISkillCooldownComponent
    {
        float ISkillCooldownComponent.cooldownPerc { get { return _cooldownPerc; } set { _cooldownPerc = value; _image.fillAmount = _cooldownPerc; } }
        float _cooldownPerc;

        // This can't be initialized in the Awake function because there are multiple images in the GameObject hierarchy
        [SerializeField]
        private Image _image;
    }
}
