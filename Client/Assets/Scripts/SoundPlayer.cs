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
        public Queue<String> MusicQueue = new Queue<String>();
        public Queue<String> SfxQueue = new Queue<String>();

        public void Update() 
        {
            if (MusicQueue.Count != 0 && !_audio.isPlaying) 
            {
                PlayMusic(MusicQueue.Dequeue());
            }
            if (SfxQueue.Count != 0)
            {
                PlaySfx(SfxQueue.Dequeue());
            }
        }

        public void QueueRandomSong() 
        {
            var songs = Directory.GetFiles(@"TestWorld\sound\music", "*.ogg", SearchOption.AllDirectories);
            var index = new System.Random().Next(songs.Length);
            var song = Path.GetFileNameWithoutExtension(songs[index]);
            MusicQueue.Enqueue(song);
        }

        public void PlayMusic(String name)
        {
            var clip = GetAudioClip(name);
            _audio.clip = clip;
            _audio.loop = false;
            _audio.Play();
            Locator.Get<SoundPlayer>().QueueRandomSong();
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
                if (String.Equals(Path.GetFileNameWithoutExtension(d), name, StringComparison.OrdinalIgnoreCase))
                {
                    return d;
                }
            }
            throw new Exception("No file found: " + name);
        }
    }
}
