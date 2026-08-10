using System.Linq;
using UnityEngine;

namespace Just5Store.Modules.PianoGame
{
    public class Piano : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PianoKey[] pianoKeys;
        [SerializeField] private PianoAmbient pianoAmbient;

        [Header("Materials")]
        [SerializeField] private Material whiteKeyMaterial;
        [SerializeField] private Material blackKeyMaterial;
        [SerializeField] private Material activeKeyMaterial;

        [Header("Settings")]
        [SerializeField] private int startMidiNote = 21;

        public int StartMidiNote => startMidiNote;
        public int EndMidiNote => startMidiNote + pianoKeys.Length - 1;
        public int KeyCount => pianoKeys != null ? pianoKeys.Length : 0;

        private void Awake()
        {
            Initialize();
        }

        [ContextMenu("Collect Piano Keys")]
        public void CollectPianoKeys()
        {
            pianoKeys = GetComponentsInChildren<PianoKey>(true)
                .OrderBy(key => key.transform.position.x)
                .ToArray();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [ContextMenu("Setup Keys")]
        public void SetupKeys()
        {
            if (pianoKeys == null || pianoKeys.Length == 0)
                return;

            for (int i = 0; i < pianoKeys.Length; i++)
            {
                PianoKey key = pianoKeys[i];

                if (key == null)
                    continue;

                bool isSharp = IsSharpByMidi(startMidiNote + i);
                Material idleMaterial = isSharp ? blackKeyMaterial : whiteKeyMaterial;

                key.Setup(key.transform, idleMaterial, activeKeyMaterial);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [ContextMenu("Initialize Piano")]
        public void Initialize()
        {
            if (pianoKeys == null || pianoKeys.Length == 0)
                return;

            for (int i = 0; i < pianoKeys.Length; i++)
            {
                PianoKey key = pianoKeys[i];

                if (key == null)
                    continue;

                bool isSharp = IsSharpByMidi(startMidiNote + i);
                Material idleMaterial = isSharp ? blackKeyMaterial : whiteKeyMaterial;

                key.Initialize();
                key.SetIdleMaterial(idleMaterial);
                key.SetPressedMaterial(activeKeyMaterial);
                key.Release();
            }
        }

        public bool HasMidiNote(int midiNote)
        {
            return midiNote >= StartMidiNote && midiNote <= EndMidiNote;
        }

        public PianoKey GetKeyByMidi(int midiNote)
        {
            int keyIndex = midiNote - startMidiNote;

            if (IsKeyIndexInvalid(keyIndex))
                return null;

            return pianoKeys[keyIndex];
        }

        public void Press(int midiNote)
        {
            int keyIndex = midiNote - startMidiNote;
            PressKeyByIndex(keyIndex);
        }

        public void Release(int midiNote)
        {
            int keyIndex = midiNote - startMidiNote;
            ReleaseKeyByIndex(keyIndex);
        }

        public void PressKeyByIndex(int keyIndex)
        {
            if (IsKeyIndexInvalid(keyIndex))
                return;

            PianoKey key = pianoKeys[keyIndex];
            if (key == null)
                return;

            key.Press();

            if (pianoAmbient != null && activeKeyMaterial != null)
            {
                pianoAmbient.Spawn(key.transform.position.x, activeKeyMaterial.color, keyIndex);
            }
        }

        public void ReleaseKeyByIndex(int keyIndex)
        {
            if (IsKeyIndexInvalid(keyIndex))
                return;

            PianoKey key = pianoKeys[keyIndex];
            if (key == null)
                return;

            key.Release();

            if (pianoAmbient != null)
            {
                pianoAmbient.Hide(keyIndex);
            }
        }

        public void ReleaseAll()
        {
            if (pianoKeys == null)
                return;

            for (int i = 0; i < pianoKeys.Length; i++)
            {
                ReleaseKeyByIndex(i);
            }
        }

        public void ResetAllKeys()
        {
            ReleaseAll();
        }

        private bool IsKeyIndexInvalid(int keyIndex)
        {
            return pianoKeys == null || keyIndex < 0 || keyIndex >= pianoKeys.Length;
        }

        private bool IsSharpByMidi(int midiNote)
        {
            int noteIndex = midiNote % 12;

            return noteIndex == 1 ||
                   noteIndex == 3 ||
                   noteIndex == 6 ||
                   noteIndex == 8 ||
                   noteIndex == 10;
        }
    }
}