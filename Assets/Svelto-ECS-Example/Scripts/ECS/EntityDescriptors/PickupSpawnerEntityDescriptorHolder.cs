using UnityEngine;
using Svelto.ECS.Example.Nodes.Enemies;
using Svelto.DataStructures;
using System;
using Svelto.ECS.Example.Components.Enemy;
using Svelto.ECS.Example.Components.Pickup;
using Svelto.ECS.Example.Nodes.PickupSpawner;

namespace Svelto.ECS.Example.EntityDescriptors.EnemySpawner
{
    class PickupSpawnerEntityDescriptor : EntityDescriptor
    {
        IPickupSpawnerComponent _component;

        public PickupSpawnerEntityDescriptor(IPickupSpawnerComponent componentsImplementor) : base(null, componentsImplementor)
        {
            _component = componentsImplementor;
        }

        //this shows how you can override the BuildNodes to adapt it to your needs. Without calling the base
        //function, the automatic component injection will be disabled
        public override FasterList<INode> BuildNodes(int ID, Action<INode> removeAction)
        {
            var nodes = new FasterList<INode>();
            var node = new PickupManagerNode
            {
                spawnerComponent = _component
            };

            nodes.Add(node);
            return nodes;
        }
    }

    [DisallowMultipleComponent]
    public class PickupSpawnerEntityDescriptorHolder : MonoBehaviour, IEntityDescriptorHolder
    {
        public EntityDescriptor BuildDescriptorType(object[] extraImplentors = null)
        {
            return new PickupSpawnerEntityDescriptor(GetComponent<IPickupSpawnerComponent>());
        }
    }
}

