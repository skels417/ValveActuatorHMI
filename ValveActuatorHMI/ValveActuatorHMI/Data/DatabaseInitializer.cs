using System.Data.Entity;
using System.Linq;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AppDbContext>());

            using (var context = new AppDbContext())
            {
                try
                {
                    // Принудительно создаем базу данных, если ее нет
                    if (!context.Database.Exists())
                    {
                        context.Database.Create();
                    }

                    // Проверяем наличие таблицы
                    if (!TableExists(context, "DeviceConfigurations"))
                    {
                        context.Database.ExecuteSqlCommand(@"
                            CREATE TABLE DeviceConfigurations (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                DeviceType TEXT NOT NULL,
                                ConfigurationJson TEXT NOT NULL
                            )");
                    }

                    // Инициализация данных
                    if (!context.DeviceConfigurations.Any())
                    {
                        context.DeviceConfigurations.Add(new DeviceConfiguration
                        {
                            DeviceType = "Default",
                            ConfigurationJson = "{}"
                        });
                        context.SaveChanges();
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                    // Не пробрасываем исключение, чтобы приложение могло запуститься
                    // throw;
                }
            }
        }

        private static bool TableExists(AppDbContext context, string tableName)
        {
            var connection = context.Database.Connection;
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                return cmd.ExecuteScalar() != null;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}