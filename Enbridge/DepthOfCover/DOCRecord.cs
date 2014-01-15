using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.DepthOfCover
{
    [Serializable]
    public class DOCRecord
    {
        public string createdBy;
        public DateTime createdDate;
        public double measurement;
        public double X;
        public double Y;
        public double Z;

        public DOCRecord(string createdBy, DateTime createdDate, double X, double Y, double Z)
        {
            this.createdBy = createdBy;
            this.createdDate = createdDate;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }


  
}
