using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPO.Services;
using TRPO.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TRPO.Database
{
    internal class RouteDB
    {
       
        public static List<Route> getAllRoutes()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM Route", DataBase.getInstance().getConnection());
            return getFlightsByCommand(command);
        }
      

        public void SaveUserToDB(Route route)
        {
            string commandExpression = "INSERT [Route] (StartPoint, FinishPoint, Distance)" +
                " VALUES (@StartPoint, @FinishPoint, @Distance)"; 
            SqlCommand command = new SqlCommand(commandExpression, DataBase.getInstance().getConnection());
            command.Parameters.Add("@StartPoint", System.Data.SqlDbType.NVarChar, 50).Value = route.StartPoint;
            command.Parameters.Add("@FinishPoint", System.Data.SqlDbType.NVarChar, 50).Value = route.FinishPoint;
            command.Parameters.Add("@Distance", System.Data.SqlDbType.Int).Value = route.Distance;
            command.ExecuteNonQuery();
            DataBase.getInstance().closeConnection();
        }
        private static Route GetFromDBByCommand(SqlCommand command)
        {
            DataRow[] routeInfo;
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(table);
            routeInfo = table.Select();
            if (routeInfo.Length > 0)
            {
                return new Route(
                    id: Convert.ToInt32(routeInfo[0][0]),
                    startPoint: Convert.ToString(routeInfo[0][1]),
                    finishPoint: Convert.ToString(routeInfo[0][2]),
                    distance: Convert.ToInt32(routeInfo[0][3])
                );
            }
            else return null;
        }
        public static Route GetFromDBById(Route id)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM [Route] WHERE Route_id = @id", DataBase.getInstance().getConnection());
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
            return GetFromDBByCommand(command);
        }


        private static List<Route> getFlightsByCommand(SqlCommand command)
        {
            List<Route> tmpRoutes = new List<Route>();
            DataBase.getInstance().openConnection();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string startPoint = reader.GetString(1);
                    string finishPoint = reader.GetString(2);
                    int distance = reader.GetInt32(3);

                    tmpRoutes.Add(new Route(id, startPoint, finishPoint, distance));
                }
            }
            DataBase.getInstance().closeConnection();
            return tmpRoutes;
        }
    }
}
