using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using Serving.FileTransferring;
using Wav;

namespace Gridia
{
    public class SoundPlayer : MonoBehaviour
    {
        private readonly Dictionary<String, WavFile> _wavFiles = new Dictionary<String, WavFile>();
        private AudioClip _currentMusic;
        public AudioSource MusicAudio;
        public AudioSource SfxAudio;
        private FileSystem _fileSystem;
        public Queue MusicQueue = Queue.Synchronized(new Queue()); // Strings
        private bool _muteMusic;
        public bool MuteMusic
        {
            get { return _muteMusic; }
            set
            {
                _muteMusic = value;
                if (_muteMusic) 
                {
                    MusicAudio.Pause();
                }
                else
                {
                    MusicAudio.Play();
                }
            }
        }
        public bool MuteSfx { get; set; }
        public bool StartingMusic { get; set; }
        public bool LoadingQueue { get; set; }
        public String CurrentSongName { get; private set; }
        public bool BreakBecauseFirstTime { get; set; }

        public void Start() 
        {
            MusicAudio.loop = false;
            MusicAudio.volume = 0.6f;
            MuteMusic = Application.isEditor;
            _fileSystem = GridiaConstants.GetFileSystem();
        }

        public void Update() 
        {
            if (BreakBecauseFirstTime)
            {
                BreakBecauseFirstTime = GridiaConstants.WorldName == null;
                return;
            }
            if (LoadingQueue)
            {
                return;
            }
            if (MusicQueue.Count == 0)
            {
                QueueRandomSongs();
            }
            else if (!MusicAudio.isPlaying && !_muteMusic && !StartingMusic) 
            {
                PlayMusic(MusicQueue.Dequeue() as String);
            }
        }

        private List<String> Shuffle(List<String> list)
        {
            var rnd = new System.Random();
            return list.OrderBy(item => rnd.Next()).ToList();
        }

        public void QueueRandomSongs() 
        {
            LoadingQueue = true;
            new Thread(() => {
                _fileSystem.CreateDirectory("worlds"); //ensure it exists :(
                var clientDataFolder = @"worlds\" + GridiaConstants.WorldName; // :(
                Debug.Log("queueing songs...");
                // recursively? :(
                var songs = _fileSystem.GetFiles(clientDataFolder)
                    .ToList()
                    .Where(path => path.Contains(@"sound\music"))
                    .Where(path => path.EndsWith(".wav") || path.EndsWith(".WAV"))
                    .Select(fullSongPath => Path.GetFileNameWithoutExtension(fullSongPath))
                    .ToList();
                Debug.Log(String.Join(", ", songs.ToArray()));
                MusicQueue = Queue.Synchronized(new Queue(Shuffle(songs)));
                LoadingQueue = false;
                if (MusicQueue.Count == 0)
                {
                    BreakBecauseFirstTime = true;
                }
            }).Start();
        }

        public void PlayMusic(String name)
        {
            Debug.Log("Song: " + name);
            StartingMusic = true;
            new Thread(() => {
                CurrentSongName = name;
                var clip = GetAudioClip(name);
                MainThreadQueue.Add(() => {
                    MusicAudio.clip = clip;
                    MusicAudio.Play();
                    StartingMusic = false;
                });
            }).Start();
        }

        public void EndCurrentSong() 
        {
            MusicAudio.Stop();
        }

        public void PlaySfx(String name, float volume = 1.0f)
        {
            if (MuteSfx) return;
            new Thread(() =>
            {
                var clip = GetAudioClip(name);
                MainThreadQueue.Add(() =>
                {
                    SfxAudio.PlayOneShot(clip, volume);
                });
            }).Start();
        }

        public void PlaySfxAt(String name, Vector3 loc)
        {
            if (MuteSfx) return;
            var playerLoc = Locator.Get<TileMapView>().Focus.Position;
            var dist = Vector3.Distance(playerLoc, loc);
            var volume = dist < 5 ? 1 : 1 / (1 + 0.25f * (dist - 5) * (dist - 5));
            PlaySfx(name, volume);
        }

        private AudioClip GetAudioClip(String name)
        {
            var wavFile = GetWavFile(name);
            if (wavFile == null)
            {
                return null;
            }
            var signal = new AutoResetEvent(false);
            AudioClip audioClip = null;
            MainThreadQueue.Add(() =>
            {
                Debug.Log("SampleRate: " + wavFile.SampleRate);
                Debug.Log("frames: " + wavFile.NumFrames);
                Debug.Log("channels: " + wavFile.NumChannels);
                Debug.Log("size: " + wavFile.AudioData.Length);
                var position = 0;
                audioClip = AudioClip.Create(name, wavFile.AudioData.Length / wavFile.NumChannels, wavFile.NumChannels, wavFile.SampleRate, false, true, data =>
                {
                    for (var i = 0; i < data.Length && position < wavFile.AudioData.Length; i++)
                    {
                        data[i] = wavFile.AudioData[position++];
                    }
                }, p => position = p);
                signal.Set();
            });
            signal.WaitOne();
            return audioClip;
        }

        private WavFile GetWavFile(String name) 
        {
            if (!_wavFiles.ContainsKey(name)) {
                _wavFiles[name] = LoadWavFile(name);
            }
            return _wavFiles[name];
        }

        private WavFile LoadWavFile(String name)
        {
            try
            {
                var fileLocation = SearchForFile(name);
                Debug.Log("Load: " + fileLocation);
                var bytes = _fileSystem.ReadAllBytes(fileLocation);
                return new WavFile(bytes);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return null;
            }
        }

        private String SearchForFile(String name) 
        {
            _fileSystem.CreateDirectory("worlds"); //ensure it exists :(
            var clientDataFolder = @"worlds\" + GridiaConstants.WorldName; // :(
            foreach (string d in _fileSystem.GetFiles(clientDataFolder, "*", SearchOption.AllDirectories))
            {
                if (d.Contains("sound") && String.Equals(Path.GetFileNameWithoutExtension(d), name, StringComparison.OrdinalIgnoreCase))
                {
                    return d;
                }
            }
            throw new Exception("No file found: " + name);
        }
    }
}
