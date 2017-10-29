using System;
using UnityEngine;

namespace Svelto.ECS.Example.Components.Ammo
{
    // Component to hold information through engines about the shooting gun and the number of projectiles shot
    public interface IAmmoGunShootingComponent: IComponent
    {
        int ammoShot { get; }
        int gunID { get; }
    }

    public struct AmmoGunShootingInfo: IAmmoGunShootingComponent
    {
        private int _ammoShot;
        private int _gunID;

        public int ammoShot { get { return _ammoShot; } }
        public int gunID { get { return _gunID; } }

        public AmmoGunShootingInfo(int gunID, int ammoShot)
        {
            _gunID = gunID;
            _ammoShot = ammoShot;
        }
    }
}

