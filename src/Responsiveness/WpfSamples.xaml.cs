namespace Responsiveness
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using WeatherNet.Clients;
	using WeatherNet.Model;

	public partial class WpfSamples : Window
	{
		private Stopwatch _Timer;
		private const string KansasCity = "Kansas City";
		private const string Seattle = "Seattle";
		private const string NewYork = "New York";

		public WpfSamples()
		{
			InitializeComponent();
		}

		private void WeatherBlockingClick(object sender, RoutedEventArgs e)
		{
			ResetTimer();
			var weather = GetWeather(KansasCity);
			ShowWeather(weather);
		}

		private static CurrentWeatherResult GetWeather(string city)
		{
			Thread.Sleep(2000); //slow it down if too fast
			return CurrentWeather.GetByCityName(city, "USA", string.Empty, string.Empty).Item;
		}

		private void WeatherTplClick(object sender, RoutedEventArgs e)
		{
			ResetTimer();
			Task.Factory.StartNew(() => GetWeather(KansasCity))
				.ContinueWith(t => ShowWeather(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
		}

		private async void WeatherAsyncClick(object sender, RoutedEventArgs e)
		{
			ResetTimer();
			var weather = await Task.Factory.StartNew(() => GetWeather(KansasCity));
			ShowWeather(weather);
		}

		private void ShowWeather(params CurrentWeatherResult[] weathers)
		{
			foreach (var weather in weathers)
			{
				var readings = new { weather.City, weather.Date, weather.Temp, weather.Description };
				TextBox.Text += readings + Environment.NewLine;
			}
			Status.Text = "Duration: " + _Timer.Elapsed;
		}

		/// <summary>
		///     Demonstrate the idea of Async APIs
		/// </summary>
		private Task<CurrentWeatherResult> GetWeatherAsync()
		{
			return Task.Factory.StartNew(() => GetWeather(KansasCity));
		}

		private void WeatherMultipleBlockingClick(object sender, RoutedEventArgs e)
		{
			ResetTimer();
			var kc = GetWeather(KansasCity);
			ShowWeather(kc);
			var seattle = GetWeather(Seattle);
			ShowWeather(seattle);
			var ny = GetWeather(NewYork);
			ShowWeather(ny);
		}

		private void WeatherMultipleTplClick(object sender, RoutedEventArgs e)
		{
			ResetTimer();
			Task.Factory.StartNew(() => GetWeather(KansasCity))
				.ContinueWith(t => ShowWeather(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
			Task.Factory.StartNew(() => GetWeather(Seattle))
				.ContinueWith(t => ShowWeather(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
			Task.Factory.StartNew(() => GetWeather(NewYork))
				.ContinueWith(t => ShowWeather(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
		}

		private async void WeatherMultipleAsyncClick(object sender, RoutedEventArgs e)
		{
			ResetTimer();
			var kc = Task.Factory.StartNew(() => GetWeather(KansasCity));
			var seattle = Task.Factory.StartNew(() => GetWeather(Seattle));
			var ny = Task.Factory.StartNew(() => GetWeather(NewYork));

			ShowWeather(await kc);
			ShowWeather(await seattle);
			ShowWeather(await ny);
		}

		private void WeatherParallelClick(object sender, RoutedEventArgs routedEventArgs)
		{
			ResetTimer();
			Task.Factory.StartNew(() =>
			{
				var cities = new[] {KansasCity, Seattle, NewYork};
				Parallel.ForEach(cities, new ParallelOptions {MaxDegreeOfParallelism = 2}, city =>
				{
					var weather = GetWeather(city);
					Dispatcher.InvokeAsync(() => ShowWeather(weather));
				});
			});
		}

		private void WeatherPlinqClick(object sender, RoutedEventArgs routedEventArgs)
		{
			ResetTimer();
			Task.Factory.StartNew(() =>
			{
				var cities = new[] {KansasCity, Seattle, NewYork};
				var weathers = (from city in cities.AsParallel().WithDegreeOfParallelism(2)
					select GetWeather(city)).ToArray();
				Dispatcher.InvokeAsync(() => ShowWeather(weathers));
			});
		}

		private void ResetTimer()
		{
			Status.Text = "Running";
			_Timer = Stopwatch.StartNew();
		}
	}
}