using Microsoft.Data.SqlClient;
using Serilog;
using System;

namespace DragonAcc.Service.Services
{
    public class DatabaseBackupService
    {
        private readonly string _connectionString;

        public DatabaseBackupService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void BackupDatabase()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    var command = new SqlCommand(@"
                        BACKUP DATABASE DragonAcc 
                        TO DISK = 'D:\\Backups\\DragonAcc_Backup.bak'
 
                        WITH FORMAT, MEDIANAME = 'MyBackup', NAME = 'Full Backup of DragonAcc';", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Backup completed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during backup: {ex.Message}");
                    Log.Error($"Error during backup: {ex.Message}");
                }
            }
        }
    }
}
