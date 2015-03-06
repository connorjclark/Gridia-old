using System;
using System.IO;
using System.Text;

// http://www.topherlee.com/software/pcm-tut-wavformat.html
// http://www-mmsp.ece.mcgill.ca/documents/AudioFormats/WAVE/WAVE.html
// http://www.icculus.org/SDL_sound/downloads/external_documentation/wavecomp.htm
// http://wiki.multimedia.cx/index.php?title=IMA_ADPCM
// http://www.microchip.com/forums/FindPost/704746

// package convention?
// multiple channels with DVI ADPCM doesn't work
// streaming?

namespace Wav
{
    public class WavFile
    {
        private static readonly int RiffMarker = StringToIntValue("RIFF");
        private static readonly int WaveMarker = StringToIntValue("WAVE");
        private static readonly int FormatMarker = StringToIntValue("fmt ");
        private static readonly int DataMarker = StringToIntValue("data");

        private static int StringToIntValue(String str)
        {
            return BitConverter.ToInt32(Encoding.ASCII.GetBytes(str), 0);
        }

        public int SampleRate { get; private set; }
        public int NumChannels { get; private set; }
        public int NumFrames { get; private set; }
        public int BitsPerSample { get; private set; }
        public int BlockAlign { get; private set; }
        public int FormatType { get; private set; }
        public int SamplesPerChannelPerBlock { get; private set; }
        public float[] AudioData { get; private set; }

        public WavFile(byte[] data)
        {
            var stream = new BinaryReader(new MemoryStream(data));
            ReadHeader(stream);
        }

        private void Expect(int expected, int actual, String message)
        {
            if (expected != actual)
            {
                throw new Exception(String.Format("Expected <{0}>, got <{1}>: {2}", expected, actual, message));
            }
        }

        private void ReadHeader(BinaryReader data)
        {
            Expect(RiffMarker, data.ReadInt32(), "Expected file to start with RIFF");
            var writtenFileSize = data.ReadInt32() + 8;
            //Expect(data.Length, writtenFileSize, "File size did not match up");
            Expect(WaveMarker, data.ReadInt32(), "Expected file type to be WAVE");
            while (data.PeekChar() != -1)
            {
                ProcessChunk(data);
            }
        }

        private void ProcessChunk(BinaryReader data)
        {
            var chunkId = data.ReadInt32();
            var chunkSize = data.ReadInt32();
            Console.WriteLine("Chunk: " + chunkId);
            Console.WriteLine("Size: " + chunkSize);
            if (chunkId == FormatMarker)
            {
                //Expect(16, chunkSize, "Expected format chunk to be 16 bytes");
                ProcessFormatChunk(data);
                if (FormatType != 17)
                {
                    data.ReadBytes(chunkSize - 16);
                }
            }
            else if (chunkId == DataMarker)
            {
                NumFrames = chunkSize / BlockAlign;
                Console.WriteLine("NumFrames " + NumFrames);
                //Expect(0, chunkSize % BlockAlign, "Data length does not match up with block align");
                ProcessDataChunk(data);
            }
            else
            {
                Console.WriteLine("Unknown chunk ...");
                data.ReadBytes(chunkSize);
            }
        }

        private void ProcessFormatChunk(BinaryReader data)
        {
            FormatType = data.ReadInt16();
            Console.WriteLine("format type: " + FormatType);
            if (FormatType != 1 && FormatType != 17)
            {
                throw new Exception("Expected format to = 1, or 17. Was " + FormatType);
            }
            NumChannels = data.ReadInt16();
            SampleRate = data.ReadInt32();
            var averageBytesPerSecond = data.ReadInt32();
            BlockAlign = data.ReadInt16();
            BitsPerSample = data.ReadInt16();

            Console.WriteLine("numChannels " + NumChannels);
            Console.WriteLine("sampleRate " + SampleRate);
            Console.WriteLine("bitsPerSample " + BitsPerSample);

            Console.WriteLine("blockAlign " + BlockAlign);

            //Expect((SampleRate * BitsPerSample * NumChannels) / 8, averageBytesPerSecond, "(Sample Rate * BitsPerSample * Channels) / 8");
            //Expect((BitsPerSample * NumChannels) / 8, BlockAlign, "(BitsPerSample * Channels) / 8");
            if (FormatType == 17)
            {
                Expect(2, data.ReadInt16(), "Should be 2.");
                SamplesPerChannelPerBlock = data.ReadInt16();
                Console.WriteLine("SamplesPerChannelPerBlock " + SamplesPerChannelPerBlock);
            }
        }

        private void ProcessDataChunk(BinaryReader data)
        {
            switch (FormatType)
            {
                case 1: Decode_Uncompressed(data);
                    break;
                case 17: Decode_DVI_ADPCM(data);
                    break;
                default: throw new Exception("Unsupported format type: " + FormatType);
            }
        }

        private float ConvertToNormalizedFloat(int value, float scale, float offset)
        {
            return value / scale + offset;
        }

        private void Decode_Uncompressed(BinaryReader data)
        {
            var numBytes = NumFrames * BlockAlign * NumChannels;
            float floatOffset = BitsPerSample > 8 ? 0 : -1;
            var floatScale = (float)(BitsPerSample > 8 ? 1 << (BitsPerSample - 1) : 0.5 * ((1 << BitsPerSample) - 1));
            var bytes = data.ReadBytes(numBytes);

            var amplitudes = new float[NumFrames * NumChannels];
            Func<int, int> converter = null;
            switch (BitsPerSample)
            {
                case 8:
                    converter = position => bytes[position];
                    break;
                case 16:
                    converter = position => BitConverter.ToInt16(bytes, position * 2);
                    break;
            }
            for (var i = 0; i < amplitudes.Length; i++)
            {
                var amplitude = converter(i);
                amplitudes[i] = ConvertToNormalizedFloat(amplitude, floatScale, floatOffset);
            }

            AudioData = amplitudes;
        }

        private readonly int[] StepTab =
        {
            7, 8, 9, 10, 11, 12, 13, 14, 16, 17,
            19, 21, 23, 25, 28, 31, 34, 37, 41, 45,
            50, 55, 60, 66, 73, 80, 88, 97, 107, 118,
            130, 143, 157, 173, 190, 209, 230, 253, 279, 307,
            337, 371, 408, 449, 494, 544, 598, 658, 724, 796,
            876, 963, 1060, 1166, 1282, 1411, 1552, 1707, 1878, 2066,
            2272, 2499, 2749, 3024, 3327, 3660, 4026, 4428, 4871, 5358,
            5894, 6484, 7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
            15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767
        };

        private readonly int[] IndexTab4Bit =
        {
            -1, -1, -1, -1, 2, 4, 6, 8,
            -1, -1, -1, -1, 2, 4, 6, 8
        };

        private void Decode_DVI_ADPCM(BinaryReader data)
        {
            var previousSample = 0;
            var index = 0;

            //var floatScale = (float)(BitsPerSample > 8 ? 1 << (BitsPerSample - 1) : 0.5 * ((1 << BitsPerSample) - 1));
            //float floatOffset = BitsPerSample > 8 ? 0 : -1;

            Func<int, float> decode = sampleCode =>
            {
                if (index > 88)
                {
                    index = 88;
                }
                if (index < 0)
                {
                    index = 0;
                }

                var step = StepTab[index];
                var delta = step >> 3;
                if ((sampleCode & 4) != 0)
                {
                    delta += step;
                }
                if ((sampleCode & 2) != 0)
                {
                    delta += step >> 1;
                }
                if ((sampleCode & 1) != 0)
                {
                    delta += step >> 2;
                }
                if ((sampleCode & 8) != 0)
                {
                    delta = -delta;
                }
                var sample = previousSample + delta;
                if (sample > 32767)
                {
                    sample = 32767;
                }
                if (sample < -32768)
                {
                    sample = -32768;
                }
                previousSample = sample;
                index = index + IndexTab4Bit[sampleCode];

                //return ConvertToNormalizedFloat(sample, floatScale, floatOffset);
                return sample / 32767f;
            };

            AudioData = new float[(data.BaseStream.Length - data.BaseStream.Position) * 2]; // ?
            var i = 0;

            while (data.BaseStream.Length > data.BaseStream.Position)
            {
                var firstSample = (int)data.ReadInt16();
                var initialStepIndex = data.ReadByte();
                var reserved = data.ReadByte();

                previousSample = firstSample;
                index = initialStepIndex;

                for (var j = 0; j < SamplesPerChannelPerBlock / 2; j++)
                {
                    var sampleCodes = data.ReadByte();
                    var sampleCode1 = sampleCodes & 0x0F;
                    var sampleCode2 = (sampleCodes >> 4) & 0x0F;

                    AudioData[i++] = decode(sampleCode1);
                    AudioData[i++] = decode(sampleCode2);
                }
            }
        }
    }
}
