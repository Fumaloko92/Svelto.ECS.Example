using System;
using Svelto.ECS.Example.Components.Gun;
using Svelto.ECS.Example.Components.PlayerAbilities;
using UnityEngine;
using Svelto.ECS.Example.Components.Base;

namespace Svelto.ECS.Example.Implementers.Player
{
    // The implementer of the player pushing skill
    public class PlayerPushingAbility : MonoBehaviour, IPushAbilityComponent, IAbilityInputNameComponent, ITransformComponent
    {
        public float _cooldown;
        public float _maxPushPower;
        public float _minAffectDistance;
        public string _triggerInputName;

        float IPushAbilityComponent.cooldown { get { return _cooldown; } }
        float IPushAbilityComponent.maxPushPower { get { return _maxPushPower; } }
        float IPushAbilityComponent.minAffectDistance { get { return _minAffectDistance; } }
        string IAbilityInputNameComponent.triggerInputName { get { return _triggerInputName; } }
    }
}
