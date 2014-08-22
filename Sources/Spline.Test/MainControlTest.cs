using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Spline.Core;
using Spline.Core.KOA;
using ZedGraph;
using Point = Spline.Core.Point;

namespace Spline.Test
{
	public partial class MainControlTest : Form
	{
		private List<DataItem> _set;

		public MainControlTest()
		{
			InitializeComponent();
		}

		private void BtnLoadClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(tbLoad.Text))
			{
				openFileDialog1.Multiselect = false;
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					tbLoad.Text = openFileDialog1.FileName;
				}
			}

			if (File.Exists(tbLoad.Text))
			{
				_set = Parser.GetDateItems(tbLoad.Text);
				tbReload.Text = _set.Count.ToString();
				Drove(_set, _set.Count);
			}
		}

		private void Drove(List<DataItem> set, int count)
		{
			var settings = new ZedGHelper.Settings();
			settings.Type = SymbolType.Circle;
			ZedGHelper.CleanAll(ZedGraphControl,true);
			ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => new Point(o.DateTime.Ticks, o.Open)), false);
			//ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => new Point(o.DateTime.Ticks, o.Close)), false);
			//ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => new Point(o.DateTime.Ticks, o.MaxPrice)), false);
			//ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => new Point(o.DateTime.Ticks, o.MinPrice)), false);
			//ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => new Point(o.DateTime.Ticks, o.Value)), true);

			//settings.GrafikColor = Color.Red;
			//settings.Type = SymbolType.Diamond;
			//var ys = BSpline.SplinePoints(set.Select(o => o.Open).ToArray(), 60);
			//ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => (double)o.DateTime.Ticks).ToArray(), ys, false);

			//settings.GrafikColor = Color.Green;
			//settings.Type = SymbolType.Square;
			//ys = BSpline.SplinePoints(set.Select(o => o.Open).ToArray(), 300);
			//ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => (double)o.DateTime.Ticks).ToArray(), ys, false);

			var t = count;

			settings.GrafikColor = Color.Purple;
			settings.Type = SymbolType.Diamond;
			var ys = BSpline.SplinePoints(set.Take(t).Select(o => o.Open).ToArray(), 60);
			ZedGHelper.Paint(ZedGraphControl, settings, set.Take(t).Select(o => (double)o.DateTime.Ticks).ToArray(), ys, false);

			settings.GrafikColor = Color.Blue;
			settings.Type = SymbolType.Square;
			ys = BSpline.SplinePoints(set.Take(t).Select(o => o.Open).ToArray(), 2000);
			ZedGHelper.Paint(ZedGraphControl, settings, set.Take(t).Select(o => (double)o.DateTime.Ticks).ToArray(), ys, false);
			ZedGraphControl.Refresh();
		}

		private void BtReloadClick(object sender, EventArgs e)
		{
			var t = int.Parse(tbReload.Text);
			Drove(_set, t);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var t = int.Parse(tbReload.Text);
			t--;
			tbReload.Text = t.ToString();
			Drove(_set, t);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var t = int.Parse(tbReload.Text);
			t++;
			tbReload.Text = t.ToString();
			Drove(_set, t);
		}
	}
}
