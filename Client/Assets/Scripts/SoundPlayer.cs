using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class SoundPlayer : MonoBehaviour
    {
        private Dictionary<String, AudioClip> _audioClips = new Dictionary<String, AudioClip>();
        private AudioClip _currentMusic;
        [SerializeField]
        private AudioSource _audio;

        public void PlayMusic(String name)
        {
            var clip = GetAudioClip(name);
            _audio.clip = clip;
            _audio.Play();
            _audio.loop = true;
        }

        public void PlaySfx(String name) 
        {
            var clip = GetAudioClip(name);
            _audio.PlayOneShot(clip);
        }

        private AudioClip GetAudioClip(String name) 
        {
            if (!_audioClips.ContainsKey(name)) {
                _audioClips[name] = LoadAudioClip(name);
            }
            return _audioClips[name];
        }

        private AudioClip LoadAudioClip(String name) 
        {
            var fileLocation = SearchForFile(name);
            var audioLoader = new WWW("file://" + fileLocation);
            while (!audioLoader.isDone) { } // :(
            return audioLoader.GetAudioClip(false);
        }

        private String SearchForFile(String name) 
        {
            foreach (string d in Directory.GetFiles(@"TestWorld\sound", "*.*", SearchOption.AllDirectories))
            {
                if (Path.GetFileNameWithoutExtension(d) == name) return d;
            }
            throw new Exception("No file found: " + name);
        }
    }
}
