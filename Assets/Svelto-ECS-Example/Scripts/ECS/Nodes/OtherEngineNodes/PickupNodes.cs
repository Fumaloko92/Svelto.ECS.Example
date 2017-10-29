using Svelto.ECS.Example.Components.Base;
using Svelto.ECS.Example.Components.Pickup;

namespace Svelto.ECS.Example.Nodes.Pickup
{
    public class PickupNode : NodeWithID
    {
        public ITransformComponent transformComponent;
        public IPickupComponent pickupComponent;
        public IPickupTypeComponent pickupTypeComponent;
        public IPickupResourceComponent resourceComponent;

        public IRemoveEntityComponent removeEntityComponent;
    }
}

namespace Svelto.ECS.Example.Nodes.PickupSpawner
{
    public class PickupSpawnerNode : NodeWithID
    {
        public IPickupControlledComponent[] pickupsToSpawnComponent;
    }

    public class PickupManagerNode : NodeWithID
    {
        public IPickupSpawnerComponent spawnerComponent;
    }
}