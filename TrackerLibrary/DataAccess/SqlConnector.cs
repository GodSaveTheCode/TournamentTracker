using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using Dapper;
using System.Data;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {

        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString("Tournaments")))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PlaceNumber", model.PlaceNumber);
                parameters.Add("@PlaceName", model.PlaceName);
                parameters.Add("@PrizeAmount", model.PrizeAmount);
                parameters.Add("@PrizePercentage", model.PrizePercentage);
                parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", parameters, commandType: CommandType.StoredProcedure);

                model.Id = parameters.Get<int>("@id");

                return model;
            }
        }
    }
}
