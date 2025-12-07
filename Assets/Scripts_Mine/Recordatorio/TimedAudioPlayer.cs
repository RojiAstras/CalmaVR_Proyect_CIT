using System.Collections;
using UnityEngine;

public class TimedAudioPlayer : MonoBehaviour
{
    [Header("Referencia a la secuencia de respiración")]
    public BreathingSequence breathingSequence;

    [Header("Audio Source único")]
    public AudioSource audioSource;

    [Header("Audios por tiempo")]
    public AudioClip audio5MinutesClip;
    public bool enable5MinAudio = true;

    public AudioClip audio15MinutesClip;
    public bool enable15MinAudio = true;

    private bool audio5Pending = false;
    private bool audio15Pending = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        StartCoroutine(RecordatorioRoutine());
        StartCoroutine(CheckPendingAudios());
    }

    private IEnumerator RecordatorioRoutine()
    {
        // ---- TIEMPO 5 MINUTOS ----
        yield return new WaitForSeconds(5 * 60);

        if (enable5MinAudio)
        {
            if (!breathingSequence.IsSequenceRunning)
            {
                PlayClip(audio5MinutesClip);
            }
            else
            {
                audio5Pending = true;
            }
        }

        // ---- TIEMPO 15 MINUTOS ----
        yield return new WaitForSeconds(10 * 60); // 10 más = 15 total

        if (enable15MinAudio)
        {
            if (!breathingSequence.IsSequenceRunning)
            {
                PlayClip(audio15MinutesClip);
            }
            else
            {
                audio15Pending = true;
            }
        }
    }

    private IEnumerator CheckPendingAudios()
    {
        while (true)
        {
            // Cuando la secuencia termine…
            if (breathingSequence.SequenceFinished)
            {
                if (audio5Pending && enable5MinAudio)
                {
                    PlayClip(audio5MinutesClip);
                    audio5Pending = false;
                }

                if (audio15Pending && enable15MinAudio)
                {
                    PlayClip(audio15MinutesClip);
                    audio15Pending = false;
                }

                yield break; // se detiene, ya no hace falta revisar más
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
