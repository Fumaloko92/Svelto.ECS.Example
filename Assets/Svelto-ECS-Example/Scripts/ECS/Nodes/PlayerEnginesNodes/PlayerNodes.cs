using Svelto.ECS.Example.Components.Base;
using Svelto.ECS.Example.Components.Damageable;
using Svelto.ECS.Example.Components.Gun;
using Svelto.ECS.Example.Components.HUD;
using Svelto.ECS.Example.Components.Player;
using Svelto.ECS.Example.Components.PlayerAbilities;

namespace Svelto.ECS.Example.Nodes.Player
{
    public class PlayerNode : NodeWithID
    {
        public IHealthComponent        healthComponent;
        public ISpeedComponent         speedComponent;
        public IRigidBodyComponent     rigidBodyComponent;
        public IPositionComponent      positionComponent;
        public IAnimationComponent     animationComponent;
    }

    public class PlayerTargetNode : NodeWithID
    {
        public IHealthComponent         healthComponent;
        public ITargetTypeComponent     targetTypeComponent;
    }
}

namespace Svelto.ECS.Example.Nodes.Gun
{
    public class GunNode : NodeWithID
    {
        public IGunAttributesComponent   gunComponent;
        public IGunFXComponent           gunFXComponent;
        public IGunHitTargetComponent    gunHitTargetComponent;
        public IAmmoComponent            ammoComponent;
    }
}

namespace Svelto.ECS.Example.Nodes.PlayerAbilities
{
    public class PushAbilityNode : NodeWithID
    {
        public ITransformComponent          transformComponent;
        public IAbilityInputNameComponent   pushAbilityInputNameComponent;
        public IPushAbilityComponent        pushAbilityComponent;
    }
}
