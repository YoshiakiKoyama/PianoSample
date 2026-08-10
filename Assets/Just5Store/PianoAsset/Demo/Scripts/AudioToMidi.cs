using UnityEngine;
using Just5Store.Modules.PianoGame;

public class MicToMidi : MonoBehaviour
{
    [SerializeField] private Piano piano;

    private AudioClip micClip;
    private float[] samples = new float[2048];

    private int currentMidi = -1;

    void Start()
    {
        micClip = Microphone.Start(null, true, 1, 44100);
    }

    void Update()
    {
        int midi = DetectMidi();

        // Same note is still playing
        if (midi == currentMidi)
            return;

        // Release old note
        if (currentMidi != -1)
        {
            piano.Release(currentMidi);
        }

        currentMidi = midi;

        // Press new note
        if (currentMidi != -1)
        {
            Debug.Log("MIDI: " + currentMidi);
            piano.Press(currentMidi);
        }
    }

    int DetectMidi()
    {
        if (!Microphone.IsRecording(null))
            return -1;

        int position = Microphone.GetPosition(null);

        if (position < samples.Length)
            return -1;

        micClip.GetData(samples, position - samples.Length);

        // Check volume
        float volume = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            volume += Mathf.Abs(samples[i]);
        }

        volume /= samples.Length;

        if (volume < 0.01f)
            return -1;

        // Find pitch using autocorrelation
        int bestOffset = 0;
        float bestCorrelation = 0f;

        for (int offset = 22; offset < 882; offset++)
        {
            float correlation = 0f;

            for (int i = 0; i < samples.Length - offset; i++)
            {
                correlation += samples[i] * samples[i + offset];
            }

            if (correlation > bestCorrelation)
            {
                bestCorrelation = correlation;
                bestOffset = offset;
            }
        }

        if (bestOffset == 0)
            return -1;

        // Convert to frequency
        float frequency = 44100f / bestOffset;

        // Convert Hz to MIDI
        int midi = Mathf.RoundToInt(
            69f + 12f * Mathf.Log(frequency / 440f, 2f)
        );

        Debug.Log("Hz: " + frequency + " | MIDI: " + midi);

        return midi;
    }
}