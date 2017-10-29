using UnityEngine;
using Svelto.ECS.Example.Nodes.Pickup;
using Svelto.ECS.Example.Components.Pickup;
using Svelto.DataStructures;
using System;

namespace Svelto.ECS.Example.EntityDescriptors.Player.Pickup
{
    public class PickupEntityDescriptor : EntityDescriptor
    {
        static readonly INodeBuilder[] _nodesToBuild;

        static PickupEntityDescriptor()
        {
            _nodesToBuild = new INodeBuilder[]
            {
                new NodeBuilder<PickupNode>(),
            };
        }

        public PickupEntityDescriptor(IComponent[] componentsImplementor) : base(_nodesToBuild, componentsImplementor)
        { }
    }

    [DisallowMultipleComponent]
    public class PickupEntityDescriptorHolder : MonoBehaviour, IEntityDescriptorHolder
    {
        public EntityDescriptor BuildDescriptorType(object[] extraImplentors = null)
        {
            return new PickupEntityDescriptor(GetComponentsInChildren<IComponent>());
        }
    }
}
