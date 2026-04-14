using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Habit_Tracker
{
    public class Habit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Username { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime Date { get; set; }

        public DateTime CompletedDate { get; set; }

        public int Streak { get; set; }
    }
}