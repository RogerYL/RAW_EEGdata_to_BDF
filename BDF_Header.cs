using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPSA41CH.BDF_Library
{
    public class BDF_Header
    {
        //Add constants that define how many chars each field has
        private const int NUM_CHARS_VERSION = 7;
        private const int NUM_CHARS_PATIENT = 80;
        private const int NUM_CHARS_RECORDING = 80;
        private const int NUM_CHARS_START_DATE = 8;
        private const int NUM_CHARS_START_TIME = 8;
        private const int NUM_CHARS_NUM_BYTES = 8;
        private const int NUM_CHARS_RESERVED = 44;
        private const int NUM_CHARS_DATA_RECORDS = 8;
        private const int NUM_CHARS_DURATION_RECORD = 8;
        private const int NUM_CHARS_NS = 4;

        private const int NUM_CHARS_LABELS = 16;
        private const int NUM_CHARS_TRANSDUCER_TYPE = 80;
        private const int NUM_CHARS_PHYSICAL_DIMENSION = 8;
        private const int NUM_CHARS_PHYSICAL_MINIMUM = 8;
        private const int NUM_CHARS_PHYSICAL_MAXIMUM = 8;
        private const int NUM_CHARS_DIGITAL_MINUMUM = 8;
        private const int NUM_CHARS_DIGITAL_MAXIMUM = 8;
        private const int NUM_CHARS_PREFILTERING = 80;
        private const int NUM_CHARS_NUM_SAMPLES_PER_RECORD = 8;
        private const int NUM_CHARS_RESERVED_2 = 32;

        //basic info
        public string bdfVersion { get; private set; } //"255" (non ascii)  "BIOSEMI" (ASCII) by default
        public string localPatientIdent { get; private set; } //patient identification
        public string localRecordingIdent { get; private set; } //equipment used, technician name, etc, check spec
        public string startDate { get; private set; } //start date of record
        public string startTime { get; private set; } //start time of record
        public string reserved { get; private set; } //"24BIT" (ASCII)
        public string numRecords { get; private set; } //-1 if unknown
        public string durationRecord { get; private set; } //duration of one record in seconds
        public string ns { get; private set; } //number of signals in one data record

        //info with size dependent on ns
        public List<string> labels { get; private set; }
        public List<string> transducerTypes { get; private set; }
        public List<string> physicalDimensions { get; private set; }
        public List<string> physicalMinimums { get; private set; }
        public List<string> physicalMaximums { get; private set; }
        public List<string> digitalMinimums { get; private set; }
        public List<string> digitalMaximums { get; private set; }
        public List<string> prefilterings { get; private set; }
        public List<string> numSamplesPerRecords { get; private set; }

        //path info
        public string FilePath { get; set; }

        public List<BDF_Signal> bdfSignals;

        string reserved2;

        string numbytes; //calculate last -> number of bytes in header

        // Use this construtor to initialise the base object BDF_Header. To complete it, call upon setNSDependantData
        public BDF_Header(string iEdfVersion, string iLocalPatientIdent, string iLocalRecordingIdent, string iStartDate, string iStartTime,
                         string iReserved, string iNumRecords, string iDurationRecord, string iNs)
        {
            setVersion(iEdfVersion);
            setLocalPatientIdent(iLocalPatientIdent);
            setLocalRecordingIdent(iLocalRecordingIdent);
            setStartDate(iStartDate);
            setStartTime(iStartTime);
            setReserved(iReserved);
            setNumRecords(iNumRecords);
            setDurationRecord(iDurationRecord);
            setNs(iNs);
        }

        // This function completes all the data required for the header. Call generateBDFHeader to get the completed header
        public void setNSDependantData(List<string> iLabels, List<string> iTransducerTypes, List<string> iPhysicalDimensions,
                                        List<string> iPhysicalMinimums,
                                        List<string> iPhysicalMaximums, List<string> iDigitalMinimums, List<string> iDigitalMaximums, List<string> iPrefilterings,
                                        List<string> iNumSamplesPerRecords)
        {
            //Initialise all advanced header field lists
            labels = new List<string>();
            transducerTypes = new List<string>();
            physicalDimensions = new List<string>();
            physicalMinimums = new List<string>();
            physicalMaximums = new List<string>();
            digitalMinimums = new List<string>();
            digitalMaximums = new List<string>();
            prefilterings = new List<string>();
            numSamplesPerRecords = new List<string>();

            //initialise EDF Signal list
            bdfSignals = new List<BDF_Signal>();

            //copy from the lists to the header fields
            setLabels(iLabels);
            settransducerTypes(iTransducerTypes);
            setphysicalDimensions(iPhysicalDimensions);
            setphysicalMinimums(iPhysicalMinimums);
            setphysicalMaximums(iPhysicalMaximums);
            setdigitalMinimums(iDigitalMinimums);
            setdigitalmaximums(iDigitalMaximums);
            setprefilterings(iPrefilterings);
            setnumSamplesPerRecords(iNumSamplesPerRecords);

            for (int i = 0; i < iLabels.Count; i++)
            {
                //add every signal to the application global list of signals
                BDF_Signal signal = new BDF_Signal();
                signal.label = iLabels[i];
                signal.transducerType = iTransducerTypes[i];
                signal.physicalDimension = iPhysicalDimensions[i];
                signal.physicalMinimum = iPhysicalMinimums[i];
                signal.physicalMaximum = iPhysicalMaximums[i];
                signal.digitalMinimum = iDigitalMinimums[i];
                signal.digitalMaximum = iDigitalMaximums[i];
                signal.preFiltering = iPrefilterings[i];
                signal.numSamples = iNumSamplesPerRecords[i];
                bdfSignals.Add(signal);
            }
        }

        // Returns final header
        public string generateBDFHeader()
        {
            string header = "";
            //initialise the numbytes block based on the formula 256 + 256 * ns
            numbytes = BDF_File.buildFile((Convert.ToString(256 + Convert.ToInt16(ns) * 256)), NUM_CHARS_NUM_BYTES);

            //build the end reserved block, 32 bytes * ns
            reserved2 = BDF_File.buildFile("", Convert.ToInt16(ns) * NUM_CHARS_RESERVED_2);

            //add basics
            header += bdfVersion + localPatientIdent + localRecordingIdent + startDate + startTime + numbytes + reserved + numRecords +
                      durationRecord + ns;

            //add advanced fields
            foreach (string x in labels)
            {
                header += x;
            }
            foreach (string x in transducerTypes)
            {
                header += x;
            }
            foreach (string x in physicalDimensions)
            {
                header += x;
            }
            foreach (string x in physicalMinimums)
            {
                header += x;
            }
            foreach (string x in physicalMaximums)
            {
                header += x;
            }
            foreach (string x in digitalMinimums)
            {
                header += x;
            }
            foreach (string x in digitalMaximums)
            {
                header += x;
            }
            foreach (string x in prefilterings)
            {
                header += x;
            }
            foreach (string x in numSamplesPerRecords)
            {
                header += x;
            }

            //add final reserved header block
            header += reserved2;

            return header;
        }

        //basic field setters
        private void setVersion(string iEdfVersion)
        {
            bdfVersion = BDF_File.buildFile(iEdfVersion, NUM_CHARS_VERSION);
        }
        private void setLocalPatientIdent(string iLocalPatientIdent)
        {
            localPatientIdent = BDF_File.buildFile(iLocalPatientIdent, NUM_CHARS_PATIENT);
        }
        private void setLocalRecordingIdent(string iLocalRecordingIdent)
        {
            localRecordingIdent = BDF_File.buildFile(iLocalRecordingIdent, NUM_CHARS_RECORDING);
        }
        private void setStartDate(string iStartDate)
        {
            startDate = BDF_File.buildFile(iStartDate, NUM_CHARS_START_DATE);
        }
        private void setStartTime(string iStartTime)
        {
            startTime = BDF_File.buildFile(iStartTime, NUM_CHARS_START_TIME);
        }
        private void setReserved(string iReserved) //special setter, ensure than reserved falls within the accepted fields
        {
            reserved = BDF_File.buildFile(iReserved, NUM_CHARS_RESERVED);
        }
        private void setNumRecords(string iNumRecords = "-1") //-1 signifies unknwon
        {
            numRecords = BDF_File.buildFile(iNumRecords, NUM_CHARS_DATA_RECORDS);
        }
        private void setDurationRecord(string iDurationRecord)
        {
            durationRecord = BDF_File.buildFile(iDurationRecord, NUM_CHARS_DURATION_RECORD);
        }
        private void setNs(string iNs)
        {
            ns = BDF_File.buildFile(Convert.ToString(Convert.ToInt16(iNs)), NUM_CHARS_NS);
        }

        //advanced setters
        private void setLabels(List<string> iLabels)
        {
            if (iLabels.Count == Convert.ToInt16(ns))
            {
                foreach (string label in iLabels)
                {
                    labels.Add(BDF_File.buildFile(label, NUM_CHARS_LABELS));
                }
            }

        }
        private void settransducerTypes(List<string> itransducerTypes)
        {
            if (itransducerTypes.Count == Convert.ToInt16(ns))
            {
                foreach (string transducerType in itransducerTypes)
                {
                    transducerTypes.Add(BDF_File.buildFile(transducerType, NUM_CHARS_TRANSDUCER_TYPE));
                }
            }

        }
        private void setphysicalDimensions(List<string> iphysicalDimensions)
        {
            if (iphysicalDimensions.Count == Convert.ToInt16(ns))
            {
                foreach (string physicalDimension in iphysicalDimensions)
                {
                    physicalDimensions.Add(BDF_File.buildFile(physicalDimension, NUM_CHARS_PHYSICAL_DIMENSION));
                }
            }

        }
        private void setphysicalMinimums(List<string> iphysicalMinimums)
        {
            if (iphysicalMinimums.Count == Convert.ToInt16(ns))
            {
                foreach (string physicalMinimum in iphysicalMinimums)
                {
                    physicalMinimums.Add(BDF_File.buildFile(physicalMinimum, NUM_CHARS_PHYSICAL_MINIMUM));
                }
            }

        }
        private void setphysicalMaximums(List<string> iphysicalmaximums)
        {
            if (iphysicalmaximums.Count == Convert.ToInt16(ns))
            {
                foreach (string physicalmaximum in iphysicalmaximums)
                {
                    physicalMaximums.Add(BDF_File.buildFile(physicalmaximum, NUM_CHARS_PHYSICAL_MAXIMUM));
                }
            }

        }
        private void setdigitalMinimums(List<string> idigitalMinimums)
        {
            if (idigitalMinimums.Count == Convert.ToInt16(ns))
            {
                foreach (string digitalMinimum in idigitalMinimums)
                {
                    digitalMinimums.Add(BDF_File.buildFile(digitalMinimum, NUM_CHARS_DIGITAL_MINUMUM));
                }
            }

        }
        private void setdigitalmaximums(List<string> idigitalmaximums)
        {
            if (idigitalmaximums.Count == Convert.ToInt16(ns))
            {
                foreach (string digitalmaximum in idigitalmaximums)
                {
                    digitalMaximums.Add(BDF_File.buildFile(digitalmaximum, NUM_CHARS_DIGITAL_MAXIMUM));
                }
            }

        }
        private void setprefilterings(List<string> iprefilterings)
        {
            if (iprefilterings.Count == Convert.ToInt16(ns))
            {
                foreach (string prefiltering in iprefilterings)
                {
                    prefilterings.Add(BDF_File.buildFile(prefiltering, NUM_CHARS_PREFILTERING));
                }
            }

        }
        private void setnumSamplesPerRecords(List<string> inumSamplesPerRecords)
        {
            if (inumSamplesPerRecords.Count == Convert.ToInt16(ns))
            {
                foreach (string numSamplesPerRecord in inumSamplesPerRecords)
                {
                    numSamplesPerRecords.Add(BDF_File.buildFile(numSamplesPerRecord, NUM_CHARS_NUM_SAMPLES_PER_RECORD));
                }
            }

        }

    }
}
