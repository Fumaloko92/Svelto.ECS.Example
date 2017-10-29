using Svelto.ECS.Example.Components.Enemy;
using Svelto.ECS.Example.Nodes.Enemies;
using Svelto.DataStructures;
using System;
using UnityEngine;
using System.Collections;
using Svelto.ECS.Example.Components.HUD;
using Svelto.ECS.Example.Components.Damageable;

namespace Svelto.ECS.Example.Engines.Enemies
{
    // Class that holds the logic for implementing the spawning of the enemies with a wave system
    public class EnemyWavesEngine : INodesEngine, IStep<DamageInfo>
    {
        internal class EnemyWaveData
        {
            internal FasterList<IEnemyWaveComponent> waves;
            internal int currentWave;
            internal float timeLeftBetweenWaves;
            internal float timeBetweenWaves;
            internal bool spawnWaveOnlyWhenPreviousOneDied;

            internal EnemyWaveData(IEnemyWavesAttackComponent wavesAttack)
            {
                waves = new FasterList<IEnemyWaveComponent>();
                waves.AddRange(wavesAttack.waves, wavesAttack.waves.Length);
                timeBetweenWaves = wavesAttack.timeBetweenWaves;
                timeLeftBetweenWaves = timeBetweenWaves;
                currentWave = 0;
                spawnWaveOnlyWhenPreviousOneDied = wavesAttack.spawnWaveOnlyWhenPreviousOneDied;
            }
        }

        internal class EnemyControlledSpawnerData
        {
            internal float timeLeft;
            internal float leftToSpawn;
            internal GameObject enemy;
            internal float spawnTime;
            internal Transform[] spawnPoints;

            internal EnemyControlledSpawnerData(IEnemyControlledSpawnerComponent spawnerComponent)
            {
                enemy = spawnerComponent.enemyPrefab;
                spawnTime = spawnerComponent.spawnTime;
                spawnPoints = spawnerComponent.spawnPoints;
                timeLeft = spawnTime;
                leftToSpawn = spawnerComponent.numberToSpawn;
            }
        }

        private Sequencer _hudChange;

        public EnemyWavesEngine(Factories.IGameObjectFactory factory, IEntityFactory entityFactory, Sequencer hudChange)
        {
            _factory = factory;
            _entityFactory = entityFactory;
            _hudChange = hudChange;
            TaskRunner.Instance.Run(new Tasks.TimedLoopActionEnumerator(Tick));
        }

        public Type[] AcceptedNodes()
        {
            return _acceptedNodes;
        }

        public void Add(INode obj)
        {
            var spawnerComponent = (obj as EnemyWavesAttackNode).waveComponent;

            _enemyWave = new EnemyWaveData(spawnerComponent);
            index = 0;
            _aliveEnemies = 0;
            newWave = true;
        }

        public void Remove(INode obj)
        {
            //remove is called on context destroyed, in this case the entire engine will be destroyed
        }

        void Tick(float deltaSec)
        {
            IEnemyWaveComponent wave;

            if (_enemyWave == null || index >= _enemyWave.waves.Count) return;

            if (newWave)
            {
                if (_enemyWave.timeLeftBetweenWaves <= 0 &&
                    (!_enemyWave.spawnWaveOnlyWhenPreviousOneDied || (_aliveEnemies == 0 && _enemyWave.spawnWaveOnlyWhenPreviousOneDied)))
                {
                    HUDSequence();
                    wave = _enemyWave.waves[index];
                    _enemiestoSpawn.Clear();
                    for (int i = 0; i < wave.spawners.Length; i++)
                        _enemiestoSpawn.Add(new EnemyControlledSpawnerData(wave.spawners[i]));
                    newWave = false;
                    _enemyWave.timeLeftBetweenWaves = _enemyWave.timeBetweenWaves;
                }
                _enemyWave.timeLeftBetweenWaves -= deltaSec;
            }

            if (!newWave)
            {
                newWave = true;
                for (int i = 0; i < _enemiestoSpawn.Count; i++)
                {
                    if (_enemiestoSpawn[i].leftToSpawn > 0 && _enemiestoSpawn[i].timeLeft <= 0)
                    {
                        // Find a random index between zero and one less than the number of spawn points.
                        int spawnPointIndex = UnityEngine.Random.Range(0, _enemiestoSpawn[i].spawnPoints.Length);

                        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
                        var go = _factory.Build(_enemiestoSpawn[i].enemy);
                        _entityFactory.BuildEntity(go.GetInstanceID(), go.GetComponent<IEntityDescriptorHolder>().BuildDescriptorType());
                        var transform = go.transform;
                        var spawnInfo = _enemiestoSpawn[i].spawnPoints[spawnPointIndex];

                        transform.position = spawnInfo.position;
                        transform.rotation = spawnInfo.rotation;

                        _enemiestoSpawn[i].timeLeft = _enemiestoSpawn[i].spawnTime;
                        _enemiestoSpawn[i].leftToSpawn--;
                        _aliveEnemies++;
                    }
                    if (_enemiestoSpawn[i].leftToSpawn > 0)
                        newWave = false;
                    _enemiestoSpawn[i].timeLeft -= deltaSec;
                }

                if (newWave)
                    index++;
            }
        }

        public void HUDSequence()
        {
            WaveInfo info;
            if (index + 1 == _enemyWave.waves.Count)
                info = new WaveInfo(index + 1, true);
            else
                info = new WaveInfo(index + 1, false);
            _hudChange.Next(this, ref info, Condition.always);
        }

        public void Step(ref DamageInfo token, Enum condition)
        {
            if ((DamageCondition)condition == DamageCondition.dead)
                _aliveEnemies--;
        }

        readonly Type[] _acceptedNodes = { typeof(EnemyWavesAttackNode) };
        EnemyWaveData _enemyWave;
        FasterList<EnemyControlledSpawnerData> _enemiestoSpawn = new FasterList<EnemyControlledSpawnerData>();
        int index = 0;
        bool newWave = true;
        int _aliveEnemies;
        Svelto.Factories.IGameObjectFactory _factory;
        IEntityFactory _entityFactory;
    }
}
