using Svelto.ECS.Example.Nodes.Enemies;
using Svelto.DataStructures;
using System;
using UnityEngine;
using System.Collections;
using Svelto.ECS.Example.Nodes.PickupSpawner;
using Svelto.ECS.Example.Components.Pickup;
using System.Collections.Generic;

namespace Svelto.ECS.Example.Engines.OtherEngines.PickupSpawner
{
    class PickupSpawnerEngine : INodesEngine, IStep<HealthPickupData>, IStep<AmmoPickupData>
    {
        internal class PickupSpawnerData
        {
            internal int timeLeft;
            internal GameObject pickupPrefab;
            internal int maxCount;
            internal FasterList<Transform> spawnPoints;
            internal int spawnTime;
            internal Dictionary<int, Transform> entitiesSpawned;

            internal PickupSpawnerData(IPickupControlledComponent spawnerComponent)
            {
                pickupPrefab = spawnerComponent.pickupPrefab;
                spawnTime = spawnerComponent.spawnTime;
                spawnPoints = new FasterList<Transform>();
                spawnPoints.AddRange(spawnerComponent.spawnPositions, spawnerComponent.spawnPositions.Length);
                timeLeft = spawnTime;
                maxCount = spawnerComponent.maxCount;
                entitiesSpawned = new Dictionary<int, Transform>();
            }
        }

        public PickupSpawnerEngine(Factories.IGameObjectFactory factory, IEntityFactory entityFactory)
        {
            _factory = factory;
            _entityFactory = entityFactory;
            TaskRunner.Instance.Run(IntervaledTick);
        }

        public Type[] AcceptedNodes()
        {
            return _acceptedNodes;
        }

        public void Add(INode obj)
        {
            var pickupsToSpawn = (obj as PickupManagerNode).spawnerComponent;

            for (int i = 0; i < pickupsToSpawn.pickupsToSpawn.Length; i++)
                _pickupsToSpawn.Add(new PickupSpawnerData(pickupsToSpawn.pickupsToSpawn[i]));
        }

        public void Remove(INode obj)
        {
            //remove is called on context destroyed, in this case the entire engine will be destroyed
        }

        IEnumerator IntervaledTick()
        {
            while (true)
            {
                yield return _waitForSecondsEnumerator;

                for (int i = _pickupsToSpawn.Count - 1; i >= 0; --i)
                {
                    var spawnData = _pickupsToSpawn[i];
                    if (spawnData.entitiesSpawned.Count >= spawnData.maxCount || spawnData.spawnPoints.Count == 0) continue;

                    if (spawnData.timeLeft <= 0.0f)
                    {
                        // Find a random index between zero and one less than the number of spawn points.
                        int spawnPointIndex = UnityEngine.Random.Range(0, spawnData.spawnPoints.Count);

                        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
                        var go = _factory.Build(spawnData.pickupPrefab);
                        _entityFactory.BuildEntity(go.GetInstanceID(), go.GetComponent<IEntityDescriptorHolder>().BuildDescriptorType());
                        int id = go.GetInstanceID();
                        
                        var transform = go.transform;
                        var spawnInfo = spawnData.spawnPoints[spawnPointIndex];

                        spawnData.entitiesSpawned.Add(go.GetInstanceID(), spawnData.spawnPoints[spawnPointIndex]);
                        spawnData.spawnPoints.RemoveAt(spawnPointIndex);

                        transform.position = spawnInfo.position;
                        transform.rotation = spawnInfo.rotation;

                        spawnData.timeLeft = spawnData.spawnTime;
                    }

                    spawnData.timeLeft -= 1;
                }
            }
        }

        public void Step(ref HealthPickupData token, Enum condition)
        {
            if ((PickupState)condition == PickupState.picked)
            {
                for (int i = 0; i < _pickupsToSpawn.Count; i++)
                    if (_pickupsToSpawn[i].entitiesSpawned.ContainsKey(token.pickupID))
                    {
                        _pickupsToSpawn[i].spawnPoints.Add(_pickupsToSpawn[i].entitiesSpawned[token.pickupID]);
                        _pickupsToSpawn[i].entitiesSpawned.Remove(token.pickupID);
                        return;
                    }
            }
        }

        public void Step(ref AmmoPickupData token, Enum condition)
        {
            if ((PickupState)condition == PickupState.picked)
            {
                for (int i = 0; i < _pickupsToSpawn.Count; i++)
                    if (_pickupsToSpawn[i].entitiesSpawned.ContainsKey(token.pickupID))
                    {
                        _pickupsToSpawn[i].spawnPoints.Add(_pickupsToSpawn[i].entitiesSpawned[token.pickupID]);
                        _pickupsToSpawn[i].entitiesSpawned.Remove(token.pickupID);
                        return;
                    }
            }
        }

        readonly Type[] _acceptedNodes = { typeof(PickupManagerNode) };
        FasterList<PickupSpawnerData> _pickupsToSpawn = new FasterList<PickupSpawnerData>();
        Svelto.Factories.IGameObjectFactory _factory;
        IEntityFactory _entityFactory;
        Tasks.WaitForSecondsEnumerator _waitForSecondsEnumerator = new Tasks.WaitForSecondsEnumerator(1);
    }
}
