using System;
using System.Collections.Generic;

namespace MPSA41CH.BDF_Library
{
    class BDF_Annotation
    {
        //TODO do up this annotation to allow for definition of last and first TALs, and more than 1 tal per record (totalbytes currently limits this artifically.
        public char onsetSign { get; set; }
        public double onset { get; set; }
        public double duration { get; set; }
        public string[] annotations { get; set; }
        public int totalBytes { get; set; }
        public int remainingBytes { get; set; }

        private const byte beforeDuration = 21;
        private const byte beforeAnnotation = 20;
        private const byte afterAnnotation = 20;
        private const byte endOfTAL = 0;
        private const byte midOfTAL = 0;
        public List<byte> builtAnnotation;

        //Constructor for an EDF TAL Annotation that is last TAL
        public BDF_Annotation(char onsetSign, double onset, int totalBytes)
        {
            builtAnnotation = new List<byte>();
            this.onsetSign = onsetSign;
            this.onset = onset;
            this.duration = duration;
            this.annotations = annotations;
            this.totalBytes = totalBytes;

            builtAnnotation.Add((Byte)onsetSign); //write onset sign (+/-)
            builtAnnotation.AddRange(new System.Text.ASCIIEncoding().GetBytes(Convert.ToString(onset))); // convert onset to bytes and add to the byte array
            builtAnnotation.Add(beforeAnnotation);
            builtAnnotation.Add(beforeAnnotation);

            if ((totalBytes - builtAnnotation.Count) > 0)
            {
                int remainingBytes = totalBytes - builtAnnotation.Count;
                for (int i = 0; i < remainingBytes; i++)
                {
                    builtAnnotation.Add(endOfTAL);
                }
            }
            else
            {
                throw new ArgumentException("Not enough bytes remaining to complete annotation! Have " + totalBytes + " bytes!");
            }
        }

        public BDF_Annotation(char onsetSign, double onset, double timing, string[] annotations, int totalBytes)
        {
            builtAnnotation = new List<byte>();
            this.onsetSign = onsetSign;
            this.onset = onset;
            this.duration = duration;
            this.annotations = annotations;
            this.totalBytes = totalBytes;

            builtAnnotation.Add((Byte)onsetSign); //write onset sign (+/-)
            builtAnnotation.AddRange(new System.Text.ASCIIEncoding().GetBytes(Convert.ToString(onset)));
            builtAnnotation.Add(beforeAnnotation);
            builtAnnotation.Add(beforeAnnotation);

            builtAnnotation.Add(midOfTAL);

            builtAnnotation.Add((Byte)onsetSign);
            builtAnnotation.AddRange(new System.Text.ASCIIEncoding().GetBytes(Convert.ToString(timing))); //  convert duration to bytes and add to the byte array
            builtAnnotation.Add(beforeAnnotation);

            for (int i = 0; i < annotations.Length; i++)
            {
                builtAnnotation.AddRange(new System.Text.ASCIIEncoding().GetBytes(annotations[i]));
                builtAnnotation.Add(afterAnnotation);
            }

            if ((totalBytes - builtAnnotation.Count) > 0)
            {
                int remainingBytes = totalBytes - builtAnnotation.Count;
                for (int i = 0; i < remainingBytes; i++)
                {
                    builtAnnotation.Add(endOfTAL);
                }
            }
            else
            {
                throw new ArgumentException("Not enough bytes remaining to complete annotation! Have " + totalBytes + " bytes!");
            }
        }
    }
}
