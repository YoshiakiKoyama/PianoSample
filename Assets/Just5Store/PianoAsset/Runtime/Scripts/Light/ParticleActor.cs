using System.Collections.Generic;
using UnityEngine;

namespace Just5Store.Modules.PianoGame
{
    public class ParticleActor : MonoBehaviour
    {
        public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

        public void Play()
        {
            foreach (var particle in particleSystems)
                particle.Play();
        }

        public void Pause()
        {
            foreach (var particle in particleSystems)
            {
                particle.Pause();
            }
        }

        public void Stop()
        {
            foreach (var particle in particleSystems)
                particle.Stop();
        }

        public void SetColor(Color color)
        {
            foreach (var particle in particleSystems)
            {
                var main = particle.main;
                main.startColor = color;
            }
        }

        public void Reset()
        {
            foreach (var particle in particleSystems)
            {
                particle.Clear();
                particle.Stop();
            }
        }
    }
}