using Svelto.ECS.Example.Components.Base;
using System;
using UnityEngine;

namespace Svelto.ECS.Example.Implementers.Enemies
{
    // Implementer to provide the rigidbody to the EnemyNode in order to apply forces to it.
    public class EnemyRigidbody : MonoBehaviour, IRigidBodyComponent
    { 
        Rigidbody IRigidBodyComponent.rigidbody { get { return _rigidbody; } }

        Rigidbody _rigidbody;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }
}
