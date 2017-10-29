using System;
using Svelto.ECS.Example.Components.Enemy;
using UnityEngine;

namespace Svelto.ECS.Example.Implementers.Enemies
{
    // Class accessed from the inspector that allows a "direct" access to the wave management through the use of the below serializable classes
    public class EnemyWaves : MonoBehaviour, IEnemyWavesAttackComponent
    {
        public EnemyWave[] waves;
        public float timeBetweenWaves;
        public bool spawnWaveOnlyWhenPreviousOneDied;

        IEnemyWaveComponent[] IEnemyWavesAttackComponent.waves { get { return waves; } }
        float IEnemyWavesAttackComponent.timeBetweenWaves { get { return timeBetweenWaves; } }

        bool IEnemyWavesAttackComponent.spawnWaveOnlyWhenPreviousOneDied { get { return spawnWaveOnlyWhenPreviousOneDied; } }
    }
    [Serializable]
    public class EnemyWave : IEnemyWaveComponent
    {
        public ControlledEnemySpawner[] spawners;

        IEnemyControlledSpawnerComponent[] IEnemyWaveComponent.spawners { get { return spawners; } }
    }

    [Serializable]
    public class ControlledEnemySpawner : IEnemyControlledSpawnerComponent
    {
        public GameObject enemy;                // The enemy prefab to be spawned.
        public float spawnTime = 3f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
        public int numberToSpawn;

        GameObject IEnemyControlledSpawnerComponent.enemyPrefab { get { return enemy; } }
        float IEnemyControlledSpawnerComponent.spawnTime { get { return spawnTime; } }
        Transform[] IEnemyControlledSpawnerComponent.spawnPoints { get { return spawnPoints; } }
        int IEnemyControlledSpawnerComponent.numberToSpawn { get { return numberToSpawn; } }
    }
}
