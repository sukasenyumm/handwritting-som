using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tugas
{
    class Neuron
    {
        public double[] Weights;

        public int X;

        public int Y;

        private int length;

        private double nf;



        public Neuron(int x, int y, int length)
        {

            X = x;

            Y = y;

            this.length = length;

            nf = 1000 / Math.Log(length);

        }



        private double Gauss(Neuron win, int it)
        {

            double distance = Math.Sqrt(Math.Pow(win.X - X, 2) + Math.Pow(win.Y - Y, 2));

            return Math.Exp(-Math.Pow(distance, 2) / (Math.Pow(Strength(it), 2)));

        }



        private double LearningRate(int it)
        {

            return Math.Exp(-it / 1000) * 0.1;

        }



        private double Strength(int it)
        {

            return Math.Exp(-it / nf) * length;

        }



        public double UpdateWeights(double[] pattern, Neuron winner, int it)
        {

            double sum = 0;

            for (int i = 0; i < Weights.Length; i++)
            {

                double delta = LearningRate(it) * Gauss(winner, it) * (pattern[i] - Weights[i]);

                Weights[i] += delta;

                sum += delta;

            }

            return sum / Weights.Length;

        }
    }
}
