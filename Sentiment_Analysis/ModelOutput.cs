using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment_Analysis
{
    public class ModelOutput
    {
        public bool PredictedLabel { get; set; } 
        public float Score { get; set; }         
    }
}
