using Svelto.ECS.Example.Components.Damageable;
using Svelto.ECS.Example.Components.Pickup;
using Svelto.ECS.Example.Nodes.DamageableEntities;
using System;

namespace Svelto.ECS.Example.Engines.Health
{
    public class HealthEngine : IEngine, IQueryableNodeEngine, IStep<DamageInfo>, IStep<PlayerDamageInfo>, IStep<HealthPickupData>
    {
        private Sequencer _damageSequence;
        private Sequencer _healthPickupSequence;

        public HealthEngine(Sequencer playerDamageSequence, Sequencer healthPickupSequence)
        {
            _damageSequence = playerDamageSequence;
            _healthPickupSequence = healthPickupSequence;
        }

        public HealthEngine(Sequencer playerDamageSequence)
        {
            _damageSequence = playerDamageSequence;
            _healthPickupSequence = null;
        }

        public IEngineNodeDB nodesDB { set; private get; }

        public void Step(ref PlayerDamageInfo token, Enum condition)
        {
            TriggerDamage(ref token);
        }

        public void Step(ref HealthPickupData token, Enum condition)
        {
            if (_healthPickupSequence == null) return;

            if((PickupState)condition == PickupState.picking)
            {
                var node = nodesDB.QueryNode<HealthNode>(token.pickingEntityID);
                var healthComponent = node.healthComponent;

                healthComponent.currentHealth = Math.Min(token.maxResource, healthComponent.currentHealth + token.resourceIncrease);

                _healthPickupSequence.Next(this, ref token, PickupState.picked);
            }
        }

        public void Step(ref DamageInfo token, Enum condition)
        {
            TriggerDamage(ref token);
        }

        void TriggerDamage<T>(ref T damage) where T:IDamageInfo
        {
            var node = nodesDB.QueryNode<HealthNode>(damage.entityDamaged);
            var healthComponent = node.healthComponent;

            healthComponent.currentHealth -= damage.damagePerShot;

            if (healthComponent.currentHealth <= 0)
            {
                _damageSequence.Next(this, ref damage, DamageCondition.dead);
                node.removeEntityComponent.removeEntity();
            }
            else
                _damageSequence.Next(this, ref damage, DamageCondition.damage);
        }
    }
}
