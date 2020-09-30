using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace candlestick_test
{
    public class candle_data
    {
        public DateTime dt;
        public double open;
        public double high;
        public double low;
        public double close;
        public int interest;
        public int v;
    }

    public class instrument_data
    {
        public string instrument_name;
        public int timeframe;                                  // 10:daily 20:m30
        public ArrayList candle_series;                        // 价格数据
        public int price_pos;                                  // 目前的价格位置
        public int chart_pos;
        public static int s_candle_numb = 300;                  // 每次复盘k线数量

        public instrument_data()
        {
            instrument_name = "unknown";
            timeframe = 0;
            candle_series = new ArrayList();
            price_pos = chart_pos = 0;
        }

        public bool try_parse(string fn)
        {
            bool bflag = false;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(fn);
                DateTime dt2 = new DateTime(2016, 1, 1, 0, 0, 0);
                ArrayList temp_candle = new ArrayList();
                foreach (string line in lines)
                {
                    string[] columns = line.Split(',');
                    if( columns.Length >= 7)
                    {
                        // Do something
                        // Console.WriteLine(columns);
                        try
                        {
                            var cd = new candle_data();
                            if (DateTime.TryParseExact(columns[0].Substring(0, 19), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out cd.dt))
                            {
                                timeframe = 10;
                            }
                            else if (DateTime.TryParseExact(columns[0].Substring(0, 19), "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out cd.dt))
                            {
                                timeframe = 20;
                            }
                            else
                            {
                                continue;
                            }
                            if (cd.dt < dt2)
                                continue;
                            cd.open = double.Parse(columns[1]);
                            cd.high = double.Parse(columns[2]);
                            cd.low = double.Parse(columns[3]);
                            cd.close = double.Parse(columns[4]);

                            cd.interest = int.Parse(columns[5]);
                            cd.v = int.Parse(columns[6]);
                            temp_candle.Add(cd);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        //Double.TryParse()
                    }
                }
                Random r = new Random();
                int temp_start = r.Next(0, Math.Max(0, temp_candle.Count - s_candle_numb));
                int temp_end = Math.Min(temp_candle.Count, temp_start + s_candle_numb);
                if( temp_end > 0)
                {
                    candle_series = temp_candle.GetRange(temp_start, temp_end - temp_start);
                    price_pos = 0;
                }
                instrument_name = Path.GetFileNameWithoutExtension(fn);
                bflag = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return bflag;
        }
    }

    // 单笔交易
    public class trade_single
    {
        public string str_instrumentid;
        public DateTime dt;
        public double open_price;
        public double close_price;
        public int lots;                // 1
        public int direction;           // -1, 1
        public trade_single()
        {
            str_instrumentid = "Unkown";
            lots = 1;
        }
    }

    // 完整的一次交易
    public class trades_s
    {
        public int[] lots_list;
        public ArrayList trades;
        public int trade_numb;
        public double d_close;
        public int direction;           // -1, 1
        public int t_lots;
        public trades_s()
        {
            lots_list = new int[] { 3, 2, 1 };
            trades = new ArrayList();
            trade_numb = 0;
            d_close = 0;
            direction = 0;
            t_lots = 0;
        }
        public int get_lots()
        {
            int resp = 1;
            if(trade_numb < lots_list.Length)
            {
                resp =  lots_list[trade_numb];
            }
            t_lots += resp;
            trade_numb++;
            return resp;
        }
    }

    // 整个交易
    public class trades_all
    {
        public ArrayList trades;            // 交易历史记录

        public double totalfunds;           // 交易资金
        public double winfunds;             // 盈利资金
        public double losefunds;            // 亏损资金
        public int totaltrades;             // 交易次数
        public int wins;                    // 盈利次数
        public int loses;

        public trades_all()
        {
            trades = new ArrayList();
            totalfunds = 0;
            winfunds = losefunds = 0;
            totaltrades = 0;
            wins = loses = 0;
        }

        public void addnewtrade( trade_single mytrade)
        {
            if( mytrade != null)
            {
                double profit = (mytrade.close_price - mytrade.open_price) * mytrade.direction * mytrade.lots;
                if (profit > 0)
                {
                    winfunds += profit;
                    wins +=mytrade.lots;
                }
                else
                {
                    losefunds += profit;
                    loses += mytrade.lots;
                }

                totalfunds += profit;
                totaltrades++;
                //wins += (mytrade.close_price - mytrade.open_price) * mytrade.direction > 0 ? 1 : 0;
                trades.Add(mytrade);
                
            }
        }
    }

}
