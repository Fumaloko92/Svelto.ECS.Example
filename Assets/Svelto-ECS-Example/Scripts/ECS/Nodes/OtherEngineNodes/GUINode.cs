using Svelto.ECS.Example.Components.Ammo;
using Svelto.ECS.Example.Components.HUD;
using Svelto.ECS.Example.Components.PlayerAbilities;

namespace Svelto.ECS.Example.Nodes.HUD
{
    public class HUDNode: NodeWithID
    {
        public IAnimatorHUDComponent    HUDAnimator;
        public IDamageHUDComponent      damageImageComponent;
        public IHealthSliderComponent   healthSliderComponent;
        public IScoreComponent          scoreComponent;
        public IWaveComponent           waveComponent;
        public IAmmoComponent           ammoComponent;
        public ISkillCooldownComponent  skillComponent;
    }
}
