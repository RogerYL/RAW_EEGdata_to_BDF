using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPSA41CH.BDF_Library
{
    public class BDF_DataRecordSignalSample
    {
        public byte[] sample { get; set; }
        public BDF_DataRecordSignalSample()
        {

        }
        public BDF_DataRecordSignalSample(byte[] sample)
        {
            this.sample = sample;
        }
    }
}
