using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RadioSystem : MonoBehaviour
{
    [Header("Audio")]
    public List<AudioClip> songs;
    private AudioSource audioSource;
    private int currentIndex = 0;

    [Header("Botones Físicos")]
    public XRSimpleInteractable nextButton;   // Cubo A
    public XRSimpleInteractable prevButton;   // Cubo B

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        PlaySong(currentIndex);

        // Conectamos los eventos a las funciones
        nextButton.selectEntered.AddListener(OnNextPressed);
        prevButton.selectEntered.AddListener(OnPrevPressed);
    }

    private void OnNextPressed(SelectEnterEventArgs args)
    {
        NextSong();
    }

    private void OnPrevPressed(SelectEnterEventArgs args)
    {
        PrevSong();
    }

    void PlaySong(int index)
    {
        if (songs.Count == 0) return;

        currentIndex = index;
        audioSource.clip = songs[currentIndex];
        audioSource.Play();
    }

    public void NextSong()
    {
        currentIndex = (currentIndex + 1) % songs.Count;
        PlaySong(currentIndex);
    }

    public void PrevSong()
    {
        currentIndex = (currentIndex - 1 + songs.Count) % songs.Count;
        PlaySong(currentIndex);
    }
}

