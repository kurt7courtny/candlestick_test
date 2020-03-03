using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace candlestick_test
{
    public partial class Form1 : Form
    {
        private instrument_data id_series;

        public Form1()
        {
            InitializeComponent();
            id_series = new instrument_data();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "csv files (*.csv)|*.csv",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fn = openFileDialog1.FileName;
                instrument_data new_id = new instrument_data();
                if (new_id.try_parse(fn))
                {
                    id_series = new_id;
                    label1.Text = new_id.instrument_name;
                    draw_candle();
                }
                else
                {
                    label1.Text = "UNknown";
                }
            }
        }

        private void draw_candle()
        {
            chart1.Series.Clear();
            Series price = new Series("price");
            chart1.Series.Add(price);
            chart1.Series["price"].ChartType = SeriesChartType.Candlestick;
            // Set point width
            chart1.Series["price"]["PointWidth"] = "1.0";
            chart1.Series["price"]["PriceUpColor"] = "Red";
            chart1.Series["price"]["PriceDownColor"] = "Green";
            chart1.Series["price"].XValueType = ChartValueType.DateTime;
            //for (int i = 0; i < id_series.candle_series.Count; i++)
            for (int i = 0; i < 10; i++)
            {
                var cd = (candle_data)id_series.candle_series[i];
                // adding date and high
                chart1.Series["price"].Points.AddXY(cd.dt, cd.high);
                // adding low
                chart1.Series["price"].Points[i].YValues[1] = cd.low;
                //adding open
                chart1.Series["price"].Points[i].YValues[2] = cd.open;
                // adding close
                chart1.Series["price"].Points[i].YValues[3] = cd.close;
            }

            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;   //此为解决Y轴自适应
        }
    }
}
