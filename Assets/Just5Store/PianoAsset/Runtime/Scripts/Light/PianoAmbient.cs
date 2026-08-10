using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


namespace Just5Store.Modules.PianoGame
{
    public class PianoAmbient : MonoBehaviour
    {
        [SerializeField] private LightActor lightPrefab;
        [SerializeField] private ParticleActor explodeParticlePrefab;
        [SerializeField] private Vector3 lightOffset;
        [SerializeField] private Vector3 particleOffset;
        private static ObjectPool<LightActor> _lightPool;
        private static ObjectPool<ParticleActor> _exParticlePool;

        private readonly Dictionary<int, LightActor> _activeLights = new Dictionary<int, LightActor>();
        private readonly Dictionary<int, ParticleActor> _activeExParticle = new Dictionary<int, ParticleActor>();
        private void Awake()
        {
            _lightPool = new ObjectPool<LightActor>(InstantiateLight);
            _exParticlePool = new ObjectPool<ParticleActor>(InstantiateExParticle);
        }

        private ParticleActor InstantiateExParticle()
        {
            var particleInstance = Instantiate(explodeParticlePrefab, transform);
            particleInstance.gameObject.SetActive(false);
            return particleInstance;
        }

        private LightActor InstantiateLight()
        {
            var lightInstance = Instantiate(lightPrefab, transform);
            lightInstance.gameObject.SetActive(false);
            return lightInstance;
        }

        public void Spawn(float posX, Color color, int index)
        {
            SpawnLight(posX, color, index);
            SpawnParticle(posX, color, index);
        }

        private void SpawnParticle(float posX, Color color, int index)
        {
            Vector3 position = new Vector3(posX, particleOffset.y, particleOffset.z);
            ParticleActor thisParticle;
            if (_activeExParticle.TryGetValue(index, out thisParticle))
            {
                thisParticle.transform.position = position;
                thisParticle.SetColor(color);
            }
            else
            {
                thisParticle = GetParticleFromPool();
                thisParticle.transform.position = position;
                thisParticle.SetColor(color);
                _activeExParticle.TryAdd(index, thisParticle);
            }

        }

        private void SpawnLight(float posX, Color color, int index)
        {
            Vector3 position = new Vector3(posX, lightOffset.y, lightOffset.z);
            LightActor thisLight;
            if (_activeLights.TryGetValue(index, out thisLight))
            {
                thisLight.transform.position = position;
                thisLight.SetColor(color);
            }
            else
            {
                thisLight = GetLightFromPool();
                thisLight.transform.position = position;
                thisLight.SetColor(color);
                _activeLights.TryAdd(index, thisLight);
            }
        }

        public void Hide(int index)
        {
            if (_activeLights.TryGetValue(index, out var value))
            {
                ReturnLightToPool(value);
                _activeLights.Remove(index);
            }
            if (_activeExParticle.TryGetValue(index, out var value2))
            {
                ReturnParticleToPool(value2);
                _activeExParticle.Remove(index);
            }
        }
        private void OnDestroy()
        {
            ClearAmbientInstances();
        }

        private void ClearAmbientInstances()
        {
            foreach (var keyValuePair in _activeLights)
            {
                ReturnLightToPool(keyValuePair.Value);
            }
            _activeLights.Clear();
            foreach (var keyValuePair in _activeExParticle)
            {
                ReturnParticleToPool(keyValuePair.Value);
            }
            _activeExParticle.Clear();
        }


        private LightActor GetLightFromPool()
        {

            LightActor noteInstance = _lightPool.Get();
            noteInstance.gameObject.SetActive(true);
            return noteInstance;
        }

        private ParticleActor GetParticleFromPool()
        {
            ParticleActor particleInstance = _exParticlePool.Get();
            particleInstance.gameObject.SetActive(true);
            particleInstance.Play();
            return particleInstance;
        }
        private void ReturnLightToPool(LightActor lightActor)
        {
            lightActor.gameObject.SetActive(false);
            _lightPool.Release(lightActor);
        }

        private void ReturnParticleToPool(ParticleActor particleActor)
        {
            particleActor.Stop();
            _exParticlePool.Release(particleActor);
        }
        public void OnChangeSong()
        {
            foreach (var keyValuePair in _activeExParticle)
            {
                keyValuePair.Value.Reset();
            }
            ClearAmbientInstances();
        }

        public void OnEditMode()
        {
            foreach (var keyValuePair in _activeExParticle)
            {
                keyValuePair.Value.Reset();
            }
        }

        public void OnTimeChanged()
        {
            ClearAmbientInstances();
        }
        public void OnPause()
        {

            foreach (var keyValuePair in _activeExParticle)
            {
                keyValuePair.Value.Pause();
            }
        }

        public void OnPlay()
        {

            foreach (var keyValuePair in _activeExParticle)
            {
                keyValuePair.Value.Play();
            }
        }


    }
}