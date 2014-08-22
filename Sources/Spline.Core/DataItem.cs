using System;

namespace Spline.Core
{
	public class DataItem
	{
		public DateTime DateTime
		{
			get
			{
				return DateTime.Parse(string.Format("{0} {1}:00", Date, Time));
			}
		}

		public string Date { get; set; }
		public string Time { get; set; }
		public double MaxPrice { get; set; }
		public double MinPrice { get; set; }
		public double Open { get; set; }
		public double Close { get; set; }
		public int Value { get; set; }
	}
}
