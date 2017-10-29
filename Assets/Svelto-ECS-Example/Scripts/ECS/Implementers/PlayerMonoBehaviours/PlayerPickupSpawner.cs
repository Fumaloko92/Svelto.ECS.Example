using System;
using Svelto.ECS.Example.Components.Base;
using Svelto.ECS.Example.Components.Damageable;
using UnityEngine;
using Svelto.ECS.Example.Components.Pickup;

namespace Svelto.ECS.Example.Implementers.Player.Pickup
{
    // The implementer - similar to the waves implementer, to spawn the pickups on the map in a controlled way
    public class PlayerPickupSpawner : MonoBehaviour, IPickupSpawnerComponent
    {
        public PickupControlled[] _pickupsToSpawn;
        IPickupControlledComponent[] IPickupSpawnerComponent.pickupsToSpawn { get { return _pickupsToSpawn; } }
    }

    [Serializable]
    public class PickupControlled: IPickupControlledComponent
    {
        public GameObject _pickupPrefab;

        public Transform[] _spawnPositions;

        public int _maxCount;

        public int _spawnTime;

        GameObject IPickupControlledComponent.pickupPrefab { get { return _pickupPrefab; } }
        Transform[] IPickupControlledComponent.spawnPositions { get { return _spawnPositions; } }
        int IPickupControlledComponent.maxCount { get { return _maxCount; } }
        int IPickupControlledComponent.spawnTime { get { return _spawnTime; } }
    }
}
