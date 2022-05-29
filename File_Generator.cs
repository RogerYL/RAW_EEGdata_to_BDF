using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MPSA41CH
{
    public class File_Generator
    {
        private string _savebuf_filename = @"D:\savebuf.dat";
        private string _bdf_temp = @"D:\BDF_temp.bdf";

        public void Create_BDFHeader()
        {
            FileStream original_dat = new FileStream(_savebuf_filename, FileMode.Open, FileAccess.Read);
            BinaryReader B_dat = new BinaryReader(original_dat);
            original_dat.Position = 0;
            byte[] readbuf = B_dat.ReadBytes((int)original_dat.Length);
            uint ChannelLength = (uint)readbuf.Length / 170;

            uint calculate_SampleRate = new uint();
            switch (readbuf[5])
            {
                case 0x05:
                    calculate_SampleRate = 125;
                    SRNo = 4;
                    break;
                case 0x04:
                    calculate_SampleRate = 250;
                    SRNo = 3;
                    break;
                case 0x03:
                    calculate_SampleRate = 500;
                    SRNo = 2;
                    break;
                case 0x02:
                    calculate_SampleRate = 1000;
                    SRNo = 1;
                    break;
                case 0x01:
                    calculate_SampleRate = 2000;
                    SRNo = 0;
                    break;
                default:
                    calculate_SampleRate = 500;
                    SRNo = 2;
                    break;
            }
            uint num_Trigger = 0;
            for (int trigger_pos = 0; trigger_pos < ChannelLength; trigger_pos++)
            {
                int index = trigger_pos * 170 + 7;
                if (readbuf[index] != 0)
                {
                    num_Trigger += 1;
                }
            }

            double Calculate_Time;
            uint Calculate_numrecords;

            if (num_Trigger < (ChannelLength / SampleRate))
            {
                Calculate_numrecords = ChannelLength / SampleRate;
                Calculate_Time = 1;
            }
            else
            {
                Calculate_numrecords = num_Trigger;
                double Time = ChannelLength * 10 / (num_Trigger * SampleRate);
                Calculate_Time = Time / 10;
            }

            //Initial information
            string version = "BIOSEMI";
            string recording = "Startdate X X X X TEST RECORD ID";
            string patient = "X X X X " + Patient_name;
            string startDate = "11.11.16";
            string startTime = "12.12.12";
            string reserved = "BDF+C";
            string numRecords = Convert.ToString(Calculate_numrecords);
            string durationRecord = Convert.ToString(Calculate_Time);
            string ns = "42";
            BDF_Library.BDF_Header newHeader = new BDF_Library.BDF_Header(version, patient, recording, startDate, startTime, reserved, numRecords, durationRecord, ns);
            BDF_Library.Output_Holder.BDFHeaderHolder = newHeader;

            BDF_Library.BDF_Header header = BDF_Library.Output_Holder.BDFHeaderHolder;
            //clear and create the relevant lists
            string NSig = Convert.ToString((int)(SampleRate * Calculate_Time));
            int numSignals = Convert.ToInt32(header.ns);
            List<string> labels = new List<string>() { "FP2", "FP1", "F4", "F3", "C4", "C3", "P4", "P3", "O2", "O1", "A2", "A1", "F8", "F7", "T4", "T3", "T6", "T5", "FZ", "CZ", "FC4", "FC3", "CP4", "CP3", "FT8", "FT7", "TP8", "TP7", "FCZ", "CPZ", "OZ", "PZ", "X2", "REF", "X3", "X1", "BP1", "BP2", "BP3", "BP4" };
            List<string> transducerTypes = new List<string>();
            List<string> physicalDimensions = new List<string>();
            List<string> physicalMinimums = new List<string>();
            List<string> physicalMaximums = new List<string>();
            List<string> digitalMinimums = new List<string>();
            List<string> digitalMaximums = new List<string>();
            List<string> prefilterings = new List<string>();
            List<string> numSamplesPerRecords = new List<string>();
            for (int i = 0; i < numSignals - 2; i++)
            {
                transducerTypes.Add("");
                physicalDimensions.Add("mV");
                physicalMinimums.Add("-2500");
                physicalMaximums.Add("2500");
                digitalMinimums.Add("-8388608");
                digitalMaximums.Add("8388607");
                prefilterings.Add("");
                numSamplesPerRecords.Add(NSig);
            }
            
            //to allow for triggers to slip in
            labels.Add("Trigger");
            transducerTypes.Add("Status");
            physicalDimensions.Add("Boolean");
            physicalMinimums.Add("-8388608");
            physicalMaximums.Add("8388607");
            digitalMinimums.Add("-8388608");
            digitalMaximums.Add("8388607");
            prefilterings.Add("No filtering");
            numSamplesPerRecords.Add(NSig);

            //to allow for annotations to slip in
            labels.Add("BDF Annotations");
            transducerTypes.Add("");//must be empty
            physicalDimensions.Add("trigger");
            physicalMinimums.Add("-1");
            physicalMaximums.Add("1");
            digitalMinimums.Add("-8388608");
            digitalMaximums.Add("8388607");
            prefilterings.Add("");//must be empty
            numSamplesPerRecords.Add(NSig);
            
            header.setNSDependantData(labels, transducerTypes, physicalDimensions, physicalMinimums, physicalMaximums, digitalMinimums, digitalMaximums, prefilterings, numSamplesPerRecords);

            byte Start = 255;
            BinaryWriter Begin = new BinaryWriter(File.Open(_bdf_temp, FileMode.Create));
            Begin.Write(Start);
            Begin.Close();

            StreamWriter writer = new StreamWriter(_bdf_temp, true);
            writer.Write(header.generateBDFHeader());
            writer.Close();
        }

        bool SaveSignalAsBDF(string FileName)
        {
            Create_BDFHeader();
            BDF_Library.BDF_DataManager BDFmanager = new BDF_Library.BDF_DataManager();
            BDF_Library.Output_Holder.BDFHeaderHolder = BDFmanager.AddFile(_savebuf_filename, BDF_Library.Output_Holder.BDFHeaderHolder);
            BDFmanager.generateBDFData(BDF_Library.Output_Holder.BDFHeaderHolder, _bdf_temp);
            File.Copy(_bdf_temp, FileName, true);
            return true;
        }

        bool SaveSignalAsM41(string FileName)
        {
            File.Copy(_savebuf_filename, FileName, true);
            return true;
        }

        public bool SaveSignal(string Filename)
        {
            bool result = false;

            if (Filename.EndsWith("bdf", true, null))
            {
                try
                {
                    result = SaveSignalAsBDF(Filename);
                }
                catch (System.Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            else
            {
                MessageBox.Show("不支持的信号文件格式！");
                result = false;
            }

            return true;
        }

    }
}
