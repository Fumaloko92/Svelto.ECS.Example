using System;
using Svelto.ECS.Example.Components.Base;
using Svelto.ECS.Example.Components.Damageable;
using UnityEngine;
using Svelto.ECS.Example.Components.Pickup;

namespace Svelto.ECS.Example.Implementers.Player.Pickup
{
    // The implementers of the Ammo pickup logic
    public class AmmoPickup : MonoBehaviour, IPickupComponent, IPickupResourceComponent, IPickupTypeComponent, IRemoveEntityComponent, ITransformComponent
    {
        public int _ammoIncrease;

        public int _maxAmmo;

        public PickupType _pickupType;

        Action IRemoveEntityComponent.removeEntity { get; set; }

        int IPickupResourceComponent.resourceIncrease { get { return _ammoIncrease; } }
        int IPickupResourceComponent.maxResource { get { return _maxAmmo; } }
        PickupType IPickupTypeComponent.pickupType { get { return _pickupType; } }

        public event Action<int, int, bool> entityInRange;

        void OnTriggerEnter(Collider other)
        {
            if (entityInRange != null)
            {
                entityInRange(other.gameObject.GetInstanceID(), gameObject.GetInstanceID(), true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (entityInRange != null)
                entityInRange(other.gameObject.GetInstanceID(), gameObject.GetInstanceID(), false);
        }
    }
}
