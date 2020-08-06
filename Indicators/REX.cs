using System;
using System.Collections.Generic;
using System.Linq;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;
using OkonkwoOandaV20.TradeLibrary.Instrument;

namespace Indicators
{
    //Not Complete
    static class REX
    {
        public static string AccountID { get; set; }
        public static int PeriodLength { get; set; }
        public static int SignalLength { get; set; }
        public static double[] TVB { get; set; }
        public static double[] Rex { get; set; }
        public static double[] Signal { get; set; }
        public static CandlestickPlus[] Candles { get; set; }

        public static void InitializeRex(string pair, string account, int signal = 14, int period = 25)
        {
            AccountID = account;
            PeriodLength = period;
            SignalLength = signal;
            LoadCandles(pair);
            CalcTVB();
            CalcRex(pair);
            CalcSignal();
        }

        public static void LoadCandles(string pair)
        {
            Candles = GetInstrumentCandlesAsync(pair, new InstrumentCandlesParameters() { granularity = "H1", count = 100, }).Result.ToArray();
        }

        public static void CalcTVB()
        {
            TVB = Candles.Select(i => (double)(3 * i.mid.c - (i.mid.h + i.mid.l + i.mid.o))).ToArray();
        }

        public static double[] CalcMA(double[] array, int period)
        {
            List<double> arrayList = new List<double>();
            for (int i = period - 1; i < array.Length; i++)
            {
                double value = 0.0;
                for (int j = 0; j < period; j++)
                {
                    value += array[i - j];
                }
                arrayList.Add(value / period);
            }

            return arrayList.ToArray();
        }

        public static void CalcRex(string pair)
        {
            List<string> instrument = new List<string>();
            instrument.Add(pair);
            var point = GetAccountInstrumentsAsync(AccountID, new AccountInstrumentsParameters { instruments = instrument }).Result.Last().pipLocation;
            Rex = CalcMA(TVB, PeriodLength);
            Rex = Rex.Select(i => i / Math.Pow(10, point - 1)).ToArray();
        }
        public static void CalcSignal()
        {
            Signal = CalcMA(Rex, SignalLength);
        }

    }
}
