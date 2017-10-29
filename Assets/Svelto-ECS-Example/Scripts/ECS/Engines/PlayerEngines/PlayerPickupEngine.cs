using System;
using Svelto.ECS.Example.Nodes.HUD;
using Svelto.ECS.Example.Observers.HUD;
using Svelto.ECS.Example.Nodes.Pickup;
using Svelto.Tasks;
using Svelto.ECS.Example.Components.Pickup;
using Svelto.ECS.Example.Nodes.Player;
using Svelto.ECS.Example.Nodes.Gun;

namespace Svelto.ECS.Example.Engines.Player.Pickup
{
    // Logic used to manage the picking up of the different pickups - ammo and health pickup.
    public class PlayerPickupEngine : INodesEngine, IQueryableNodeEngine
    {
        private Sequencer _healthPickupSequence;
        private Sequencer _ammoPickupSequence;

        public IEngineNodeDB nodesDB { set; private get; }

        public PlayerPickupEngine(Sequencer healthPickupSequence, Sequencer ammoPickupSequence)
        {
            _healthPickupSequence = healthPickupSequence;
            _ammoPickupSequence = ammoPickupSequence;
        }

        public Type[] AcceptedNodes() { return _acceptedNodes; }

        public void Add(INode obj)
        {
            if (obj is PickupNode)
            {
                var pickupNode = obj as PickupNode;
                pickupNode.pickupComponent.entityInRange += CheckPlayerInRange;
            }
            else
            {
                if (obj is PlayerNode)
                    _playerNode = obj as PlayerNode;
                else
                    _playerGunNode = obj as GunNode;
            }
        }
        public void Remove(INode obj)
        {
            if (obj is PickupNode)
            {
                var pickupNode = obj as PickupNode;
                pickupNode.pickupComponent.entityInRange -= CheckPlayerInRange;
            }
            else
            {
                if (obj is PlayerNode)
                    _playerNode = null;
                else
                    _playerGunNode = null;
            }
        }

        void CheckPlayerInRange(int playerID, int pickupID, bool inRange)
        {
            if (_playerNode == null)
                return;

            if (playerID == _playerNode.ID)
            {
                if (inRange)
                {
                    var pickupNode = nodesDB.QueryNode<PickupNode>(pickupID);
                    
                    switch(pickupNode.pickupTypeComponent.pickupType)
                    {
                        case (PickupType.health):
                            HealthPickupData healthData = new HealthPickupData(pickupID, playerID, pickupNode.resourceComponent.resourceIncrease, pickupNode.resourceComponent.maxResource);
                            _healthPickupSequence.Next(this, ref healthData, PickupState.picking);
                            break;
                        case (PickupType.ammo):
                            AmmoPickupData ammoData = new AmmoPickupData(pickupID, _playerGunNode.ID, pickupNode.resourceComponent.resourceIncrease, pickupNode.resourceComponent.maxResource);
                            _ammoPickupSequence.Next(this, ref ammoData, PickupState.picking);
                            break;
                    }
                                        
                    DestroyEntity(pickupNode);
                }
            }
        }

        void DestroyEntity(PickupNode node)
        {
            node.removeEntityComponent.removeEntity();
            UnityEngine.GameObject.Destroy(node.transformComponent.transform.gameObject);
        }

        readonly Type[] _acceptedNodes = { typeof(PickupNode), typeof(GunNode), typeof(PlayerNode) };

        PlayerNode _playerNode;
        GunNode _playerGunNode;
    }
}


