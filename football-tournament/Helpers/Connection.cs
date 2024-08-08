using System.Configuration;

namespace GameTournament.Helpers
{
    public static class Connection
    {
        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
