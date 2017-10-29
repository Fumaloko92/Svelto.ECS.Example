using Svelto.ECS.Example.Components.Ammo;
using Svelto.ECS.Example.Components.HUD;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Implementers.HUD
{
    // Class to manage the ammo counter displayed on the HUD
    public class AmmoManager : MonoBehaviour, IAmmoComponent
    {
        int IAmmoComponent.currentAmmo { get { return _score; } set { _score = value; _text.text = _score.ToString(); } }

        void Awake()
        {
            // Set up the reference.
            _text = GetComponentInChildren<Text>();

            // Reset the score.
            _score = 0;
        }

        int _score;
        Text _text;
    }
}
