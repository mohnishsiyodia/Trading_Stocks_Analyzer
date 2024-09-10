using Stock_Update_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using StackExchange.Redis;
using System.Threading;
using System.Windows.Threading;
using System.Security.Cryptography.X509Certificates;

namespace Stock_Update_WPF.ViewModels
{
    public class StockViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Stock> stocks;
        private DispatcherTimer timer;
        private IDatabase db;
        private ConnectionMultiplexer redis;
        private DateTime today = DateTime.MinValue;
        private Dictionary<string, double> openingPrice = new Dictionary<string,double>();
        private Dictionary<string, DateTime> openingPriceDate = new Dictionary<string, DateTime>();

        public ObservableCollection<Stock> Stocks
        {
            get { return stocks; }
            set
            {
                stocks = value;
                OnPropertyChanged("Stocks");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public StockViewModel()
        {
            //Stocks = new ObservableCollection<Stock>();

            //redis = ConnectionMultiplexer.Connect("localhost");
            //db = redis.GetDatabase();

            //LoadStockData();

            //timer = new Timer(5000);
            //timer.Elapsed += Timer_Elapsed;
            //timer.Start();
            redis = ConnectionMultiplexer.Connect("localhost");
            db = redis.GetDatabase();
            stocks = new ObservableCollection<Stock>();

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0) 
            };
            timer.Tick += (sender, args) => LoadStockData();
            timer.Start();
            LoadStockData();
        }


        private void LoadStockData()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Stocks.Clear();

                var keys = redis.GetServer("localhost", 6379).Keys();

                foreach (var key in keys)
                {
                    var stockName = key.ToString();

                    if (!stockName.EndsWith("_history")) // Ignoring history keys
                    {
                        var price = db.HashGet(stockName, "Price");
                        var lastUpdated = db.HashGet(stockName, "LastUpdated");
                        
                        if (!price.IsNullOrEmpty && !lastUpdated.IsNullOrEmpty)
                        {
                            if (!today.ToShortDateString().Equals(GetStockDate(stockName)))
                            {
                                today = DateTime.Now;
                                openingPrice.Add(stockName, double.Parse(price));
                                openingPriceDate.Add(stockName, today);
                                UpdateStocks(stockName, price, lastUpdated);
                            }
                            else
                            {
                                UpdateStocks(stockName, price, lastUpdated);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Stock {stockName} has null or empty values.");
                        }
                    }
                }
            });
        }

        private void UpdateStocks(string stockName, RedisValue price, RedisValue lastUpdated)
        {
            Stocks.Add(new Stock
            {
                StockName = stockName,
                OpenPrice = openingPrice.ContainsKey(stockName) ? openingPrice[stockName] : 0,
                Price = double.Parse(price),
                LastUpdated = lastUpdated
            });
        }

        private string GetStockDate(string stockName)
        {
            //return openingPriceDate.ContainsKey(stockName) ? openingPriceDate[stockName].ToShortDateString() : null;
            if (openingPriceDate.ContainsKey(stockName))
            {
                return openingPriceDate[stockName].ToShortDateString();
            }
            else
            {
                return null;
            }
        }
    }
}
