using Svelto.ECS.Example.Components.HUD;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Implementers.HUD
{
    // This class is used from the HUD to implement the showing of the current wave to the player.
    // Implementers should be dumb. This code might look like some logic is here but it is super dumb.
    public class WaveManager : MonoBehaviour, IWaveComponent
    {
        int IWaveComponent.wave {
            get { return _wave; }
            set {
                _wave = value;
                if (!_isLastWave)
                    SetWaveNumberText();
                else
                    SetFinalWaveText();
            }
        }
        bool IWaveComponent.isLastWave
        {
            get { return _isLastWave; }
            set { _isLastWave = value; SetFinalWaveText(); }
        }
        void Awake()
        {
            // Set up the reference.
            _text = GetComponent<Text>();

            // Reset the wave.
            _wave = 0;
        }

        private void SetFinalWaveText()
        {
            _text.text = "final wave";
        }

        private void SetWaveNumberText()
        {
            _text.text = "wave " + _wave;
        }

        int _wave;
        Text _text;
        bool _isLastWave; 
    }
}