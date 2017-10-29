using Svelto.ECS.Example.Components.HUD;
using Svelto.ECS.Example.Components.Damageable;

namespace Svelto.ECS.Example.Nodes.HUD
{
    public class HUDPickupHealthEventNode : NodeWithID
    {
        public IHealthComponent healthComponent;
    }

    public class HUDPickupAmmoEventNode : NodeWithID
    {
        public IAmmoComponent ammoComponent;
    }
}
