using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;

namespace ContainerTestImage.Database
{
    public static class DbContextExtensions
    {
        public static string GetConnectionStringDataSource(this DbContext context)
        {
            try
            {
                var connection = context.Database.GetDbConnection();
                if (connection is SqlConnection sqlConnection)
                {
                    var connectionStringBuilder = new SqlConnectionStringBuilder(sqlConnection.ConnectionString);
                    return connectionStringBuilder.DataSource;
                }
                return "Unknown data source";
            }
            catch (Exception)
            {
                return "Unable to retrieve data source";
            }
        }
    }
}
