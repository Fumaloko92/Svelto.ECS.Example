using System;
using UnityEngine;

// Namespace that contains all the components that contribute to the Pickup logic
namespace Svelto.ECS.Example.Components.Pickup
{
    // Component used to spawn a defined set of pickup with controlled settings
    public interface IPickupSpawnerComponent: IComponent
    {
        IPickupControlledComponent[] pickupsToSpawn { get; }
    }

    // Component used to spawn a pickup in a controllable - not random, way
    public interface IPickupControlledComponent: IComponent
    {
        GameObject pickupPrefab { get; }
        Transform[] spawnPositions { get; }
        int maxCount { get; }
        int spawnTime { get; }
    }

    // General component for a pickup. A pickup should always increase(decrease) a resource up to a certain amount
    public interface IPickupResourceComponent: IComponent
    {
        int resourceIncrease { get; }
        int maxResource { get; }
    }

    // This component is used to trigger the event connected to the picking of a pickup
    public interface IPickupComponent: IComponent
    {
        event System.Action<int, int, bool> entityInRange;
    }

    // This component forces the implementer to specify the type of pickup that it is connected to
    public interface IPickupTypeComponent: IComponent
    {
        PickupType pickupType { get; }
    }

    public enum PickupType
    {
        health,
        ammo
    }

    // The structs below wrap up the information about a pickup allowing the sharing of the information and
    // preventing its change at the same time.

    public struct HealthPickupData: IPickupResourceComponent
    {
        private int _pickingEntityID;
        private int _pickupID;
        private int _healthIncrease;
        private int _maxHealth;
        public int resourceIncrease { get { return _healthIncrease; } }
        public int pickingEntityID { get { return _pickingEntityID; } }
        public int pickupID { get { return _pickupID; } }
        public int maxResource { get { return _maxHealth; } }
        public HealthPickupData(int pickupID, int pickingEntityID, int healthIncrease, int maxHealth)
        {
            _pickupID = pickupID;
            _healthIncrease = healthIncrease;
            _pickingEntityID = pickingEntityID;
            _maxHealth = maxHealth;
        }
    }

    public struct AmmoPickupData: IPickupResourceComponent
    {
        private int _pickingEntityID;
        private int _pickupID;
        private int _ammoIncrease;
        private int _maxAmmo;
        public int resourceIncrease { get { return _ammoIncrease; } }
        public int pickingEntityID { get { return _pickingEntityID; } }
        public int pickupID { get { return _pickupID; } }
        public int maxResource { get { return _maxAmmo; } }
        public AmmoPickupData(int pickupID, int pickingEntityID, int ammoIncrease, int maxAmmo)
        {
            _pickupID = pickupID;
            _ammoIncrease = ammoIncrease;
            _pickingEntityID = pickingEntityID;
            _maxAmmo = maxAmmo;
        }
    }
}

