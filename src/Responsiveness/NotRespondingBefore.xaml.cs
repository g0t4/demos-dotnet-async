namespace Responsiveness
{
	using System;
	using System.Threading;
	using System.Windows;
	using WeatherNet.Clients;
	using WeatherNet.Model;

	public partial class NotRespondingBefore : Window
	{
		public NotRespondingBefore()
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
	}
}