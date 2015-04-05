using System;
using System.Collections.Generic;
using System.IO;

namespace Tugas
{
    class Map
    {
        private string result;
        private string learningtime;
        private Neuron[,] outputs;  // Collection of weights.
        private int iteration;      // Current iteration.
        private int length;        // Side length of output grid.
        private int dimensions;    // Number of input dimensions.
        private Random rnd = new Random();

        private List<string> labels = new List<string>();
        private List<double[]> patterns = new List<double[]>();

        public Map(int dimensions, int length, string file)
        {

            DateTime startTime = DateTime.Now;
            this.length = length;
            this.dimensions = dimensions;
            Initialise();
            LoadData(file);
            //NormalisePatterns();
            Train(0.0000001);
            DumpCoordinates();

            TimeSpan runTime = DateTime.Now - startTime;
            Console.WriteLine("Time elapsed: " + Convert.ToString(runTime.TotalMilliseconds / 1000) + " s");
            learningtime = "Time elapsed: " + Convert.ToString(Math.Floor(runTime.TotalMilliseconds / 1000)) + " s";

            CheckNeuronMap();
        }

        public string getResult()
        {
            return result;
        }
        public string getLearningTime()
        {
            return learningtime;
        }
        private void Initialise()
        {
            Console.WriteLine("Neuron Initialized.... ");
            outputs = new Neuron[length, length];

            for (int i = 0; i < length; i++)
            {

                for (int j = 0; j < length; j++)
                {
                    outputs[i, j] = new Neuron(i, j, length);
                    Console.WriteLine("Output Neuron {0}: [{1},{2}] weight:", i, outputs[i, j].X, outputs[i, j].Y);

                    outputs[i, j].Weights = new double[dimensions];

                    for (int k = 0; k < dimensions; k++)
                    {

                        outputs[i, j].Weights[k] = rnd.NextDouble();
                        Console.Write("{0:N2} ", outputs[i, j].Weights[k]);
                    }
                }
            }
        }

        private void LoadData(string file)
        {

            StreamReader reader = File.OpenText(file);
            reader.ReadLine(); // Ignore first line.
            while (!reader.EndOfStream)
            {

                string[] line = reader.ReadLine().Split(',');
                labels.Add(line[0]);
                double[] inputs = new double[dimensions];
                for (int i = 0; i < dimensions; i++)
                {
                    ////harcoded here
                    //if (double.Parse(line[i + 1]) == 10.0)
                    //    inputs[i] = double.Parse(line[i + 1]) - 9.0;
                    //else
                        inputs[i] = double.Parse(line[i + 1]);
                }
                patterns.Add(inputs);
            }
            reader.Close();

        }

        //commented because we don't need to normalize data. (0,1)
        //private void NormalisePatterns()
        //{

        //    for (int j = 0; j < dimensions; j++)
        //    {

        //        double sum = 0;
        //        for (int i = 0; i < patterns.Count; i++)
        //        {
        //            sum += patterns[i][j];
        //        }

        //        double average = sum / patterns.Count;

        //        for (int i = 0; i < patterns.Count; i++)
        //        {
        //            patterns[i][j] = patterns[i][j] / average;
        //            //hardcoded here
        //            if (double.IsNaN(patterns[i][j]))
        //                patterns[i][j] = 0;
        //        }
        //    }
        //}



        private void Train(double maxError)
        {

            double currentError = double.MaxValue;
            Console.WriteLine("learning progress... (iteration)");
            while (currentError > maxError)
            {
                currentError = 0;

                List<double[]> TrainingSet = new List<double[]>();

                foreach (double[] pattern in patterns)
                {
                    TrainingSet.Add(pattern);
                }

                for (int i = 0; i < patterns.Count; i++)
                {
                    double[] pattern = TrainingSet[rnd.Next(patterns.Count - i)];
                    currentError += TrainPattern(pattern);
                    TrainingSet.Remove(pattern);
                }
            }

        }



        private double TrainPattern(double[] pattern)
        {
            double error = 0;

            Neuron winner = Winner(pattern);
            //Console.WriteLine("Neuron Winner {0},{1}", winner.X, winner.Y);

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    error += outputs[i, j].UpdateWeights(pattern, winner, iteration);
                }
            }
            iteration++;

            return Math.Abs(error / (length * length));
        }

        private void DumpCoordinates()
        {
            for (int i = 0; i < patterns.Count-1; i++)
            {
                if (Winner(patterns[patterns.Count-1]).X == Winner(patterns[i]).X && Winner(patterns[patterns.Count-1]).Y == Winner(patterns[i]).Y)
                {
                    if (labels[i] != "9999")
                    result = "Recognize: '" + labels[i] +"'";
                    break;
                }
                else
                {
                    result = "result not found";
                }
            }

            for (int j = 0; j < patterns.Count; j++)
            {
                Neuron n = Winner(patterns[j]);
                Console.WriteLine("{0},{1},{2}", labels[j], n.X, n.Y);
            }

        }

        private void CheckNeuronMap()
        {
            for (int i = 0; i < patterns.Count; i++)
            {
                Neuron n = Winner(patterns[i]);
                Console.WriteLine("{0},{1},{2}", labels[i], n.X, n.Y);
            }
            int err = 0;
            for (int i = 0; i < patterns.Count; i++)
            {
                int min = i;
                Neuron n1 = Winner(patterns[min]);
                for (int j = i + 1; j < patterns.Count; j++)
                {

                    Neuron n2 = Winner(patterns[j]);

                    if (n1.X == n2.X && n1.Y == n2.Y && labels[j] != labels[min])
                    {
                        err++;
                    }
                }
            }
            Console.WriteLine("error: {0}", err);
        }

        private Neuron Winner(double[] pattern)
        {

            Neuron winner = null;
            double min = double.MaxValue;

            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                {
                    double d = Distance(pattern, outputs[i, j].Weights);
                    if (d < min)
                    {
                        min = d;
                        winner = outputs[i, j];
                    }
                }
            return winner;
        }


        /*euclidean distance between neuron input and output*/
        private double Distance(double[] vector1, double[] vector2)
        {
            double value = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                value += Math.Pow((vector1[i] - vector2[i]), 2);
            }
            return Math.Sqrt(value);

        }
    }
}
