using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MPSA41CH.BDF_Library
{
    class BDF_DataManager
    {
        public BDF_Header AddFile(string path, BDF_Header iHeader)
        {
            return AddM41(path, iHeader);
        }

        private BDF_Header AddM41(string path, BDF_Header iHeader)
        {
            BDF_Header header = iHeader;

            FileStream original_dat = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader B_dat = new BinaryReader(original_dat);

            original_dat.Position = 0;
            byte[] readbuf = B_dat.ReadBytes((int)original_dat.Length);
            uint ChannelLength = (uint)readbuf.Length / 170;
            int sampleSize = (int)(ChannelLength);

            for (uint ch = 0; ch < 40; ch++)
            {
                BDF_DataRecordSignalSample[] samples = new BDF_DataRecordSignalSample[sampleSize];
                for (uint pos = 0; pos < ChannelLength; pos++)
                {
                    byte[] temp_save = new byte[3];
                    uint index = (pos * 170 + ch * 4 + 10);
                    temp_save[0] = readbuf[index];
                    temp_save[1] = readbuf[index + 1];
                    temp_save[2] = readbuf[index + 2];
                    samples[pos] = new BDF_DataRecordSignalSample(temp_save);
                }
                header.bdfSignals[(int)ch].samples = samples;
            }
            
            BDF_DataRecordSignalSample[] Trigger_samples = new BDF_DataRecordSignalSample[sampleSize];
            for (uint Trigger_pos = 0; Trigger_pos < ChannelLength; Trigger_pos++)
            {
                byte[] temp_save = new byte[3];
                uint index2 = (Trigger_pos * 170 + 7);
                temp_save[0] = readbuf[index2];
                temp_save[1] = patch;
                temp_save[2] = patch;
                Trigger_samples[Trigger_pos] = new BDF_DataRecordSignalSample(temp_save);
            }
            header.bdfSignals[40].samples = Trigger_samples;

            original_dat.Dispose();
            return header;
        }

        // Main function to write all signals data to BDF
        public void generateBDFData(BDF_Header iHeader, string path)
        {
            BDF_Header header = iHeader;
            List<BDF_Signal> Signals = header.bdfSignals;
            
            //check if annotations are added
            Byte[][] annotationSignals;
            //TODO Add annotations functionality above the default requirements
            int numAnnotationBytes = String_Convert.toInt32((Signals[Signals.Count - 1].numSamples)) * 2; // times 2 as we need number of bytes, not number of 2-byte ints.
            BDF_AnnotationGenerator defGen = new BDF_AnnotationGenerator(foldPath, String_Convert.toInt32(header.numRecords), numAnnotationBytes);
            annotationSignals = defGen.getAnnotationSignals();

            BDF_DataBlock dataBlock = new BDF_DataBlock();
            dataBlock.numRecords = String_Convert.toInt32(header.numRecords);
            //declare and define the records field
            dataBlock.records = new BDF_DataRecord[dataBlock.numRecords];

            for (int i = 0; i < dataBlock.numRecords; i++)
            {
                //create the required data record, initialise fields
                BDF_DataRecord record = new BDF_DataRecord();
                record.numSignals = String_Convert.toInt32(header.ns);
                //will contain all the signals for this record
                BDF_DataRecordSignal[] newSignals = new BDF_DataRecordSignal[record.numSignals];

                //for every signal, which will be handled separately
                for (int j = 0; j < Signals.Count; j++)
                {
                    BDF_DataRecordSignal newSignal = new BDF_DataRecordSignal();
                    newSignal.numSamples = String_Convert.toInt32(Signals[j].numSamples);
                    newSignal.samples = new BDF_DataRecordSignalSample[newSignal.numSamples];
                    for (int k = 0; k < newSignal.numSamples; k++)
                    {
                        newSignal.samples[k] = Signals[j].samples[k];
                    }
                    newSignals[j] = newSignal;
                }
                
                //because annotation need to be encoded as ascii(2byte), and bdf need 3byte sample unit, so turn 2byte to 3byte unit by patching 0 at the end
                byte[] all_annotation = new byte[annotationSignal.Length + (annotationSignal.Length / 2)];
                for (int m = 0; m < annotationSignal.Length / 2; m++)
                {
                    byte[] temp_annotation_save = Int16_to_byte(newAnnotationSamples[m].Trigger);
                    all_annotation[2 * m + 0] = temp_annotation_save[1];
                    all_annotation[2 * m + 1] = temp_annotation_save[0];
                }
                for(int n = annotationSignal.Length; n < (annotationSignal.Length + (annotationSignal.Length / 2)); n++)
                {
                    all_annotation[n] = patch;
                }

                //allocate every 3byte into BDF_DataRecordSignalSample
                for (int p = 0; p < annotationSignal.Length / 2; p++)
                {
                    byte[] filled_bdf_annotation = new byte[3];
                    filled_bdf_annotation[0] = all_annotation[3 * p];
                    filled_bdf_annotation[1] = all_annotation[3 * p + 1];
                    filled_bdf_annotation[2] = all_annotation[3 * p + 2];
                    bdf_Annotations[p] = new BDF_DataRecordSignalSample(filled_bdf_annotation);
                }

                newAnnotationSignal.numSamples = String_Convert.toInt32(Signals[Signals.Count - 1].numSamples); //get size of annotation field and set
                newAnnotationSignal.samples = bdf_Annotations; //set the samples of the signal to the previously converted and assigned 2byte ints

                //finally, put this as the last signal in the signal array
                newSignals[Signals.Count - 1] = newAnnotationSignal;
                
                record.signals = newSignals;
                dataBlock.records[i] = record;
            }

            //get the entire BDF Datablock
            byte[] outData = dataBlock.getBDFData();

            string test_path = path;

            //write to output BDF
            using (BinaryWriter b = new BinaryWriter(File.Open(path, FileMode.Append)))
            {
                for (int i = 0; i < outData.Length; i++)
                {
                    b.Write(outData[i]);
                }
                b.Close();
            }
        }
    }
}
