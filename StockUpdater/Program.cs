using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace StockUpdater
{
    class Program
    {
        private static ConnectionMultiplexer redis;
        private static IDatabase db;
        private static List<string> stockNames;

        static void Main(string[] args)
        {
            
            redis = ConnectionMultiplexer.Connect("localhost");
            db = redis.GetDatabase();

            stockNames = GenerateStockNames(20);
            var random = new Random();

            Console.WriteLine("Uploading initial stock names with random prices.\n");

            UploadStockData(random);

            Console.WriteLine("Initial stocks and prices uploaded.\n");

            Timer timer = new Timer(UpdateStockData, random, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1));

            Console.ReadLine();
        }

        static List<string> GenerateStockNames(int count)
        {
            var stockNames = new List<string>();
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                var stockName = "STOCK" + random.Next(10, 100);
                stockNames.Add(stockName);
            }
            return stockNames;
        }
        
        static void UploadStockData(Random random)
        {
            foreach (var stock in stockNames)
            {
                var price = random.Next(100, 1000);
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                db.HashSet(stock, new HashEntry[]
                {
                    new HashEntry("Price", price),
                    new HashEntry("LastUpdated", timestamp)
                });

                Console.WriteLine($"{stock}: Rs.{price} at {timestamp} \n");
            }
        }

        static void UpdateStockData(object state)
        {
            Console.WriteLine("Updating stock prices.\n");
            var random = (Random)state;
            UploadStockData(random);

            

        }
    }
}

