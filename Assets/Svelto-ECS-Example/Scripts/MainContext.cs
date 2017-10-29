using Svelto.ECS.Example.Engines.Enemies;
using Svelto.ECS.Example.Engines.Health;
using Svelto.ECS.Example.Engines.HUD;
using Svelto.ECS.Example.Engines.Player;
using Svelto.ECS.Example.Engines.Player.Gun;
using Svelto.ECS.Example.Engines.Sound.Damage;
using Svelto.ECS.Example.Observables.Enemies;
using Svelto.ECS.Example.Observers.HUD;
using Svelto.Context;
using UnityEngine;
using Steps = System.Collections.Generic.Dictionary<Svelto.ECS.IEngine, System.Collections.Generic.Dictionary<System.Enum, Svelto.ECS.IStep[]>>;
using System.Collections.Generic;
using Svelto.ECS.Example.Engines.Player.Pickup;
using Svelto.ECS.Example.Engines.OtherEngines.PickupSpawner;
using Svelto.ECS.Example.Engines.Ammo;
using Svelto.ECS.Example.Engines.Player.Ability;

//Main is the Application Composition Root.
//Composition Root is the place where the framework can be initialised.
namespace Svelto.ECS.Example
{
    public class Main : ICompositionRoot
    {
        public Main()
        {
            SetupEnginesAndComponents();
        }

        void SetupEnginesAndComponents()
        {
            _entityFactory = _enginesRoot = new EnginesRoot();

            GameObjectFactory factory = new GameObjectFactory();

            var enemyKilledObservable = new EnemyKilledObservable();
            var scoreOnEnemyKilledObserver = new ScoreOnEnemyKilledObserver(enemyKilledObservable);

            Sequencer playerDamageSequence = new Sequencer();
            Sequencer enemyDamageSequence = new Sequencer();
            Sequencer enemyWaveSequence = new Sequencer();
            Sequencer healthPickupSequence = new Sequencer();
            Sequencer ammoSequence = new Sequencer();
            Sequencer ammoPickupSequence = new Sequencer();
            Sequencer pushAbilitySequence = new Sequencer();

            var enemyAnimationEngine = new EnemyAnimationEngine();
            var playerHealthEngine = new HealthEngine(playerDamageSequence, healthPickupSequence);
            var enemyHealthEngine = new HealthEngine(enemyDamageSequence);
            var hudEngine = new HUDEngine();
            var damageSoundEngine = new DamageSoundEngine();
            var playerShootingEngine = new PlayerGunShootingEngine(enemyKilledObservable, enemyDamageSequence, ammoSequence);
            var playerMovementEngine = new PlayerMovementEngine();
            var playerAnimationEngine = new PlayerAnimationEngine();
            var enemyAttackEngine = new EnemyAttackEngine(playerDamageSequence);
            var enemyMovementEngine = new EnemyMovementEngine();
            var enemyWavesEngine = new EnemyWavesEngine(factory, _entityFactory, enemyWaveSequence);
            var pickupSpawnerEngine = new PickupSpawnerEngine(factory, _entityFactory);
            var playerPickupEngine = new PlayerPickupEngine(healthPickupSequence, ammoPickupSequence);
            var playerAmmoEngine = new AmmoEngine(ammoPickupSequence);
            var playerPushingAbilityEngine = new PlayerPushingAbilityEngine(pushAbilitySequence);
            var enemyPushBackEngine = new EnemyPushBackEngine(pushAbilitySequence);

            playerDamageSequence.SetSequence(
                new Steps() //sequence of steps
                {
                    { //first step
                        enemyAttackEngine, //this step can be triggered only by this engine through the Next function
                        new Dictionary<System.Enum, IStep[]>() //this step can lead only to one branch
                        {
                            {  Condition.always, new [] { playerHealthEngine }  }, //these engines will be called when the Next function is called with the Condition.always set
                        }
                    },
                    { //second step
                        playerHealthEngine, //this step can be triggered only by this engine through the Next function
                        new Dictionary<System.Enum, IStep[]>() //this step can branch in two paths
                        {
                            {  DamageCondition.damage, new IStep[] { hudEngine, damageSoundEngine }  }, //these engines will be called when the Next function is called with the DamageCondition.damage set
                            {  DamageCondition.dead, new IStep[] { hudEngine, damageSoundEngine, playerMovementEngine, playerAnimationEngine, enemyAnimationEngine }  }, //these engines will be called when the Next function is called with the DamageCondition.dead set
                        }
                    }
                }
            );

            enemyDamageSequence.SetSequence(
                new Steps()
                {
                    {
                        playerShootingEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            {  Condition.always, new IStep[] { enemyHealthEngine }  }
                        }
                    },
                    {
                        enemyHealthEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            {  DamageCondition.damage, new IStep[] { enemyAnimationEngine }  },
                            {  DamageCondition.dead, new IStep[] { enemyMovementEngine, enemyAnimationEngine, playerShootingEngine, enemyWavesEngine, enemyPushBackEngine }  },
                        }
                    }
                }
            );

            enemyWaveSequence.SetSequence(
                new Steps()
                {
                    {
                        enemyWavesEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            { Condition.always, new [] { hudEngine } },
                        }
                    }
                }
                );

            healthPickupSequence.SetSequence(
                new Steps()
                {
                    {
                        playerPickupEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            {  PickupState.picking, new [] { playerHealthEngine } },
                        }
                    },
                    {
                        playerHealthEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            {  PickupState.picked, new IStep[] { hudEngine, pickupSpawnerEngine } }
                        }
                    }
                }
                );

            ammoSequence.SetSequence(
                new Steps()
                {
                    {
                        playerShootingEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            { Condition.always, new IStep[] { playerAmmoEngine, hudEngine } }
                        }
                    }
                }
                );

            ammoPickupSequence.SetSequence(
                new Steps()
                {
                    {
                        playerPickupEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            {  PickupState.picking, new [] { playerAmmoEngine } },
                        }
                    },
                    {
                        playerAmmoEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            {  PickupState.picked, new IStep[] { hudEngine, pickupSpawnerEngine } }
                        }
                    }
                }
                );

            pushAbilitySequence.SetSequence(
               new Steps()
               {
                    {
                        playerPushingAbilityEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            { PushAbilityState.pushing, new IStep[] { enemyMovementEngine, enemyPushBackEngine } },
                            { Condition.always, new [] { hudEngine } }
                        }
                    },
                    {
                        enemyPushBackEngine,
                        new Dictionary<System.Enum, IStep[]>()
                        {
                            {  PushAbilityState.unstunning, new IStep[] { enemyMovementEngine } }
                        }
                    }
               }
               );
            
            AddEngine(playerMovementEngine);
            AddEngine(playerAnimationEngine);
            AddEngine(playerShootingEngine);
            AddEngine(playerHealthEngine);
            AddEngine(new PlayerGunShootingFXsEngine());

            AddEngine(new EnemySpawnerEngine(factory, _entityFactory));
            AddEngine(enemyAttackEngine);
            AddEngine(enemyMovementEngine);
            AddEngine(enemyAnimationEngine);
            AddEngine(enemyHealthEngine);

            AddEngine(enemyWavesEngine);
            AddEngine(playerPickupEngine);
            AddEngine(pickupSpawnerEngine);
            AddEngine(playerAmmoEngine);
            AddEngine(playerPushingAbilityEngine);
            AddEngine(enemyPushBackEngine);

            AddEngine(damageSoundEngine);
            AddEngine(hudEngine);
            AddEngine(new ScoreEngine(scoreOnEnemyKilledObserver));
        }

        void AddEngine(IEngine engine)
        {
            _enginesRoot.AddEngine(engine);
        }

        void ICompositionRoot.OnContextCreated(UnityContext contextHolder)
        {
            IEntityDescriptorHolder[] entities = contextHolder.GetComponentsInChildren<IEntityDescriptorHolder>();

            for (int i = 0; i < entities.Length; i++)
                _entityFactory.BuildEntity((entities[i] as MonoBehaviour).gameObject.GetInstanceID(), entities[i].BuildDescriptorType());
        }

        void ICompositionRoot.OnContextInitialized()
        { }

        void ICompositionRoot.OnContextDestroyed()
        { }

        EnginesRoot _enginesRoot;
        IEntityFactory _entityFactory;

    }

    //A GameObject containing UnityContext must be present in the scene
    //All the monobehaviours present in the scene statically that need
    //to notify the Context, must belong to GameObjects children of UnityContext.

    public class MainContext : UnityContext<Main>
    { }

}