using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MPSA41CH.BDF_Library
{
    class BDF_AnnotationGenerator
    {
        private Byte[][] annotationSignals;
        public string foldPath;
        public int num_Records;
        public int num_Trigger = 0;
        public int num_AnnotationBytes;
        public uint Channel_Length;
        public int Sample_Rate;
        public int Calculate_DataBlock;

        private byte[] Get_Triggers(string path)
        {
            FileStream original_dat = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader B_dat = new BinaryReader(original_dat);

            original_dat.Position = 0;
            byte[] readbuf = B_dat.ReadBytes((int)original_dat.Length);
            uint ChannelLength = (uint)readbuf.Length / 170;
            Channel_Length = ChannelLength;

            int SampleRate = new int();
            switch (readbuf[5])
            {
                case 0x05:
                    SampleRate = 125;
                    break;
                case 0x04:
                    SampleRate = 250;
                    break;
                case 0x03:
                    SampleRate = 500;
                    break;
                case 0x02:
                    SampleRate = 1000;
                    break;
                case 0x01:
                    SampleRate = 2000;
                    break;
                default:
                    SampleRate = 500;
                    break;
            }
            Sample_Rate = SampleRate;

            byte[] annotations = new byte[ChannelLength];

            for (int trigger_pos = 0; trigger_pos < ChannelLength; trigger_pos++)
            {
                int index = trigger_pos * 170 + 7;
                annotations[trigger_pos] = readbuf[index];
            }

            for (int i = 0; i < ChannelLength; i++)
            {
                if (annotations[i] != 0)
                {
                    num_Trigger += 1;
                }
            }

            if (num_Trigger > (ChannelLength / SampleRate))
            {
                double Time = ChannelLength * 10 / (num_Trigger * SampleRate);
                Calculate_DataBlock = (int)((Time / 10) * SampleRate);
            }
            original_dat.Dispose();
            return annotations;
        }

        public BDF_AnnotationGenerator(string path, int numRecords, int numAnnotationBytes)
        {
            foldPath = path;
            num_Records = numRecords;
            num_AnnotationBytes = numAnnotationBytes;
        }

        public Byte[][] getAnnotationSignals()
        {
            byte[] Filled_annotationSignals = Get_Triggers(foldPath);
            int counting = 0;

            annotationSignals = new Byte[num_Records][];
            if (num_Trigger < num_Records)
            {
                for (int i = 0; i < num_Records; i++)
                {
                    for (int j = 0; j < Sample_Rate; j++)
                    {
                        if (Filled_annotationSignals[i * Sample_Rate + j] != 0)
                        {
                            char onsetSign = '+';
                            double onset = double.Parse(Convert.ToString(counting));
                            float timing = (((i * Sample_Rate) + j) * 1000) / Sample_Rate;
                            double trigger_timing = double.Parse(Convert.ToString(timing / 1000));
                            string[] annotations = new string[1] { "Trigger Input " + Convert.ToString(Filled_annotationSignals[i * Sample_Rate + j]) };
                            BDF_Annotation annotation = new BDF_Annotation(onsetSign, onset, trigger_timing, annotations, num_AnnotationBytes);
                            annotationSignals[counting] = annotation.builtAnnotation.ToArray();
                            counting += 1;
                        }
                    }
                }

                for (int i = counting; i < num_Records; i++)
                {
                    char onsetSign = '+';
                    double onset = i;
                    BDF_Annotation annotation = new BDF_Annotation(onsetSign, onset, num_AnnotationBytes);
                    annotationSignals[i] = annotation.builtAnnotation.ToArray();
                }
            }
            else
            {
                for (int i = 0; i < num_Records; i++)
                {
                    for (int j = 0; j < Calculate_DataBlock; j++)
                    {
                        if (Filled_annotationSignals[i * Calculate_DataBlock + j] != 0)
                        {
                            char onsetSign = '+';
                            double onset = double.Parse(Convert.ToString(counting));
                            float timing = (((i * Calculate_DataBlock) + j) * 1000) / Sample_Rate;
                            double trigger_timing = double.Parse(Convert.ToString(timing / 1000));
                            string[] annotations = new string[1] { "Trigger Input " + Convert.ToString(Filled_annotationSignals[i * Calculate_DataBlock + j]) };
                            BDF_Annotation annotation = new BDF_Annotation(onsetSign, onset, trigger_timing, annotations, num_AnnotationBytes);
                            annotationSignals[counting] = annotation.builtAnnotation.ToArray();
                            counting += 1;
                        }
                    }
                }
            }

            return annotationSignals;
        }
    }
}
