using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using System.IO;
using TrackerLibrary.DataAccess.Helpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        //TODO - make CreatePrize() actually work for text file connection.

        // получить данные из файла как List<string> перевести в List<PrizeModel>
        // получить id последнего приза в файле, чтоб узнать какой id задать новому призу
        // сохранить PrizeModel как List<string> в текстовый файл
        public const string PrizesFile = "Prizes.txt";

       

        public PrizeModel CreatePrize(PrizeModel model)
        {
           List<PrizeModel> prizes = PrizesFile.GetFullPath().LoadFile().ConvertToPrizeModels();

            int newId = 1;
            if (prizes.Count > 0)
            {
                newId = prizes.OrderByDescending(p => p.Id).First().Id + 1;
            }
            model.Id = newId;
            prizes.Add(model);
            prizes.SaveToPrizeFile(PrizesFile.GetFullPath());
            return model;
            
        }
    }
}
