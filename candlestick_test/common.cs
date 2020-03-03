using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ArrayList candle_series;                        // 

        public instrument_data()
        {
            instrument_name = "unknown";
            timeframe = 0;
            candle_series = new ArrayList();
        }

        public bool try_parse(string fn)
        {
            bool bflag = false;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(fn);
                foreach (string line in lines)
                {
                    string[] columns = line.Split(',');
                    foreach (string column in columns)
                    {
                        // Do something
                        // Console.WriteLine(columns);
                        var cd = new candle_data();
                        if( DateTime.TryParseExact( columns[0], "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out cd.dt)){
                            timeframe = 10;
                        }
                        else if(DateTime.TryParseExact(columns[0], "yyyy/MM/dd hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out cd.dt))
                        {
                            timeframe = 20;
                        }
                        else
                        {
                                return bflag;
                        }
                        cd.open = double.Parse(columns[1]);
                        cd.high = double.Parse(columns[2]);
                        cd.low = double.Parse(columns[3]);
                        cd.close = double.Parse(columns[4]);

                        cd.interest = int.Parse(columns[5]);
                        cd.v = int.Parse(columns[6]);
                        candle_series.Add(cd);
                        //Double.TryParse()
                    }
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
}
