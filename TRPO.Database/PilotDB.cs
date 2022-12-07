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
    internal class PilotDB
    {
        
        public static List<Pilot> getAllPilots()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM Pilot", DataBase.getInstance().getConnection());

            return getPilotsByCommand(command);
        }
        private static List<Pilot> getPilotsByCommand(SqlCommand command)
        {
            List<Pilot> tmpFlights = new List<Pilot>();
            DataBase.getInstance().openConnection();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int pilotId = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string surname = reader.GetString(2);
                    tmpFlights.Add(new Pilot(pilotId, name, surname));
                }
                DataBase.getInstance().closeConnection();
            }

            return tmpFlights;
        }
        public void SaveUserToDB(Pilot pilot)
        {
            string commandExpression = "INSERT [Pilot] (Name, Surname)" +
                " VALUES (@Name, @Surname)";
            SqlCommand command = new SqlCommand(commandExpression, DataBase.getInstance().getConnection());
            command.Parameters.Add("@Role", System.Data.SqlDbType.NChar, 20).Value = pilot.Name;
            command.Parameters.Add("@Email", System.Data.SqlDbType.NChar, 20).Value = pilot.Surname;
            DataBase.getInstance().openConnection();
            command.ExecuteNonQuery();
            DataBase.getInstance().closeConnection();
        }
        public static Pilot GetFromDBById(Pilot id)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM [Pilot] WHERE Pilot_id = @id", DataBase.getInstance().getConnection());
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
            return GetFromDBByCommand(command);
        }
        private static Pilot GetFromDBByCommand(SqlCommand command)
        {
            DataRow[] pilotInfo;
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(table);
            pilotInfo = table.Select();
            if (pilotInfo.Length > 0)
            {
                return new Pilot(
                    pilotId: Convert.ToInt32(pilotInfo[0][0]),
                    name: Convert.ToString(pilotInfo[0][1]),
                    surname: Convert.ToString(pilotInfo[0][2])
                );
            }
            else return null;
        }
       
    }
}
