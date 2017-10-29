using System;
using Svelto.ECS.Example.Components.Base;
using Svelto.ECS.Example.Components.Damageable;
using UnityEngine;
using Svelto.ECS.Example.Components.Pickup;

namespace Svelto.ECS.Example.Implementers.Player.Pickup
{
    public class HealthPickup : MonoBehaviour, IPickupComponent, IPickupTypeComponent, IPickupResourceComponent, IRemoveEntityComponent, ITransformComponent
    {
        public int _healthIncrease;

        public int _maxHealth;

        public PickupType _pickupType;

        Action IRemoveEntityComponent.removeEntity { get; set; }

        int IPickupResourceComponent.resourceIncrease { get { return _healthIncrease; } }
        int IPickupResourceComponent.maxResource { get { return _maxHealth; } }
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
