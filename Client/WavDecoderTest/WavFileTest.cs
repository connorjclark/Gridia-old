using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Wav;

namespace WavDecoderTest
{
    [TestClass]
    public class WavFileTest
    {
        [TestMethod]
        public void Canary()
        {
            Assert.IsTrue(true);
        }

        private WavFile GetWav(String name)
        {
            var soundsPath = "../../../../Server/GridiaServer/worlds/demo-world/clientdata/sound";
            return new WavFile(File.ReadAllBytes(String.Format("{0}/{1}.wav", soundsPath, name)));
        }

        [TestMethod]
        public void ValidHeadersParse()
        {
            var soundsPath = "../../../worlds/demo-world/clientdata/sound";
            var soundPaths = Directory.GetFiles(soundsPath, "*.wav", SearchOption.AllDirectories);
            var failedSounds = new List<String>();
            var exceptions = soundPaths
                .Select(soundPath =>
                {
                    try
                    {
                        Console.WriteLine("\n" + soundPath + ":\n");
                        return new WavFile(File.ReadAllBytes(soundPath));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        failedSounds.Add(soundPath);
                        return (Object) ex;
                    }
                }).OfType<Exception>()
                .ToList();
            if (exceptions.Count == 0) return;
            Assert.Fail(exceptions.Count + " / " + soundPaths.Count() + " wav files failed.\n" + String.Join("\n", failedSounds.ToArray()));
        }

        [TestMethod]
        public void CorrectSampleRate()
        {
            var wav = GetWav("sfx/rpgwo/criket");
            Assert.AreEqual(11025, wav.SampleRate);
        }

        [TestMethod]
        public void CorrectAudioData()
        {
            var wav = GetWav("sfx/rpgwo/criket");
            var expectedValue = new float[]
            {
                0.003921569f,
                0.03529412f,
                0.003921569f,
                -0.07450981f,
                0.003921569f,
                0.003921569f
            };
            var expectedIndex = new int[]
            {
                0,
                100,
                1000,
                5000,
                10000,
                11285
            };
            for (var i = 0; i < expectedValue.Length; i++)
            {
                var expected = expectedValue[i];
                Assert.AreEqual(expected, wav.AudioData[expectedIndex[i]], Math.Abs(expected * 0.0001));
            }
            Assert.AreEqual(11286, wav.AudioData.Length);
        }

        [TestMethod]
        public void WavWithTwoChannelsParses()
        {
            var wav = GetWav("sfx/ryanconway/woodcutting");
            Assert.AreEqual(2, wav.NumChannels);
            Assert.AreEqual(wav.AudioData.Length, wav.NumFrames * wav.NumChannels);
        }
    }
}
