using UnityEngine;
using Svelto.ECS.Example.Nodes.Enemies;
using Svelto.DataStructures;
using System;
using Svelto.ECS.Example.Components.Enemy;

namespace Svelto.ECS.Example.EntityDescriptors.EnemyWaves
{
    public class EnemyWavesEntityDescriptor : EntityDescriptor
    {
        IEnemyWavesAttackComponent _component;

        public EnemyWavesEntityDescriptor(IEnemyWavesAttackComponent componentsImplementor):base(null, componentsImplementor)
		{
            _component = componentsImplementor;
        }

        //this shows how you can override the BuildNodes to adapt it to your needs. Without calling the base
        //function, the automatic component injection will be disabled
        public override FasterList<INode> BuildNodes(int ID, Action<INode> removeAction)
        {
            var nodes = new FasterList<INode>();
            var node = new EnemyWavesAttackNode
            {
                waveComponent = _component
            };

            nodes.Add(node);
            return nodes;
        }
    }

    [DisallowMultipleComponent]
    public class EnemyWavesEntityDescriptorHolder : MonoBehaviour, IEntityDescriptorHolder
    {
        public EntityDescriptor BuildDescriptorType(object[] extraImplentors = null)
        {
            return new EnemyWavesEntityDescriptor(GetComponent<IEnemyWavesAttackComponent>());
        }
    }
}
