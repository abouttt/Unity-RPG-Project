using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private readonly List<AudioSource> _audioSources = new();
    private Transform _root;

    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "Sound_Root" }.transform;
            Object.DontDestroyOnLoad(_root);

            var names = System.Enum.GetNames(typeof(SoundType));
            for (int i = 0; i < names.Length; i++)
            {
                var go = new GameObject() { name = names[i] };
                _audioSources.Add(go.AddComponent<AudioSource>());
                go.transform.parent = _root;
            }

            _audioSources[(int)SoundType.Bgm].loop = true;
        }
    }

    public void Play(string key, SoundType type)
    {
        var audioClip = Managers.Resource.Load<AudioClip>(key);
        Play(audioClip, type);
    }

    public void Play(AudioClip audioClip, SoundType type)
    {
        if (audioClip == null)
        {
            return;
        }

        var audioSource = _audioSources[(int)type];

        if (type == SoundType.Bgm)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Stop(SoundType type)
    {
        _audioSources[(int)type].Stop();
    }

    public float GetVolume(SoundType type)
    {
        return _audioSources[(int)type].volume;
    }

    public void SetVolume(SoundType type, float volume)
    {
        _audioSources[(int)type].volume = volume;
    }

    public void Clear()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
}
