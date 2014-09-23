namespace ExamplesInTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using NUnit.Framework;

	public class NotFastEnough
	{
		[Test]
		public void BirdStrikes()
		{
			// Because we provide bird strike data services and we want to grow our subscriber base
			// Currently ~ 12 seconds, needs to be 8 seconds, cut 1/3 of time
			var lines = File.ReadAllLines("Bird Strikes Big.csv");

			var strikesByAirline = lines.Skip(1)
				.Select(ParseStrike)
				.GroupBy(s => s.Airline)
				.ToDictionary(a => a.Key, a => a.Count());
			foreach (var airline in strikesByAirline)
			{
				Console.WriteLine("{0}: {1}", airline.Key, airline.Value);
			}
		}

		public Strike ParseStrike(string line)
		{
			var split = SplitCsvLine(line);
			return new Strike
			{
				Airline = split[2]
			};
		}

		public List<string> SplitCsvLine(string s)
		{
			int i;
			var a = 0;
			var count = 0;
			var str = new List<string>();
			for (i = 0; i < s.Length; i++)
			{
				switch (s[i])
				{
					case ',':
						if ((count & 1) == 0)
						{
							str.Add(s.Substring(a, i - a));
							a = i + 1;
						}
						break;
					case '"':
						count++;
						break;
				}
			}
			str.Add(s.Substring(a));

			// evil, show removing this later to show that Parallel can't eliminate slowness from contention
			for (i = 0; i < 1000; i++)
			{
				a = i;
			}
			str.Add(a.ToString());

			return str;
		}

		[Test, Ignore]
		public void MakeBigDataSet()
		{
			var lines = File.ReadAllLines("Bird Strikes.csv");
			File.WriteAllLines("Bird Strikes Big.csv", lines);
			for (var i = 0; i < 19; i++)
			{
				File.AppendAllLines("Bird Strikes Big.csv", lines);
			}
		}

		// Hints: Use PLINQ and AsParallel to speed up
	}

	public class Strike
	{
		public string Airline { get; set; }
	}
}