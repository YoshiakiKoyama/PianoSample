using UnityEngine;
using UnityEngine.InputSystem;

namespace Just5Store.Modules.PianoGame
{
    public class PianoDemoInput : MonoBehaviour
    {
        [SerializeField] private Piano piano;

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                piano.Press(60);
            }

            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                piano.Release(60);
            }
        }
    }
}