﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decryption.SubstitutionDecript
{
    public class LanguageStatisticsHelper
    {
        private LanguageStatisticsHelper()
        {
            BiGramDict = LoadBiGramStatistic();
            UniGramDict = LoadUniGramStatistic();
            TriGramDict = LoadTriGramStatistic();
        }
        private static readonly LanguageStatisticsHelper _languageStatisticsHelper = new LanguageStatisticsHelper();

        public static LanguageStatisticsHelper GetLanguageStatistics()
        {
            return _languageStatisticsHelper;
        }

        public IDictionary<char, double> UniGramDict { get; set; }
        public IDictionary<string,double> BiGramDict { get; set; }
        public IDictionary<string,double> TriGramDict { get; set; }

        public IDictionary<char, double> LoadUniGramStatistic()
        {
            return File.ReadLines("../../../Decryption/SubstitutionDecript/unigram.csv")
                .Select(line => line.Split(';'))
                .ToDictionary(line => Convert.ToChar(line[0]), line => Convert.ToDouble(line[1]) / 100);
        }

        public IDictionary<string,double> LoadBiGramStatistic()
        {
            return File.ReadLines("../../../Decryption/SubstitutionDecript/bigram.csv")
                .Select(line => line.Split(','))
                .ToDictionary(line => line[0], line => Convert.ToDouble(line[1])/2800000000000);
        }

        public IDictionary<string,double> LoadTriGramStatistic()
        {
            return File.ReadLines("../../../Decryption/SubstitutionDecript/trigram.csv")
                .Select(line => line.Split(','))
                .ToDictionary(line => line[0], line => Convert.ToDouble(line[1]) / 4500000000);
        }

        public IDictionary<char,double> CreateUniGramStatistic(string text)
        {
            double _len = Convert.ToDouble(text.Count());
            return text.GroupBy(c => c)
                .Select(c => new { Char = c.Key, Count = c.Count() })
                .ToDictionary(x => x.Char, x => x.Count/_len);
        }

        public IDictionary<string,double> CreateBiGramStatistic(string text)
        {
            double _len = Convert.ToDouble(text.Count())/2;

            ConcurrentDictionary<string, double> biGramDict = new ConcurrentDictionary<string, double>();

            for(int i = 1; i < text.Length; i++)
            {
                char prev = text[i - 1];
                char curr = text[i];
                char[] currBlock = { prev, curr };
                string key = new string(currBlock);
                biGramDict.AddOrUpdate(key, 1, (id, count) => count + 1);
            }
            return biGramDict.ToDictionary(x => x.Key, x => x.Value / _len);
        }

        public IDictionary<string,double> CreateTriGramStatistic(string text)
        {
            double _len = Convert.ToDouble(text.Count())/3;

            ConcurrentDictionary<string, double> triGramDict = new ConcurrentDictionary<string, double>();

            for(int i = 2; i < text.Length; i++)
            {
                char prevprev = text[i - 2];
                char prev = text[i - 1];
                char curr = text[i];
                char[] currBlock = { prevprev, prev, curr };
                string key = new string(currBlock);
                triGramDict.AddOrUpdate(key, 1, (id, count) => count + 1);
            }
            return triGramDict.ToDictionary(x => x.Key, x => x.Value / _len);
        }
    }
}
