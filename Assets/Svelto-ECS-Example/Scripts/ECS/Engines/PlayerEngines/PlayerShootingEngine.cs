using System;
using UnityEngine;
using Svelto.ECS.Example.Nodes.Player;
using Svelto.ECS.Example.Components.Damageable;
using Svelto.ECS.Example.Observables.Enemies;
using Svelto.ECS.Example.Nodes.Gun;
using Svelto.ECS.Example.Components.Ammo;

namespace Svelto.ECS.Example.Engines.Player.Gun
{
    public class PlayerGunShootingEngine : INodesEngine, IQueryableNodeEngine, IStep<DamageInfo>
    {
        public IEngineNodeDB nodesDB { set; private get; }

        public PlayerGunShootingEngine(EnemyKilledObservable enemyKilledObservable, Sequencer damageSequence, Sequencer ammoSequence)
        {
            _enemyKilledObservable = enemyKilledObservable;
            _enemyDamageSequence = damageSequence;
            _ammoSequence = ammoSequence;

            TaskRunner.Instance.Run(new Tasks.TimedLoopActionEnumerator(Tick));
        }

        public Type[] AcceptedNodes() { return _acceptedNodes; }

        public void Add(INode obj)
        {
            if (obj is GunNode)
            {
                _playerGunNode = obj as GunNode;
                AmmoGunShootingInfo ammoHUDUpdate = new AmmoGunShootingInfo(_playerGunNode.ID, 0);
                _ammoSequence.Next(this, ref ammoHUDUpdate);
            }
        }

        public void Remove(INode obj)
        {
            if (obj is PlayerNode) //the gun is never removed (because the level reloads on death), so remove on playerdeath
                _playerGunNode = null;
        }

        private void OnPlayerDead(int ID, bool isDead)
        {
            _playerGunNode = null;
        }

        void Tick(float deltaSec)
        {
            if (_playerGunNode == null) return;

            var playerGunComponent = _playerGunNode.gunComponent;

            playerGunComponent.timer += deltaSec;

            if (Input.GetButton("Fire1") && playerGunComponent.timer >= _playerGunNode.gunComponent.timeBetweenBullets && Time.timeScale != 0
                && _playerGunNode.ammoComponent.currentAmmo > 0)
                Shoot();
        }

        void Shoot()
        {
            RaycastHit shootHit;
            var playerGunComponent = _playerGunNode.gunComponent;
            var playerGunHitComponent = _playerGunNode.gunHitTargetComponent;

            playerGunComponent.timer = 0;

            AmmoGunShootingInfo ammoShootingInfo = new AmmoGunShootingInfo(_playerGunNode.ID, _playerGunNode.gunComponent.numberOfBulletsFired);
            _ammoSequence.Next(this, ref ammoShootingInfo);
            bool hitSomething = false;
            for (int i = 0; i < _playerGunNode.gunComponent.numberOfBulletsFired; i++)
            {
                if (Physics.Raycast(playerGunComponent.shootRay, out shootHit, playerGunComponent.range, SHOOTABLE_MASK | ENEMY_MASK))
                {
                    var hitGO = shootHit.collider.gameObject;

                    PlayerTargetNode targetComponent = null;
                    //note how the GameObject GetInstanceID is used to identify the entity as well
                    if (hitGO.layer == ENEMY_LAYER && nodesDB.QueryNode(hitGO.GetInstanceID(), out targetComponent))
                    {
                        var damageInfo = new DamageInfo(playerGunComponent.damagePerShot, shootHit.point, hitGO.GetInstanceID());
                        _enemyDamageSequence.Next(this, ref damageInfo);

                        playerGunComponent.lastTargetPosition = shootHit.point;
                        playerGunHitComponent.targetHit.value = true;
                        hitSomething = true;
                    }
                }
            }
            if (hitSomething) return;
            playerGunHitComponent.targetHit.value = false;
        }

        void OnTargetDead(int targetID)
        {
            var playerTarget = nodesDB.QueryNode<PlayerTargetNode>(targetID);
            var targetType = playerTarget.targetTypeComponent.targetType;

            _enemyKilledObservable.Dispatch(ref targetType);
        }

        public void Step(ref DamageInfo token, Enum condition)
        {
            OnTargetDead(token.entityDamaged);
        }

        readonly Type[] _acceptedNodes = { typeof(GunNode), typeof(PlayerNode) };

        GunNode                 _playerGunNode;
        EnemyKilledObservable   _enemyKilledObservable;
        Sequencer              _enemyDamageSequence;
        Sequencer              _ammoSequence;

        static readonly int SHOOTABLE_MASK = LayerMask.GetMask("Shootable");
        static readonly int ENEMY_MASK = LayerMask.GetMask("Enemies");
        static readonly int ENEMY_LAYER = LayerMask.NameToLayer("Enemies");
        
    }
}
