using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPSA41CH.BDF_Library
{
    public class BDF_DataRecordSignalTrigger
    {
        public Int16 Trigger { get; set; }
        public BDF_DataRecordSignalTrigger()
        {

        }
        public BDF_DataRecordSignalTrigger(Int16 Trigger)
        {
            this.Trigger = Trigger;
        }
    }
}
