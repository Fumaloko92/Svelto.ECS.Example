using System;
using UnityEngine;
using Svelto.ECS.Example.Components.HUD;

namespace Svelto.ECS.Example.Implementers.Player
{
    // The implementers of the ammo logic for the player
    public class PlayerAmmo : MonoBehaviour, IAmmoComponent
    {
        public int startingAmmo;

        private int _currentAmmo;

        void Awake()
        {
            _currentAmmo = startingAmmo;
        }
        int IAmmoComponent.currentAmmo { get { return _currentAmmo; } set { _currentAmmo = value; } }
    }
}
