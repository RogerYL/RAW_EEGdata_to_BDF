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
            return header;
        }

        // Main function to write all signals data to BDF
        public void generateBDFData(BDF_Header iHeader, string path)
        {
            BDF_Header header = iHeader;
            List<BDF_Signal> Signals = header.bdfSignals;

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