using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spline.Core;

namespace Spline.Test
{
	public class Parser
	{
		public static List<DataItem> GetDateItems(string fileName)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException(fileName);

			var buf = new List<DataItem>(6048);

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
						var item = new DataItem();
						item.Date = column[k++];
						item.Time = column[k++];
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
