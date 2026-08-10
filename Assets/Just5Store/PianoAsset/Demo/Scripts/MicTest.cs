using UnityEngine;
using Just5Store.Modules.PianoGame;

public class MicTest : MonoBehaviour
{
    [SerializeField] private Piano piano;

    private AudioClip micClip;
    private float[] samples = new float[128];
    private bool isMakingSound = false;

    void Start()
    {
        micClip = Microphone.Start(null, true, 1, 44100);
    }

    void Update()
    {
        if (!Microphone.IsRecording(null))
            return;

        int position = Microphone.GetPosition(null);

        if (position < samples.Length)
            return;

        micClip.GetData(samples, position - samples.Length);

        float volume = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            volume += Mathf.Abs(samples[i]);
        }

        volume /= samples.Length;

        if (volume > 0.01f)
{
    if (!isMakingSound)
    {
        piano.Press(60);   // Press Middle C
        isMakingSound = true;
    }
}
else
{
    if (isMakingSound)
    {
        piano.Release(60); // Release Middle C
        isMakingSound = false;
    }
}
    }
}