using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        //TODO - make CreatePrize() actually work for text file connection.
        public PrizeModel CreatePrize(PrizeModel model)
        {
            model.Id = 1;
            return model;
        }
    }
}
