using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.Loop;
            s.source.spatialBlend = s.spatialBlend;
        }
    }

    public void Play(string name)
    {
        Sound s =Array.Find(sounds, sound => sound.name == name);

        if (s == null)
            return;
        s.source.Play();
    }
}
