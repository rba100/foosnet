using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FoosNet.Vision.Webservice
{
    public class FoosVisionService : IFoosVisionService
    {
        public bool GetTableInuse()
        {
            return (new Random()).NextDouble() > 0.5;
        }
    }
}
