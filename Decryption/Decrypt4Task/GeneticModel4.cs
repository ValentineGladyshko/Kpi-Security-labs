﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decryption.SubstitutionDecript
{
    public class GeneticModel4
    {
        Random rand = new Random();

        private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private string encryptedText;
        private SortedSet<Gen4> population;
        private int generationCount = 10000;
        private int populationCount = 50;
        private int keySize = 4;
        private double[] maxScore;
        private int mutationCount = 1;
        private double percentageOfElitism = 10;
        private int currentGeneration;

        public GeneticModel4(string encryptedText)
        {
            maxScore = new double[generationCount];
            this.encryptedText = encryptedText;
            population = new SortedSet<Gen4>();
        }

        public string GenerateRandomKey()
        {
            char[] key = alphabet;
            for (int i = 0; i < key.Length; i++)
            {
                char c = key[i];
                int shuffleIndex = rand.Next(key.Length - i) + i;
                key[i] = key[shuffleIndex];
                key[shuffleIndex] = c;
            }
            return new string(key);
        }
        //dejgnseprseranithribiljtmdntosjiilltilfltmdervrwadufoewfilemoshsedyithigaheaefnsceneegsttebrelerbtwanhaintrfetitefofileciorgnsunawenplyaddcithmathrrtlenomosdoncerdeawicounoputadserandoseciconyourhowsfrvarymoeanenohohnreanvdognnvooirevrskttfiluanhhllstavialetvldhtmeetaacwanhaivrhsehoectdcrtbnamownvutshairsitcrowoinfrgothttpdclattitawneyhaiconyoharsarsetjwnvutomeizhnsiacashenofrhytitleswingieinthlndcaocaallrqalqterdunchthsphstpanyoliabteidfjrmdoseeyhpzothtmhlqeltstmermlkslonecththrufeupoowerdebriscttfombanmidinslralwhsjgommniorwethsqtwantoedolcerdebtrepngfoliwapkrtseraniavaotethspnmfgmoulesdivbltheastrhdfwansbllhdpstfeaijcrsneetvtiheldcrsleswingpolisapobaoysaoovonunnsiiswhabhairsitcshluorchqhafryrtkenoqhauordkrenceispgticarrthoxeqahilosyeosteedicorendtideabserdsjusecicoocnemislneuideefshesiboat
        public string Run()
        {
            CreateFirstPopulation();
            double maxRate = 0.0;
            double sumRate = 0.0;
            int ss = 0;

            foreach (var Gen4 in population)
            {
                sumRate += Gen4.CalculateFitness();
                if (maxRate < Gen4.CalculateFitness())
                    maxRate = Gen4.CalculateFitness();
            }

            maxScore[0] = maxRate;
            Console.WriteLine("generation: " + currentGeneration + " max rate: " + maxRate + " avg rate: " +
                (sumRate / populationCount));
            //population[0].Show(encryptedText);
            for (int i = 0; i < generationCount; i++)
            {
                
                if (ss == 50)
                {
                    string decryptedText = new SubstitutionDecrypt4(population.First().Chromosome).DecryptText(encryptedText);
                    Console.WriteLine("\n===========\n" + decryptedText);
                    CipherFitness4.Show(decryptedText);
                    CipherFitness4.NewEvaluateShow(decryptedText);
                    Console.WriteLine("\n===========\n");
                    ss = 0;
                }
                ss++;
                CreateNewGeneration();
                //Console.WriteLine("generation: " + currentGeneration);
                maxRate = 0.0;
                sumRate = 0.0;
                foreach (var Gen4 in population)
                {
                    sumRate += Gen4.CalculateFitness();
                    if (maxRate < Gen4.CalculateFitness())
                        maxRate = Gen4.CalculateFitness();
                }
                maxScore[i] = maxRate;
                if (i > 20 && maxScore[i] == maxScore[i - 20])
                {

                    //Console.Write("No new gens: ");
                    //CipherFitness4.Show(new SubstitutionDecrypt4(population.First().Chromosome).DecryptText(encryptedText));
                }
                
 
               // Console.WriteLine("generation: " + currentGeneration + " max rate: " + maxRate + " avg rate: " +
               // (sumRate / populationCount));
                // population[0].Show(encryptedText);
            }
            return new SubstitutionDecrypt4(population.First().Chromosome).DecryptText(encryptedText);
        }

        private void CreateFirstPopulation()
        {
            currentGeneration = 0;
            List<string> gen = new List<string>();
            for (int i = 0; i < keySize; i++)
            {
                gen.Add(new string(alphabet));
            }
            population.Add(new Gen4(gen, encryptedText));
            while (population.Count < populationCount)
            {
                gen = new List<string>();
                for (int i = 0; i < keySize; i++)
                {
                    gen.Add(GenerateRandomKey());
                }
                Gen4 Gen4 = new Gen4(gen, encryptedText);
                population.Add(Gen4);
            }
        }

        private Gen4 Mutate(Gen4 gen)
        {
            List<char[]> chromosome = new List<char[]>();
            for (int i = 0; i < gen.Chromosome.Count; i++)
            {
                chromosome.Add(gen.Chromosome[i].ToCharArray());
            }

            int mutate = rand.Next(1, keySize);


            for (int j = 0; j < mutate; j++)
            {
                int position = rand.Next(0, keySize);
                int length = chromosome[position].Length;

                for (int i = 0; i < mutationCount; i++)
                {
                    int oldPosition = rand.Next(0, length);
                    int newPosition = -1;
                    do
                    {
                        newPosition = rand.Next(0, length);
                    }
                    while (oldPosition == newPosition);
                    char oldChar = chromosome[position][oldPosition];
                    chromosome[position][oldPosition] = chromosome[position][newPosition];
                    chromosome[position][newPosition] = oldChar;
                }
                int mut = 1;
                double mutation = rand.NextDouble();
                while (mutation < Math.Pow(0.8, mut))
                {
                    position = rand.Next(0, keySize);
                    mutation = rand.NextDouble();
                    mut += 1;
                    int oldPosition = rand.Next(0, length);
                    int newPosition = -1;
                    do
                    {
                        newPosition = rand.Next(0, length);
                    }
                    while (oldPosition == newPosition);
                    char oldChar = chromosome[position][oldPosition];
                    chromosome[position][oldPosition] = chromosome[position][newPosition];
                    chromosome[position][newPosition] = oldChar;
                }
            }
            List<string> newChromosome = new List<string>();

            for (int i = 0; i < chromosome.Count; i++)
            {
                newChromosome.Add(new string(chromosome[i]));
            }
            return new Gen4(newChromosome, encryptedText);
        }

        public void CreateNewGeneration()
        {
            SortedSet<Gen4> newPopulation = new SortedSet<Gen4>();
            List<Gen4> populationArray = population.ToList();
            int elitismAmount = (int)Math.Ceiling(populationCount * (percentageOfElitism / 100.0));

            int numberOfChildren = populationCount - elitismAmount;
            if (elitismAmount > 0)
            {
                for (int i = 0; i < elitismAmount; i++)
                {
                    newPopulation.Add(populationArray[i]);
                }
            }
            if (numberOfChildren > 0)
            {
                double sumRate = 0.0;
                for (int i = 0; i < elitismAmount; i++)
                {
                    sumRate += populationArray[i].CalculateFitness();
                }
                List<int> index = new List<int>();
                for (int i = 0; i < elitismAmount; i++)
                {
                    int count = (int)Math.Ceiling(Math.Pow(populationArray[i].CalculateFitness() / 100.0, 1.5));
                    for (int j = 0; j < count; j++)
                    {
                        index.Add(i);
                    }
                }

                while (newPopulation.Count < populationCount)
                {
                    newPopulation.Add(Mutate(populationArray[index[rand.Next(0, index.Count)]]));
                }
            }

            population = newPopulation;
            currentGeneration++;
        }

    }
}