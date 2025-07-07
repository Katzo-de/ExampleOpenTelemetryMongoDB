using MongoDB.Driver;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ExampleOpenTelemetryMongoDB.Business
{
    public class WeatherForecastBusiness
    {
        private readonly IMongoCollection<WeatherForecast> _collection;
        private static readonly ActivitySource ActivitySource = new("MyAspNetService");
        private static readonly Meter Meter = new("MongoDB.Driver", "1.0.0");
        private static readonly Counter<long> MongoFindCounter = Meter.CreateCounter<long>("mongodb_find_count");

        public WeatherForecastBusiness(IMongoClient mongoClient)
        {
            var db = mongoClient.GetDatabase("testdb");
            _collection = db.GetCollection<WeatherForecast>("Weather");
        }

        public async Task<List<WeatherForecast>> GetForecastAsync()
        {
            using var activity = ActivitySource.StartActivity("mongodb.find.weather", ActivityKind.Client);
            activity?.SetTag("db.system", "mongodb");
            activity?.SetTag("db.operation", "find");
            await Task.Delay(500);
            MongoFindCounter.Add(1);
            await Task.Delay(500);
            var result = await _collection.Find(null).ToListAsync();

            activity?.SetTag("db.mongo.resultCount", result.Count);

            //activity.End();
            return result;
        }

        public async Task InsertForecastAsync(WeatherForecast forecast)
        {
            using var activity = ActivitySource.StartActivity("mongodb.insert.weather", ActivityKind.Client);
            activity?.SetTag("db.system", "mongodb");
            activity?.SetTag("db.operation", "insert");
            await _collection.InsertOneAsync(forecast);
            await Task.Delay(500);
            //span.End();
        }
    }
}
