using UnityEngine;

namespace Just5Store.Modules.PianoGame
{
    public class LightActor : MonoBehaviour
    {
        public Light light;

        private void Awake()
        {
            light = GetComponent<Light>();
        }

        public void SetColor(Color color)
        {
            light.color = color;
        }
    }
}