using Npgsql;

namespace BF_Host.Services
{
    public abstract class BaseService
    {
        // Conect from docker container
        protected NpgsqlConnectionStringBuilder connStr;
        protected NpgsqlConnection nc;

        protected BaseService()
        {
            // Conect from docker container
            connStr = new NpgsqlConnectionStringBuilder("Server = host.docker.internal; Database = bf_test; Port = 5432; User Id = Lena; Password = ;");


            // Open SQL        
            nc = new NpgsqlConnection(connStr.ToString());
        }
    }
}
