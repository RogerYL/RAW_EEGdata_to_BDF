using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPSA41CH.BDF_Library
{
    class BDF_DataBlock
    {
        public int numRecords { get; set; }
        public BDF_DataRecord[] records;

        public BDF_DataBlock()
        {

        }

        public BDF_DataBlock(int numRecords, BDF_DataRecord[] records)
        {
            if (numRecords == records.Length)
            {
                this.numRecords = numRecords;
                this.records = records;
            }
            else
            {
                throw new ArgumentException("Expected " + numRecords + " records, got " + records.Length + "!");
            }
        }

        public byte[] getBDFData()
        {
            List<byte> outData = new List<byte>();
            for (int i = 0; i < records.Length; i++)
            {
                for (int j = 0; j < records[i].signals.Length; j++)
                {
                    for (int k = 0; k < records[i].signals[j].samples.Length; k++)
                    {
                        for (int n = 0; n < records[i].signals[j].samples[k].sample.Length; n++)
                        {
                            outData.Add(records[i].signals[j].samples[k].sample[n]);
                        }
                    }
                }
            }
            return outData.ToArray();
        }
    }
}
