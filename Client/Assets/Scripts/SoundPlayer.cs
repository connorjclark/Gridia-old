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
        private bool _muteMusic;
        public bool MuteMusic
        {
            get { return _muteMusic; }
            set
            {
                _muteMusic = value;
                if (_muteMusic) 
                {
                    _audio.Pause();
                }
                else
                {
                    _audio.Play();
                }
            }
        }
        public bool MuteSfx { get; set; }
        public String CurrentSongName { get; private set; }

        public void Start() 
        {
            _audio.loop = false;
            MuteMusic = Application.isEditor;
        }

        public void Update() 
        {
            if (MusicQueue.Count == 0)
            {
                QueueRandomSongs();
            }
            if (!_audio.isPlaying && !_muteMusic) 
            {
                PlayMusic(MusicQueue.Dequeue());
            }
            if (SfxQueue.Count != 0)
            {
                PlaySfx(SfxQueue.Dequeue());
            }
        }

        private List<String> Shuffle(List<String> list)
        {
            var rnd = new System.Random();
            return list.OrderBy(item => rnd.Next()).ToList();
        }

        public void QueueRandomSongs() 
        {
            var songs = Directory.GetFiles(@"TestWorld\sound\music", "*.ogg", SearchOption.AllDirectories)
                .ToList()
                .Select(fullSongPath => Path.GetFileNameWithoutExtension(fullSongPath))
                .ToList();
            MusicQueue = new Queue<String>(Shuffle(songs));
        }

        public void PlayMusic(String name)
        {
            CurrentSongName = name;
            var clip = GetAudioClip(name);
            _audio.clip = clip;
            
            if (!_muteMusic) 
            {
                _audio.Play();
            }
        }

        public void EndCurrentSong() 
        {
            _audio.Stop();
        }

        public void PlaySfx(String name) 
        {
            if (!MuteSfx)
            {
                var clip = GetAudioClip(name);
                _audio.PlayOneShot(clip);
            }
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
