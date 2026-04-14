using MongoDB.Driver;

namespace Habit_Tracker
{
    public class MongoDBConnection
    {
        private static string connectionString = "mongodb+srv://admin:Test123@cluster0.qj6ei8h.mongodb.net/HabitTrackerDB?retryWrites=true&w=majority";

        public static IMongoDatabase GetDatabase()
        {
            var client = new MongoClient(connectionString);
            return client.GetDatabase("HabitTrackerDB");
        }
    }
}