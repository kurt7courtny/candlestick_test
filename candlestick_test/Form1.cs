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
        private int min_candle_numbs = 1000;    // 最小数据量
        private int f_candle_numbs = 150;       // 前历史k线数量
        private int b_candle_numbs = 500;       // 后测试k线数据

        private bool play_state;
        private instrument_data my_instrument_data;
        private trades_all mytrades;
        private trade_single mytrade;
        private candle_data cd;
        private int pos;
        private Timer myTimer;        

        public Form1()
        {
            myTimer = new System.Windows.Forms.Timer();
            myTimer.Tick += new EventHandler(myTimer_Callback);
            InitializeComponent();
            play_state = false;

            my_instrument_data = new instrument_data();
            mytrades = new trades_all();
            mytrade = null;
            cd = new candle_data();
            pos = 0;

            comboBox1.Items.Add("1秒1根");
            comboBox1.Items.Add("1.5秒1根");
            comboBox1.Items.Add("2秒1根");
            comboBox1.SelectedIndex = 1;
            
            changeplaystate(play_state);
            //reset_timer();
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
                    my_instrument_data = new_id;
                    label1.Text = new_id.instrument_name;
                    init_candle();
                    mytrades = new trades_all();
                    pos = 0;
                    mytrade = null;
                    //draw_candle();
                }
                else
                {
                    label1.Text = "UNknown";
                }
            }
        }

        private void init_candle()
        {
            if(my_instrument_data.candle_series.Count > min_candle_numbs)
            {
                chart1.Series.Clear();
                
                Series price = new Series("price");
                //Series volumn = new Series("volumn");
                
                price.IsXValueIndexed = true;
                //volumn.IsXValueIndexed = true;
                chart1.Series.Add(price);
                //chart1.Series.Add(volumn);
                
                chart1.Series["price"].ChartArea = "ChartArea1";
                //chart1.Series["volumn"].ChartArea = "ChartArea2";
                
                chart1.Series["price"].ChartType = SeriesChartType.Candlestick;
                //chart1.Series["volumn"].ChartType = SeriesChartType.Column;

                // Set point width
                chart1.Series["price"]["PointWidth"] = "1.0";
                chart1.Series["price"]["PriceUpColor"] = "Red";
                chart1.Series["price"]["PriceDownColor"] = "Green";
                chart1.Series["price"].XValueType = ChartValueType.DateTime;
                
                //chart1.Series["volumn"].XValueType = ChartValueType.DateTime;
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy/MM/dd";
                //chart1.ChartAreas[1].AxisX.LabelStyle.Format = "yyyy/MM/dd";

                chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;   //此为解决Y轴自适应
                //chart1.ChartAreas[1].AxisY.IsStartedFromZero = false;   //此为解决Y轴自适应

                Random r = new Random();
                my_instrument_data.price_pos = r.Next(f_candle_numbs, my_instrument_data.candle_series.Count - b_candle_numbs); //for ints
                for (int i = 0; i < f_candle_numbs; i++)
                {
                    cd = (candle_data)my_instrument_data.candle_series[my_instrument_data.price_pos];
                    // adding date and high
                    chart1.Series["price"].Points.AddXY(cd.dt, cd.high);
                    // adding low
                    chart1.Series["price"].Points[my_instrument_data.chart_pos].YValues[1] = cd.low;
                    //adding open
                    chart1.Series["price"].Points[my_instrument_data.chart_pos].YValues[2] = cd.open;
                    // adding close
                    chart1.Series["price"].Points[my_instrument_data.chart_pos].YValues[3] = cd.close;

                    //chart1.Series["volumn"].Points.AddXY(cd.dt, cd.v);
                    //if (cd.open < cd.close)
                    //    chart1.Series["volumn"].Points[my_instrument_data.chart_pos].Color = Color.Red;
                    //else
                    //    chart1.Series["volumn"].Points[my_instrument_data.chart_pos].Color = Color.Green;

                    my_instrument_data.price_pos++;
                    my_instrument_data.chart_pos++;

                }
                
            }
            else
            {
                tool_status.Text = "init_draw, 数据量不够，最少 " + min_candle_numbs;
            }
            //draw_one();
        }

        private void draw_one()
        {
            if (my_instrument_data.candle_series.Count < min_candle_numbs)
            {
                tool_status.Text = "draw_one, 数据量不够，最少 " + min_candle_numbs;
                return;
            }

            if (my_instrument_data.candle_series.Count == my_instrument_data.price_pos)
            {
                tool_status.Text = "draw_one, 已经到达最后一根k线";
                return;
            }
            
            cd = (candle_data)my_instrument_data.candle_series[my_instrument_data.price_pos];
            // adding date and high
            pos = chart1.Series["price"].Points.AddXY(cd.dt, cd.high);
            // adding low
            chart1.Series["price"].Points[my_instrument_data.chart_pos].YValues[1] = cd.low;
            //adding open
            chart1.Series["price"].Points[my_instrument_data.chart_pos].YValues[2] = cd.open;
            // adding close
            chart1.Series["price"].Points[my_instrument_data.chart_pos].YValues[3] = cd.close;

            chart1.Series["price"].LegendText = String.Format("价格：{0}\n成交：{1}", cd.close, cd.v);
            //chart1.Series["price"].Label = "";
            //chart1.Series["price"].Points[my_instrument_data.chart_pos].Label = "asdf";
            //chart1.Series["volumn"].Points.AddXY(cd.dt, cd.v);
            //if (cd.open < cd.close)
            //    chart1.Series["volumn"].Points[my_instrument_data.chart_pos].Color = Color.Red;
            //else
            //    chart1.Series["volumn"].Points[my_instrument_data.chart_pos].Color = Color.Green;

            Axis xaxis = chart1.ChartAreas[0].AxisX;
            xaxis.Minimum = xaxis.Maximum - f_candle_numbs;
            //xaxis = chart1.ChartAreas[1].AxisX;
            //xaxis.Minimum = xaxis.Maximum - f_candle_numbs;

            // Axis yaxis = chart1.ChartAreas[0].AxisY;

            double min_p, min_v;
            min_p = min_v = double.MaxValue;
            double max_p, max_v;
            max_p = max_v = double.MinValue;

            for (int i = 0; i < f_candle_numbs; i++)
            {
                var pcd = (candle_data)my_instrument_data.candle_series[my_instrument_data.price_pos-i];
                min_p = Math.Min(pcd.low, min_p);
                min_v = Math.Min(pcd.v, min_v);
                max_p = Math.Max(pcd.high, max_p);
                max_v = Math.Max(pcd.v, max_v);
            }

            Axis yaxis = chart1.ChartAreas[0].AxisY;
            yaxis.Minimum = min_p - (max_p - min_p) * 0.05;
            yaxis.Maximum = max_p + (max_p - min_p) * 0.05;

            //yaxis = chart1.ChartAreas[1].AxisY;
            //yaxis.Minimum = min_v - (max_v - min_v);// * 0.05;
            //yaxis.Maximum = max_v + (max_v - min_v);// * 0.05;

            //chart1.Legends[0].Title = cd.close.ToString();
            my_instrument_data.price_pos++;
            my_instrument_data.chart_pos++;

        }

        private void update_info()
        {
            if (cd != null)
                label5.Text = "k线时间：" + cd.dt.ToString();
            textBox1.Text = String.Format("{0:0,0.0}", mytrades.totalfunds);
            label3.Text = "胜率 盈：" + mytrades.wins + ", 亏：" + mytrades.loses + "，总：" + mytrades.totaltrades; 
            if( mytrades.totaltrades != 0)
            {
                label3.Text += "，胜率：" + String.Format("{0:0.#}%", ((double)mytrades.wins) / mytrades.totaltrades * 100);
            }
            if( mytrades.loses != 0 && mytrades.wins != 0)
            {
                label4.Text = "盈亏比：" + String.Format("{0:0.#}", -mytrades.winfunds / mytrades.wins / (mytrades.losefunds/ mytrades.loses));
            }
        }

        private void draw_candle()
        {
            chart1.Series.Clear();
            Series price = new Series("price");
            Series volumn = new Series("volumn");
            price.IsXValueIndexed = true;
            volumn.IsXValueIndexed = true;
            chart1.Series.Add(price);
            chart1.Series.Add(volumn);
            chart1.Series["price"].ChartArea = "ChartArea1";
            chart1.Series["volumn"].ChartArea = "ChartArea2";

            chart1.Series["price"].ChartType = SeriesChartType.Candlestick;
            chart1.Series["volumn"].ChartType = SeriesChartType.Column;

            // Set point width
            chart1.Series["price"]["PointWidth"] = "1.0";
            chart1.Series["price"]["PriceUpColor"] = "Red";
            chart1.Series["price"]["PriceDownColor"] = "Green";
            chart1.Series["price"].XValueType = ChartValueType.DateTime;
            chart1.Series["volumn"].XValueType = ChartValueType.DateTime;
            //for (int i = 0; i < id_series.candle_series.Count; i++)
            for (int i = 0; i < 10; i++)
            {
                var cd = (candle_data)my_instrument_data.candle_series[i];
                // adding date and high
                chart1.Series["price"].Points.AddXY(cd.dt, cd.high);
                // adding low
                chart1.Series["price"].Points[i].YValues[1] = cd.low;
                //adding open
                chart1.Series["price"].Points[i].YValues[2] = cd.open;
                // adding close
                chart1.Series["price"].Points[i].YValues[3] = cd.close;

                chart1.Series["volumn"].Points.AddXY(cd.dt, cd.v);

            }

            //chart1.ChartAreas[0].AxisX.
            chart1.ChartAreas[0].AxisX.Interval = 5;
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;   //此为解决Y轴自适应
        }

        private void button5_Click(object sender, EventArgs e)
        {
            changeplaystate(!play_state);
        }

        private void changeplaystate(bool bflag)
        {
            if (bflag)
            {
                button5.Image = candlestick_test.Properties.Resources.pause;
            }
            else
            {
                button5.Image = candlestick_test.Properties.Resources.play;
            }
            play_state = bflag;
            reset_timer();
        }

        private void reset_timer()
        {
            myTimer.Enabled = play_state;
            if( comboBox1.SelectedIndex == 0)
                myTimer.Interval = 1000;
            else if (comboBox1.SelectedIndex == 1)
                myTimer.Interval = 1500;
            else if (comboBox1.SelectedIndex == 2)
                myTimer.Interval = 2000;
            else
                myTimer.Interval = 1500;
            Console.WriteLine("setimer to " + myTimer.Enabled + " Timer intervals:" + myTimer.Interval);
        }

        private void myTimer_Callback(object sender, EventArgs e)
        {
            draw_one();
            update_info();
            Console.WriteLine("tick! time now " + DateTime.Now.ToString() + " - " + cd.dt + "," + cd.open + "," + cd.high + "," + cd.low + "," + cd.close);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            reset_timer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // close
            if (mytrade != null && cd != null)
            {
                mytrade.close_price = cd.close;
                mytrades.addnewtrade(mytrade);
                mytrade = null;
                chart1.Series["price"].Points[pos].Label = "<";
            }
            else
            {
                tool_status.Text = "暂无需要平的仓位!";
            }
        }
         
        private void button2_Click(object sender, EventArgs e)
        {
            // sell
            if ( mytrade == null && my_instrument_data.candle_series.Count>0 && cd != null)
            {
                mytrade = new trade_single();
                mytrade.str_instrumentid = my_instrument_data.instrument_name;
                mytrade.direction = -1;
                mytrade.dt = cd.dt;
                mytrade.open_price = cd.close;
                mytrade.lots = 1;
                chart1.Series["price"].Points[pos].Label = "v";
            }
            else
            {
                tool_status.Text = "请先平仓!";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //buy
            if (mytrade == null && my_instrument_data.candle_series.Count > 0 && cd != null)
            {
                mytrade = new trade_single();
                mytrade.str_instrumentid = my_instrument_data.instrument_name;
                mytrade.direction = 1;
                mytrade.dt = cd.dt;
                mytrade.open_price = cd.close;
                mytrade.lots = 1;
                chart1.Series["price"].Points[pos].Label = "^";
            }
            else
            {
                tool_status.Text = "请先平仓!";
            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Suppose when User Press Ctrl + J then Click Button1

            if (e.KeyCode == Keys.NumPad0)
            {
                button5.PerformClick();
            }
            if (e.KeyCode == Keys.NumPad1)
            {
                button3.PerformClick();
            }
            if (e.KeyCode == Keys.NumPad2)
            {
                button2.PerformClick();
            }
            if (e.KeyCode == Keys.NumPad3)
            {
                button1.PerformClick();
            }
            if (e.KeyCode == Keys.NumPad4)
            {
                comboBox1.SelectedIndex = 0;
                reset_timer();
            }
            if (e.KeyCode == Keys.NumPad5)
            {
                comboBox1.SelectedIndex = 1;
                reset_timer();
            }
            if (e.KeyCode == Keys.NumPad6)
            {
                comboBox1.SelectedIndex = 2;
                reset_timer();
            }
            if (e.KeyCode == Keys.Decimal)
            {
                changeplaystate(false);
                this.WindowState = FormWindowState.Minimized;
            }
            if( e.KeyCode == Keys.NumPad7)
            {
                if (f_candle_numbs < 300)
                    f_candle_numbs+=10;
            }
            if( e.KeyCode == Keys.NumPad8)
            {
                if (f_candle_numbs > 50)
                    f_candle_numbs-=10;
            }
        }
    }
}
