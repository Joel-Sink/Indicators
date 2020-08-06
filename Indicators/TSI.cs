using System;
using System.Collections.Generic;
using System.Linq;
using OkonkwoOandaV20.TradeLibrary.Instrument;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

namespace Indicators
{
    public static class TSI
    {
        public static CandlestickPlus[] Candles { get; set; }
        public static decimal[] MTM { get; set; }
        public static decimal[] ABSMTM { get; set; }
        public static decimal[] MTMEMA { get; set; }
        public static decimal[] ABSMTMEMA { get; set; }
        public static decimal[] MTMEMA2 { get; set; }
        public static decimal[] ABSMTMEMA2 { get; set; }
        public static decimal PeriodOne { get; set; }
        public static decimal PeriodTwo { get; set; }
        public static int MAPeriod { get; set; }
        public static decimal[] Tsi { get; set; }
        public static decimal[] TsiMA { get; set; }
        public static void InitializeTSI(int periodOne, int periodTwo, int maPeriod, string pair)
        {
            PeriodOne = periodOne;
            PeriodTwo = periodTwo;
            MAPeriod = maPeriod;
            LoadCandles(pair);
            LoadMomentum();
            LoadMTMEMA();
            LoadABSMTMEMA();
            LoadMTMEMA2();
            LoadABSMTMEMA2();
            LoadTSI();
            LoadTSIMA();
        }
        public static void LoadCandles(string pair)
        {
            Candles = GetInstrumentCandlesAsync(pair, new InstrumentCandlesParameters { granularity = "H1", count = 200 }).Result.ToArray();
        }
        public static void LoadMomentum()
        {
            List<decimal> MTMList = new List<decimal>();
            for (int i = 1; i < Candles.Length; i++)
            {
                MTMList.Add(Candles[i].mid.c - Candles[i - 1].mid.c);
            }
            MTM = MTMList.ToArray();
            ABSMTM = MTMList.Select(i => Math.Abs(i)).ToArray();
            MTMList.Clear();
        }
        public static void LoadMTMEMA()
        {
            MTMEMA = CalcEMA(MTM, PeriodOne);
        }
        public static void LoadABSMTMEMA()
        {
            ABSMTMEMA = CalcEMA(ABSMTM, PeriodOne);
        }
        public static void LoadMTMEMA2()
        {
            MTMEMA2 = CalcEMA(MTMEMA, PeriodTwo);
        }
        public static void LoadABSMTMEMA2()
        {
            ABSMTMEMA2 = CalcEMA(ABSMTMEMA, PeriodTwo);
        }
        public static void LoadTSI()
        {
            List<decimal> array = new List<decimal>();
            if (ABSMTMEMA2.Length == MTMEMA2.Length)
            {
                for (int i = 0; i < ABSMTMEMA2.Length; i++)
                {
                    array.Add(100 * (MTMEMA2[i] / ABSMTMEMA2[i]));
                }
            }
            Tsi = array.ToArray();
        }
        public static void LoadTSIMA()
        {
            TsiMA = CalcMA(Tsi, MAPeriod);
        }
        public static decimal[] CalcEMA(decimal[] array, decimal period)
        {
            decimal value = (decimal)0.0;
            for (int i = 0; i < period; i++)
            {
                value += Math.Abs(array[i]);
            }
            decimal SMA = (period - value) / period;

            List<decimal> EMA = new List<decimal>
            {
                SMA
            };
            decimal multiplyer = 2 / (period + 1);

            for (int i = int.Parse(period.ToString()); i < array.Length; i++)
            {
                var value1 = array[i] * multiplyer;
                var value2 = EMA.Last() * (1 - multiplyer);
                var emaval = value1 + value2;
                EMA.Add(emaval);
            }

            return EMA.ToArray();
        }
        public static decimal[] CalcMA(decimal[] array, int period)
        {
            List<decimal> arrayList = new List<decimal>();
            for (int i = period - 1; i < array.Length; i++)
            {
                decimal value = (decimal)0.0;
                for (int j = 0; j < period; j++)
                {
                    value += array[i - j];
                }
                arrayList.Add(value / period);
            }

            return arrayList.ToArray();
        }
    }
}
