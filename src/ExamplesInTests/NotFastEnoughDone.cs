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
			// We provide bird strike data services and we want to grow our subscriber base by 10% in next year
			// Assume same 10% load we have now
			// Sequential, assume we don't have much down time, neglible, 100% one worker handling requests
			// Currently ~ X seconds, need (X/1.1) to be sufficient, maybe X/1.2 is ideal
			var lines = File.ReadAllLines("Bird Strikes Big.csv");

			var strikesByAirline = lines.Skip(1)
				.AsParallel()
				.Select(ParseStrike)
				.GroupBy(s => s.Airline)
				.ToDictionary(a => a.Key, a => a.Count());
			foreach (var airline in strikesByAirline.OrderByDescending(s => s.Value))
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
}