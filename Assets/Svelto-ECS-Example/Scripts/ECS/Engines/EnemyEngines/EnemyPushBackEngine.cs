using System;
using UnityEngine;
using Svelto.ECS.Example.Nodes.Enemies;
using Svelto.ECS.Example.Components.PlayerAbilities;
using Svelto.DataStructures;
using Svelto.ECS.Example.Components.Damageable;

namespace Svelto.ECS.Example.Engines.Enemies
{
    // This engine is used to hold the logic of the pushing skill that takes care of pushing the enemies towards a certain direction
    // and to manage the rigidbody drag that would prevent the enemy from receiving forces
    public class EnemyPushBackEngine : IEngine, IQueryableNodeEngine, IStep<PushAbilityInfo>, IStep<DamageInfo>
    {
        private Sequencer _pushAbilitySequence;

        public IEngineNodeDB nodesDB { set; private get; }

        public EnemyPushBackEngine(Sequencer pushAbilitySequence)
        {
            _enemiesPushed = new FasterList<int>();
            _pushAbilitySequence = pushAbilitySequence;
            TaskRunner.Instance.Run(new Tasks.TimedLoopActionEnumerator(Tick));
        }
        void Tick(float deltaSec)
        {
            if (_unstunTimer <= 0)
                for (int i = _enemiesPushed.Count - 1; i >= 0; i--)
                {
                    var enemyNode = nodesDB.QueryNode<EnemyNode>(_enemiesPushed[i]);
                    float magnitude = enemyNode.rigidbodyComponent.rigidbody.velocity.magnitude;
                    if (magnitude < .5f)
                    {
                        enemyNode.rigidbodyComponent.rigidbody.drag = float.PositiveInfinity;
                        enemyNode.rigidbodyComponent.rigidbody.angularDrag = float.PositiveInfinity;
                        int id = _enemiesPushed[i];
                        _pushAbilitySequence.Next(this, ref id, PushAbilityState.unstunning);
                        _enemiesPushed.RemoveAt(i);
                    }
                }
            else
                _unstunTimer -= deltaSec;
        }

        public void Step(ref PushAbilityInfo token, Enum condition)
        {
            if ((PushAbilityState)condition == PushAbilityState.pushing)
            {
                var enemyNode = nodesDB.QueryNode<EnemyNode>(token.enemyAffectedID);
                enemyNode.rigidbodyComponent.rigidbody.drag = 1;
                enemyNode.rigidbodyComponent.rigidbody.angularDrag = 1;
                enemyNode.rigidbodyComponent.rigidbody.AddForce(token.pushPower, ForceMode.VelocityChange);
                _pushApplied = false;
                if (_enemiesPushed.Count == 0) _unstunTimer = _timerCheckDelay;
                _enemiesPushed.Add(token.enemyAffectedID);
            }
        }

        public void Step(ref DamageInfo token, Enum condition)
        {
            if ((DamageCondition)condition == DamageCondition.dead)
                _enemiesPushed.Remove(token.entityDamaged);
        }

        private bool _pushApplied;
        private float _timerCheckDelay = .5f;
        private float _unstunTimer;

        FasterList<int> _enemiesPushed;
    }
}
