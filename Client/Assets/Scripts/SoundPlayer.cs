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
        private AudioSource _musicAudio;
        [SerializeField]
        private AudioSource _sfxAudio;
        public Queue<String> MusicQueue = new Queue<String>();
        private bool _muteMusic;
        public bool MuteMusic
        {
            get { return _muteMusic; }
            set
            {
                _muteMusic = value;
                if (_muteMusic) 
                {
                    _musicAudio.Pause();
                }
                else
                {
                    _musicAudio.Play();
                }
            }
        }
        public bool MuteSfx { get; set; }
        public String CurrentSongName { get; private set; }

        public void Start() 
        {
            _musicAudio.loop = false;
            _musicAudio.volume = 0.6f;
            MuteMusic = Application.isEditor;
        }

        public void Update() 
        {
            if (MusicQueue.Count == 0)
            {
                QueueRandomSongs();
            }
            if (!_musicAudio.isPlaying && !_muteMusic) 
            {
                PlayMusic(MusicQueue.Dequeue());
            }
        }

        private List<String> Shuffle(List<String> list)
        {
            var rnd = new System.Random();
            return list.OrderBy(item => rnd.Next()).ToList();
        }

        public void QueueRandomSongs() 
        {
            var songs = Directory.GetFiles(GridiaConstants.WORLD_NAME + @"\sound\music", "*.ogg", SearchOption.AllDirectories)
                .ToList()
                .Select(fullSongPath => Path.GetFileNameWithoutExtension(fullSongPath))
                .ToList();
            MusicQueue = new Queue<String>(Shuffle(songs));
        }

        public void PlayMusic(String name)
        {
            CurrentSongName = name;
            var clip = GetAudioClip(name);
            _musicAudio.clip = clip;
            
            if (!_muteMusic) 
            {
                _musicAudio.Play();
            }
        }

        public void EndCurrentSong() 
        {
            _musicAudio.Stop();
        }

        public void PlaySfx(String name, Vector3 loc) 
        {
            if (!MuteSfx)
            {
                var clip = GetAudioClip(name);
                var playerLoc = Locator.Get<TileMapView>().Focus.Position;
                var dist = Vector3.Distance(playerLoc, loc);
                var volume = dist < 5 ? 1 : 1 / (1 + 0.25f * (dist - 5) * (dist - 5));
                _sfxAudio.PlayOneShot(clip, volume);
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
            foreach (string d in Directory.GetFiles(GridiaConstants.WORLD_NAME + @"\sound", "*.*", SearchOption.AllDirectories))
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
