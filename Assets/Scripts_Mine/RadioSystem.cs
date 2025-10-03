using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RadioSystem : MonoBehaviour
{
    [Header("Audio")]
    public List<AudioClip> songs;
    private AudioSource audioSource;
    private int currentIndex = 0;

    [Header("XR Input")]
    public InputActionProperty nextSongButton; // Asignar boton A
    public InputActionProperty prevSongButton; // Asignar boton B

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Importante: Activamos el loop del AudioSource
        audioSource.loop = true;

        PlaySong(currentIndex);
    }

    void Update()
    {
        if (!isHeld) return;

        if (nextSongButton.action.WasPerformedThisFrame())
        {
            NextSong();
        }
        if (prevSongButton.action.WasPerformedThisFrame())
        {
            PrevSong();
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    void PlaySong(int index)
    {
        if (songs.Count == 0 || index < 0 || index >= songs.Count) return;

        currentIndex = index;
        audioSource.clip = songs[currentIndex];
        audioSource.Play();
    }

    void NextSong()
    {
        currentIndex = (currentIndex + 1) % songs.Count;
        PlaySong(currentIndex);
    }

    void PrevSong()
    {
        currentIndex = (currentIndex - 1 + songs.Count) % songs.Count;
        PlaySong(currentIndex);
    }
}
