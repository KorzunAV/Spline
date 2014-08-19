using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spline.Test
{
	public struct DateItem
	{
		public DateTime DateTime { get; set; }
		public double MaxPrice { get; set; }
		public double MinPrice { get; set; }
		public double Open { get; set; }
		public double Close { get; set; }
		public int Value { get; set; }
	}

	public class Parser
	{
		public static List<DateItem> GetDateItems(string fileName)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException(fileName);

			var buf = new List<DateItem>(6048);

			using (var sr = new StreamReader(fileName))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine();
					if (line != null)
					{
						var column = line.Split(new[] { '\t', ' ' });
						if (column.Count() != 7)
							throw new ArgumentNullException("Количество колонок не соответствует шаблону.");
						
						int k = 0;
						var item = new DateItem();
						item.DateTime = DateTime.Parse(string.Format("{0} {1}:00", column[k++], column[k++]));
						item.MinPrice = Double.Parse(column[k++]);
						item.MaxPrice = Double.Parse(column[k++]);
						item.Open = Double.Parse(column[k++]);
						item.Close = Double.Parse(column[k++]);
						item.Value = int.Parse(column[k]);
						buf.Add(item);
					}
				}
			}
			return buf;
		}
	}
}
