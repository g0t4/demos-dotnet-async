namespace Responsiveness
{
	using System;
	using System.ComponentModel;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using WeatherNet.Clients;
	using WeatherNet.Model;

	/// <summary>
	///     Interaction logic for NotResponding.xaml
	/// </summary>
	public partial class NotResponding : Window
	{
		public NotResponding()
		{
			InitializeComponent();
		}

		private void GetWeather(object sender, RoutedEventArgs e)
		{
			var weather = GetWeather();
			var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
			Output.Text += readings + Environment.NewLine;
		}

		private static CurrentWeatherResult GetWeather()
		{
			Thread.Sleep(2000);
			return CurrentWeather.GetByCityName("Kansas City", "USA", string.Empty, string.Empty).Item;
		}

		private void GetWeatherBackgroundWorker1(object sender, RoutedEventArgs e)
		{
			var worker = new BackgroundWorker();
			worker.DoWork += (o, args) =>
			{
				var weather = GetWeather();
				var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
				Output.Text += readings + Environment.NewLine;
			};
			worker.RunWorkerAsync();
		}

		private void GetWeatherBackgroundWorker2(object sender, RoutedEventArgs e)
		{
			var worker = new BackgroundWorker();
			worker.DoWork += (o, args) => { args.Result = GetWeather(); };
			worker.RunWorkerCompleted += (o, args) =>
			{
				// completed event fired on UI thread
				var weather = (CurrentWeatherResult) args.Result;
				var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
				Output.Text += readings + Environment.NewLine;
			};
			worker.RunWorkerAsync();
		}

		private void GetWeatherQueueThread(object sender, RoutedEventArgs e)
		{
			// launching a thread - physical threads are expensive and really need to be managed
			var thread = new Thread(() =>
			{
				var weather = GetWeather();
				var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
				Dispatcher.InvokeAsync(() => Output.Text += readings + Environment.NewLine);
			});
			thread.Start();
		}

		private void GetWeatherQueueUserWorkItem(object sender, RoutedEventArgs e)
		{
			ThreadPool.QueueUserWorkItem(state =>
			{
				var weather = GetWeather();
				var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
				Dispatcher.InvokeAsync(() => Output.Text += readings + Environment.NewLine);
			});
		}

		private void GetWeatherTpl1(object sender, RoutedEventArgs e)
		{
			Task.Factory.StartNew(() =>
			{
				var weather = GetWeather();
				var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
				Output.Text += readings + Environment.NewLine;
			});
		}

		private void GetWeatherTpl2(object sender, RoutedEventArgs e)
		{
			Task.Factory.StartNew(() => GetWeather())
				.ContinueWith(getWeather =>
				{
					var weather = getWeather.Result;
					var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
					Output.Text += readings + Environment.NewLine;
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private async void GetWeatherAsyncAwait(object sender, RoutedEventArgs e)
		{
			var weather = await Task.Factory.StartNew(() => GetWeather());
			var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
			Output.Text += readings + Environment.NewLine;
		}

		private async void GetWeatherAsyncAwait2(object sender, RoutedEventArgs e)
		{
			var weather = await GetWeatherAsync();
			var readings = new {weather.City, weather.Date, weather.Temp, weather.Description};
			Output.Text += readings + Environment.NewLine;
		}

		private static Task<CurrentWeatherResult> GetWeatherAsync()
		{
			return Task.Factory.StartNew(() => GetWeather());
		}
	}
}