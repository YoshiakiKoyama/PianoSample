using UnityEngine;
using Just5Store.Modules.PianoGame;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class MidiNoteSpawner : MonoBehaviour
{
    // References to the piano, falling note prefab, and MIDI file
    [SerializeField] private Piano piano;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private TextAsset midiFile;

    // Controls how fast notes fall and where they reach the piano
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float hitLineY = 0f;

    void Start()
    {
        // Read the MIDI file from the assigned TextAsset
        var midi = MidiFile.Read(
            new System.IO.MemoryStream(midiFile.bytes)
        );

        // Get tempo information to convert MIDI timing into seconds
        var tempoMap = midi.GetTempoMap();

        // Loop through every note in the MIDI file
        foreach (var note in midi.GetNotes())
        {
            // Find the piano key corresponding to the MIDI note number
            PianoKey key = piano.GetKeyByMidi((int)note.NoteNumber);

            // Skip notes outside the piano's supported range
            if (key == null)
                continue;

            // Convert the note's starting time into real time
            var start = TimeConverter.ConvertTo<MetricTimeSpan>(
                note.Time, tempoMap
            );

            // Convert the note's duration into real time
            var duration = LengthConverter.ConvertTo<MetricTimeSpan>(
                note.Length, note.Time, tempoMap
            );

            // Convert microseconds into seconds
            float startTime = (float)start.TotalMicroseconds / 1000000f;
            float noteLength = (float)duration.TotalMicroseconds / 1000000f;

            // Create a falling note from the prefab
            GameObject obj = Instantiate(notePrefab);

            // Calculate note height based on how long it lasts
            float height = Mathf.Max(0.1f, noteLength * fallSpeed);

            // Set the note's width, height, and thickness
            obj.transform.localScale = new Vector3(1f, height, 5f);

            // Position the note above its corresponding piano key
            // X = Piano key's horizontal position
            // Y = Starting height based on MIDI timing + 30-second delay
            // Z = Piano key's depth position
            obj.transform.position = new Vector3(
                key.transform.position.x,
                hitLineY + (startTime + 30f) * fallSpeed + height / 2f,
                key.transform.position.z
            );

            // Attach the FallingNote script to control movement
            FallingNote falling = obj.AddComponent<FallingNote>();

            // Initialize falling movement using MIDI timing
            // The extra 30 seconds delays when the note reaches the hit line
            falling.Initialize(
      startTime + 30f,
      fallSpeed,
      hitLineY
  );
        }
    }
}