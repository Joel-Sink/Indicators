using OkonkwoOandaV20.TradeLibrary.Instrument;
using System;
using System.Collections.Generic;
using System.Linq;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

namespace Indicators
{
    class ATR
    {
        public static int PeriodLength { get; set; }
        public static CandlestickPlus[] Candles { get; set; }
        public static decimal[] TR { get; set; }
        public static decimal[] Atr { get; set; }

        public static void InitializeATR(string pair, int period = 14)
        {
            PeriodLength = period;
            LoadCandles(pair);
            CalcTR();
            CalcFirstValueATR();
            CalcATR();
        }
        public static void LoadCandles(string pair)
        {
            Candles = GetInstrumentCandlesAsync(pair, new InstrumentCandlesParameters() { granularity = "H1", count = 200, }).Result.ToArray();
        }
        public static void CalcTR()
        {
            var ListTR = new List<decimal>();
            for (int i = 1; i < Candles.Length; i++)
            {
                var close = Candles[i - 1].mid.c;
                var high = Candles[i].mid.h;
                var low = Candles[i].mid.l;

                var tr = Math.Max(high, close) - Math.Min(low, close);

                ListTR.Add(tr);
            }

            TR = ListTR.ToArray();
        }
        public static void CalcFirstValueATR()
        {
            decimal sumation = (decimal)0.0;

            for (int i = 0; i < PeriodLength; i++)
            {
                sumation += TR[i];
            }

            var atr = TR[13] * sumation / PeriodLength;

            List<decimal> first = new List<decimal>();
            first.Add(atr);

            Atr = first.ToArray();
        }
        public static void CalcATR()
        {
            decimal currentATR;
            List<decimal> ATRarray = Atr.ToList();

            for (int i = 14; i < TR.Length; i++)
            {
                currentATR = (ATRarray.Last() * (PeriodLength - 1) + TR[i]) / PeriodLength;
                ATRarray.Add(currentATR);
            }

            Atr = ATRarray.ToArray();
        }
    }
}
