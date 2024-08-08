using Dapper;
using GameTournament.MVVM.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GameTournament.Helpers
{
    public class DataAccess
    {
        public List<Player> GetPlayers()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.CnnString("PES2013")))
            {
                return connection.Query<Player>("Use PES2013;\n" +
                    "select * from Players;").ToList();
            }
        }

        public List<Team> GetTeams()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.CnnString("PES2013")))
            {
                return connection.Query<Team>("Use PES2013;\n" +
                    "select * from Teams;").ToList();
            }
        }

        public void AddPlayer(string newPlayer)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.CnnString("PES2013")))
            {
                connection.Query<Player>($"Use PES2013; INSERT INTO Players(Name) VALUES ('{newPlayer}');");
            }
        }

        public void AddTeam(string newTeam)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.CnnString("PES2013")))
            {
                connection.Query<Team>($"Use PES2013; INSERT INTO Teams(Name) VALUES ('{newTeam}');");
            }
        }


    }
}
