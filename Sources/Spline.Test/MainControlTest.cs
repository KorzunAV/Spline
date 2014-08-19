using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Lib.Graphic.ZedGraph;
using Spline.Core;
using ZedGraph;
using Point = Spline.Core.Point;

namespace Spline.Test
{
	public partial class MainControlTest : Form
	{
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
				var set = Parser.GetDateItems(tbLoad.Text);

				var settings = new ZedGHelper.Settings();
				settings.Type = SymbolType.Circle;
				ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => new Point(o.DateTime.Ticks, o.Open)));

				settings.GrafikColor = Color.Red;
				settings.Type = SymbolType.Diamond;
				var ys = BSpline.SplinePoints(set.Select(o => o.Open).ToArray());
				ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => (double)o.DateTime.Ticks).ToArray(), ys);

				settings.GrafikColor = Color.Green;
				settings.Type = SymbolType.Square;
				ys = BSpline.SplinePoints(set.Select(o => o.Open).ToArray(), 10);
				ZedGHelper.Paint(ZedGraphControl, settings, set.Select(o => (double)o.DateTime.Ticks).ToArray(), ys);
			}
		}
	}
}
