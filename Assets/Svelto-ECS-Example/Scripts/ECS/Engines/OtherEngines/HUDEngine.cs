using Svelto.ECS.Example.Components.Ammo;
using Svelto.ECS.Example.Components.Damageable;
using Svelto.ECS.Example.Components.Pickup;
using Svelto.ECS.Example.Components.HUD;
using Svelto.ECS.Example.Nodes.HUD;
using System;
using UnityEngine;

namespace Svelto.ECS.Example.Engines.HUD
{
    public class HUDEngine : INodesEngine, IQueryableNodeEngine, IStep<PlayerDamageInfo>, IStep<HealthPickupData>, IStep<AmmoPickupData>, IStep<AmmoGunShootingInfo>, IStep<PushAbilityCooldownInfo>, IStep<WaveInfo>
    {
        public IEngineNodeDB nodesDB { set; private get; }

        public Type[] AcceptedNodes()
        {
            return _acceptedNodes;
        }

        public HUDEngine()
        {
            TaskRunner.Instance.Run(new Tasks.TimedLoopActionEnumerator(Tick));
        }

        public void Add(INode obj)
        {
            if (obj is HUDNode)
                _guiNode = obj as HUDNode;
        }

        public void Remove(INode obj)
        {
            if (obj is HUDNode)
                _guiNode = null;
        }

        void Tick(float deltaSec)
        {
            if (_guiNode == null) return;

            var damageComponent = _guiNode.damageImageComponent;
            var damageImage = damageComponent.damageImage;

            damageImage.color = Color.Lerp(damageImage.color, Color.clear, damageComponent.flashSpeed * deltaSec);
        }

        void OnDamageEvent(ref PlayerDamageInfo damaged)
        {
            var damageComponent = _guiNode.damageImageComponent;
            var damageImage = damageComponent.damageImage;

            damageImage.color = damageComponent.flashColor;

            _guiNode.healthSliderComponent.healthSlider.value = nodesDB.QueryNode<HUDDamageEventNode>(damaged.entityDamaged).healthComponent.currentHealth;
        }

        void OnPickupHealthEvent(ref HealthPickupData pickupInfo)
        {
            if (pickupInfo.resourceIncrease > 0)
                _guiNode.healthSliderComponent.healthSlider.value = nodesDB.QueryNode<HUDPickupHealthEventNode>(pickupInfo.pickingEntityID).healthComponent.currentHealth;
        }

        void OnPickupAmmoEvent(ref AmmoPickupData pickupInfo)
        {
            if (pickupInfo.resourceIncrease > 0)
                _guiNode.ammoComponent.currentAmmo = nodesDB.QueryNode<HUDPickupAmmoEventNode>(pickupInfo.pickingEntityID).ammoComponent.currentAmmo;
        }

        void OnDeadEvent()
        {
            _guiNode.HUDAnimator.hudAnimator.SetTrigger("GameOver");
        }

        public void Step(ref PlayerDamageInfo token, Enum condition)
        {
            if ((DamageCondition)condition == DamageCondition.damage)
                OnDamageEvent(ref token);
            else
            if ((DamageCondition)condition == DamageCondition.dead)
                OnDeadEvent();
                
        }

        public void Step(ref HealthPickupData token, Enum condition)
        {
            if((PickupState)condition == PickupState.picked)
            {
                OnPickupHealthEvent(ref token);
            }
        }

        public void Step(ref AmmoGunShootingInfo token, Enum condition)
        {
            var ammoNode = nodesDB.QueryNode<HUDPickupAmmoEventNode>(token.gunID);
            _guiNode.ammoComponent.currentAmmo = ammoNode.ammoComponent.currentAmmo;
        }

        public void Step(ref AmmoPickupData token, Enum condition)
        {
            OnPickupAmmoEvent(ref token);
        }

        public void Step(ref PushAbilityCooldownInfo token, Enum condition)
        {
            _guiNode.skillComponent.cooldownPerc = token.cooldownPerc;
        }

        public void Step(ref WaveInfo waveInfo, Enum condition)
        {
            _guiNode.waveComponent.isLastWave = waveInfo.isLastWave;
            _guiNode.waveComponent.wave = waveInfo.wave;
        }

        readonly Type[] _acceptedNodes = { typeof(HUDNode) };

        HUDNode         _guiNode;
    }
}

