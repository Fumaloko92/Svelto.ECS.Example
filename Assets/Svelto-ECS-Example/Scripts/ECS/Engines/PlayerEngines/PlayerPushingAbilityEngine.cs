using System;
using UnityEngine;
using Svelto.ECS.Example.Components.HUD;
using Svelto.ECS.Example.Nodes.Enemies;
using Svelto.ECS.Example.Nodes.PlayerAbilities;
using Svelto.ECS.Example.Components.PlayerAbilities;

namespace Svelto.ECS.Example.Engines.Player.Ability
{
    // This class implement the first part of the logic for implementing the pushing skill. Indeed, this engine
    // takes care of detecting the player input, triggering the stopping of movement of the affected enemies(performed in the EnemyMovementEngine)
    // and the the pushing back of the such enemies
    public class PlayerPushingAbilityEngine : INodesEngine, IQueryableNodeEngine
    {
        private Sequencer _abilitySequence;

        internal class PushAbilityData
        {
            internal string triggerName;
            internal float cooldown;
            internal float timerCooldown;
            internal float minDistance;
            internal float maxPushPower;
            internal Transform abilityTransform;

            public PushAbilityData(float cooldown, string triggerName, Transform abilityTransform, float minDistance, float maxPushPower)
            {
                this.cooldown = cooldown;
                timerCooldown = 0;
                this.triggerName = triggerName;
                this.abilityTransform = abilityTransform;
                this.minDistance = minDistance;
                this.maxPushPower = maxPushPower;
            }
        }
        public IEngineNodeDB nodesDB { set; private get; }

        public PlayerPushingAbilityEngine(Sequencer abilitySequence)
        {
            _abilitySequence = abilitySequence;
            TaskRunner.Instance.Run(new Tasks.TimedLoopActionEnumerator(Tick));
        }

        public Type[] AcceptedNodes() { return _acceptedNodes; }

        public void Add(INode obj)
        {
            if (obj is PushAbilityNode)
            {
                var node = obj as PushAbilityNode;
                _pushAbilityData = new PushAbilityData(node.pushAbilityComponent.cooldown, node.pushAbilityInputNameComponent.triggerInputName, node.transformComponent.transform,
                    node.pushAbilityComponent.minAffectDistance, node.pushAbilityComponent.maxPushPower);
            }
        }

        public void Remove(INode obj)
        {
            if (obj is PushAbilityNode)
                _pushAbilityData = null;
        }


        void Tick(float deltaSec)
        {
            if (_pushAbilityData == null) return;

            if (Input.GetButton(_pushAbilityData.triggerName) && _pushAbilityData.timerCooldown <= 0)
                UsePushAbility();
            else
                _pushAbilityData.timerCooldown = Math.Max(0, _pushAbilityData.timerCooldown-deltaSec);

            PushAbilityCooldownInfo cooldownInfo = new PushAbilityCooldownInfo(_pushAbilityData.timerCooldown/_pushAbilityData.cooldown);
            _abilitySequence.Next(this, ref cooldownInfo);
        }


        void UsePushAbility()
        {
            _pushAbilityData.timerCooldown = _pushAbilityData.cooldown;

            foreach (EnemyNode enemyNode in nodesDB.QueryNodes<EnemyNode>())
            {
                float distance = Vector3.Distance(enemyNode.transformComponent.transform.position, _pushAbilityData.abilityTransform.position);
                if (distance <= _pushAbilityData.minDistance)
                {
                    Vector3 pushPower = enemyNode.transformComponent.transform.position - _pushAbilityData.abilityTransform.position;
                    pushPower *= _pushAbilityData.maxPushPower * (1 - distance / _pushAbilityData.minDistance);

                    PushAbilityInfo data = new PushAbilityInfo(pushPower, _pushAbilityData.cooldown, enemyNode.ID);
                    _abilitySequence.Next(this, ref data, PushAbilityState.pushing);

                }
            }
        }



        PushAbilityData _pushAbilityData;

        readonly Type[] _acceptedNodes = { typeof(PushAbilityNode) };
    }
}
