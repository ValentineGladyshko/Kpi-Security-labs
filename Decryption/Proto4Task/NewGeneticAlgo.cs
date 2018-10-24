﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decryption.Proto4Task
{
    public class NewGeneticAlgo
    {
        public static readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        public static Random rand = new Random();
        public static string PolySubstitutionDecoder(string source, List<Dictionary<char, char>> dict)
        {
            List<char> decryptedText = new List<char>();
            for (int i = 0; i < source.Length; i++)
            {
                if (!dict[i % dict.Count].TryGetValue(source[i], out char value))
                    value = '?';
                decryptedText.Add(value);
            }
            return new string(decryptedText.ToArray());
        }

        public static double PolySubstitutionScore(string source, int[] ngrams, Dictionary<string, double> dict)
        {
            double score = 0.0;

            foreach (var ngram in ngrams)
            {
                for (int i = 0; i < source.Length - ngram + 1; i++)
                {
                    dict.TryGetValue(source.Substring(i, ngram), out double value);
                    score += value * Math.Pow(ngram, 2);
                }
            }

            return score;
        }

        public static List<Dictionary<char, char>> InitMapping(int keyLength)
        {
            List<Dictionary<char, char>> mappings = new List<Dictionary<char, char>>();

            for (int i = 0; i < keyLength; i++)
            {
                List<char> letters = new List<char>(alphabet);
                Dictionary<char, char> mapping = new Dictionary<char, char>();

                foreach (char c in alphabet)
                {
                    char decode = letters[rand.Next(0, letters.Count)];
                    letters.Remove(decode);

                    mapping[c] = decode;
                }

                mappings.Add(mapping);
            }
            return mappings;
        }

        public static Dictionary<char, char> UpdateMappping(Dictionary<char, char> mapping, char key, char newValue)
        {
            char valueKey= ' ';
            foreach (var pair in mapping)
            {
                if (newValue == pair.Value)
                    valueKey = pair.Key;
            }

            char temp = mapping[key];
            mapping[key] = mapping[valueKey];
            mapping[valueKey] = temp;

            return mapping;
        }

        public static void Permutate<T>(HashSet<T> set, List<T> list, List<List<T>> list1)
        {
            if (set.Count == 0)
            {
                list1.Add(list);
                list = null;
                return;
            }
            foreach (var ch in set)
            {
                List<T> tempList = new List<T>(list);           
                HashSet<T> tempSet = new HashSet<T>(set);

                tempList.Add(ch);
                tempSet.Remove(ch);

                Permutate(tempSet, tempList, list1);
            }
            return;
        }

        public static List<List<Dictionary<char, char>>> InitPopulation(string source, int countTopSymbols, int keyLength)
        {
            List<char> topSymbols = new List<char>("etaoinsrhdluc");

            topSymbols = topSymbols.GetRange(0, countTopSymbols);

            HashSet<char> temp = new HashSet<char>(topSymbols);

            List<List<char>> topSymbolsPermutations = new List<List<char>>();
            Permutate(temp, new List<char>(), topSymbolsPermutations);

            List<List<char>> topSymbolsSource = new List<List<char>>();
            for (int i = 0; i < keyLength; i++)
            {
                Dictionary<char, int> counter = new Dictionary<char, int>();

                for (int j = i; j < source.Length; j += keyLength)
                {
                    counter[source[j]] = 0;
                }
                for (int j = i; j < source.Length; j += keyLength)
                {
                    counter[source[j]]++;
                }
               
                topSymbolsSource[i] = counter
                    .OrderByDescending(pair => pair.Value).ToList()
                    .GetRange(0, countTopSymbols).Select(pair => pair.Key).ToList();
            }

            List<List<Dictionary<char, char>>> population = new List<List<Dictionary<char, char>>>();

            for (int i = 0; i < topSymbolsPermutations.Count; i++)
            {
                population.Add(InitMapping(keyLength));
            }

            for (int i = 0; i < population.Count; i++)
            {
                for (int j = 0; j < keyLength; j++)
                {
                    for (int k = 0; (k < topSymbolsSource[j].Count) && (k < topSymbolsPermutations[i].Count); k++)
                    {
                        UpdateMappping(population[i][j], topSymbolsSource[j][k], topSymbolsPermutations[i][k]);
                    }
                }
            }

            return population;
        }

        public static List<double> Softmax(List<double> x)
        {
            double max = x.Max();
            List<double> exp = new List<double>();
            for (int i = 0; i < x.Count; i++)
            {
                exp.Add(Math.Exp(x[i] - max));
            }

            double sum = exp.Sum();

            for (int i = 0; i < exp.Count; i++)
            {
                exp[i] = exp[i] / sum;
            }

            return exp;
        }

        public static List<List<Dictionary<char, char>>> Crossover
            (List<List<Dictionary<char, char>>> dicts, List<double> scores, int populationSize, int keyLength)
        {
            List<double> probas = Softmax(scores);

            double sum = 0;
            probas = probas.Select(w => sum += w).ToList();

            List<List<Dictionary<char, char>>> population = new List<List<Dictionary<char, char>>>();

            for (int i = 0; i < populationSize; i++)
            {
                int index = probas.FindIndex(value => value >= rand.NextDouble());
                if (index == -1)
                {
                    index = probas.Count - 1;
                }
                else if (index > 0)
                {
                    index--;
                }

                List<Dictionary<char, char>> x = dicts[index];

                int index2 = probas.FindIndex(value => value >= rand.NextDouble());
                if (index2 == -1)
                {
                    index2 = probas.Count - 1;
                }
                else if (index2 > 0)
                {
                    index2--;
                }

                List<Dictionary<char, char>> y = dicts[index2];

                List<Dictionary<char, char>> child = new List<Dictionary<char, char>>();
                foreach (var dict in x)
                {
                    child.Add(new Dictionary<char, char>(dict));
                }

                for (int j = 0; j < keyLength; j++)
                {
                    foreach(var k in child[j].Keys)
                    {
                        UpdateMappping(child[j], k, (rand.Next(0, 2) == 0) ? x[j][k] : y[j][k]);
                    }
                }

                population.Add(child);
            }

            return population;
        }

        public static List<List<Dictionary<char, char>>> Mutate
            (List<List<Dictionary<char, char>>> dicts, int keyLength, double mutation = 0.25)
        {           
            for(int i=0;i< dicts.Count; i++)
            {
                for(int j=0;j< keyLength;j++)
                {
                    while(rand.NextDouble() < mutation)
                    {
                        char index1 = alphabet[rand.Next(0, alphabet.Length)];
                        char index2 = alphabet[rand.Next(0, alphabet.Length)];

                        UpdateMappping(dicts[i][j], index1, index2);
                    }
                }
            }

            return dicts;
        }


        public static void GeneticAlgo()
        {

        }
    }
}
