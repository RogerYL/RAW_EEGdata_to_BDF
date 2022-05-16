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
        private string _edf_temp = @"D:\EDF_temp.edf";
        private string _bdf_temp = @"D:\BDF_temp.bdf";
        public int SRNo;

        public void Create_EDFHeader()
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

            //Initial information
            string version = "0";
            string recording = "TEST RECORD ID";
            string patient = "TEST PATIENT ID";
            string startDate = "11.11.16";
            string startTime = "12.12.12";
            string reserved = "RESERVED";
            string numRecords = "1";
            string durationRecord = Convert.ToString(calculate_SampleRate);
            string ns = "40";
            EDF_Library.EDF_Header newHeader = new EDF_Library.EDF_Header(version, patient, recording, startDate, startTime, reserved, numRecords, durationRecord, ns);
            EDF_Library.ObjectHolder.EDFHeaderHolder = newHeader;

            EDF_Library.EDF_Header header = EDF_Library.ObjectHolder.EDFHeaderHolder;
            //clear and create the relevant lists
            string NSig = Convert.ToString(ChannelLength);
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
            for (int i = 0; i < numSignals; i++)
            {
                transducerTypes.Add("");
                physicalDimensions.Add("mV");
                physicalMinimums.Add("-625");
                physicalMaximums.Add("625");
                digitalMinimums.Add("-32768");
                digitalMaximums.Add("32767");
                prefilterings.Add("");
                numSamplesPerRecords.Add(NSig);
            }
            header.setNSDependantData(labels, transducerTypes, physicalDimensions, physicalMinimums, physicalMaximums, digitalMinimums, digitalMaximums, prefilterings, numSamplesPerRecords);

            StreamWriter writer = new StreamWriter(_edf_temp);
            writer.Write(header.generateEDFHeader());
            writer.Close();
        }

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

            //Initial information
            string version = "BIOSEMI";
            string recording = "TEST RECORD ID";
            string patient = "TEST PATIENT ID";
            string startDate = "11.11.16";
            string startTime = "12.12.12";
            string reserved = "RESERVED";
            string numRecords = "1";
            string durationRecord = Convert.ToString(calculate_SampleRate);
            string ns = "40";
            BDF_Library.BDF_Header newHeader = new BDF_Library.BDF_Header(version, patient, recording, startDate, startTime, reserved, numRecords, durationRecord, ns);
            BDF_Library.Output_Holder.BDFHeaderHolder = newHeader;

            BDF_Library.BDF_Header header = BDF_Library.Output_Holder.BDFHeaderHolder;
            //clear and create the relevant lists
            string NSig = Convert.ToString(ChannelLength);
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
            for (int i = 0; i < numSignals; i++)
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
            header.setNSDependantData(labels, transducerTypes, physicalDimensions, physicalMinimums, physicalMaximums, digitalMinimums, digitalMaximums, prefilterings, numSamplesPerRecords);

            byte Start = 255;
            BinaryWriter Begin = new BinaryWriter(File.Open(_bdf_temp, FileMode.Create));
            Begin.Write(Start);
            Begin.Close();

            StreamWriter writer = new StreamWriter(_bdf_temp, true);
            writer.Write(header.generateBDFHeader());
            writer.Close();
        }

        bool SaveSignalAsEDF(string FileName)
        {
            Create_EDFHeader();
            EDF_Library.EDF_DataManager EDFmanager = new EDF_Library.EDF_DataManager();
            EDF_Library.ObjectHolder.EDFHeaderHolder = EDFmanager.AddFile(_savebuf_filename, EDF_Library.ObjectHolder.EDFHeaderHolder);
            EDFmanager.generateEDFData(EDF_Library.ObjectHolder.EDFHeaderHolder, _edf_temp);
            File.Copy(_edf_temp, FileName, true);
            return true;
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

            if (Filename.EndsWith("edf", true, null))
            {
                try
                {
                    result = SaveSignalAsEDF(Filename);
                }
                catch (System.Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else if (Filename.EndsWith("bdf", true, null))
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
            else if (Filename.EndsWith("m41", true, null))
            {
                try
                {
                    result = SaveSignalAsM41(Filename);
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
