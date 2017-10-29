using System;
using UnityEngine;

// Namespace that contains all the component that are used in implementing the logic for the ability
namespace Svelto.ECS.Example.Components.PlayerAbilities
{
    // Component used to personalize the power of the pushing ability
    public interface IPushAbilityComponent: IComponent
    {
        float cooldown { get; }
        float minAffectDistance { get; }
        float maxPushPower { get; }
    }

    public struct PushAbilityInfo
    {
        private Vector3 _pushPower;
        private float _cooldown;
        private int _enemyAffectedID;

        public Vector3 pushPower { get { return _pushPower; } }
        public float cooldown { get { return _cooldown; } }
        public int enemyAffectedID { get { return _enemyAffectedID; } }

        public PushAbilityInfo(Vector3 pushPower, float cooldown, int enemyAffectedID)
        {
            _pushPower = pushPower; _cooldown = cooldown; _enemyAffectedID = enemyAffectedID;
        }
    }
    // General component for a general ability. Each ability should be triggered by a specific trigger with a specific name
    public interface IAbilityInputNameComponent: IComponent
    {
        string triggerInputName { get; }
    }


}

