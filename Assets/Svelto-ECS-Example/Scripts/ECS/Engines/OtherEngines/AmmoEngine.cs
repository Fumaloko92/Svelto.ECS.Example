using System;
using Svelto.ECS.Example.Components.Ammo;
using Svelto.ECS.Example.Nodes.Gun;
using Svelto.ECS.Example.Components.Pickup;

namespace Svelto.ECS.Example.Engines.Ammo
{
    // Class that implements the logic of the ammo. This is a general engine since it could be also used from enemies at some point(as for the health)
    public class AmmoEngine : IEngine, IQueryableNodeEngine, IStep<AmmoGunShootingInfo>, IStep<AmmoPickupData>
    {
        Sequencer _ammoPickupSequence;

        public AmmoEngine(Sequencer ammoPickupSequence)
        {
            _ammoPickupSequence = ammoPickupSequence;
        }

        public IEngineNodeDB nodesDB { set; private get; }

        public void Step(ref AmmoPickupData token, Enum condition)
        {
            if((PickupState)condition == PickupState.picking)
            {
                GunNode pickingGunNode = nodesDB.QueryNode<GunNode>(token.pickingEntityID);

                pickingGunNode.ammoComponent.currentAmmo = Math.Min(pickingGunNode.ammoComponent.currentAmmo + token.resourceIncrease, token.maxResource);
                _ammoPickupSequence.Next(this, ref token, PickupState.picked);
            }
        }

        public void Step(ref AmmoGunShootingInfo token, Enum condition)
        {
            UseAmmo(ref token);
        }

        private void UseAmmo(ref AmmoGunShootingInfo ammoGunInfo)
        {
            GunNode shootingGunNode = nodesDB.QueryNode<GunNode>(ammoGunInfo.gunID);
            shootingGunNode.ammoComponent.currentAmmo -= ammoGunInfo.ammoShot;
        }
    }
}
