using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

namespace HarmonySearchWPFapp
{
    class HarmonyTool
    {

        public static double Rand_cont_gen(String min, String max)
        {
            Expression e = new Expression("rUni(" + min + "," + max + ")");
            return e.calculate();
        }

        public static int Rand_discrete_gen(String min, String max)
        {
            Expression e = new Expression("rUnid(" + min + "," + max + ")");
            double result = e.calculate();
            return Convert.ToInt32(result);
        }


        public static double EvaluateFun(Function f, double[] variables)
        {
            double result = f.calculate(variables);
            return result;
        }

        public static void Swap(double[,] tab, int i1, int j1, int i2, int j2)
        {
            double tmp = tab[i1, j1];
            tab[i1, j1] = tab[i2, j2];
            tab[i2, j2] = tmp;
        }

        public static void InitializeHM(double[,] HMtab, double[] PVBmin, double[] PVBmax)
        {
            System.Random rng = SystemRandomSource.Default;
            double tmp;
            for (int i = 0; i < HMtab.GetLength(0); i++)
            {
                for (int j = 0; j < HMtab.GetLength(1) - 1; j++)
                {
                    tmp = rng.NextDouble() * (PVBmax[j] - PVBmin[j]) + PVBmin[j];
                    HMtab[i, j] = tmp;
                }
                HMtab[i, HMtab.GetLength(1) - 1] = 0;
            }
        }

        public static String DisplayHM(double[,] HMtab)
        {
            String partialHM="";
            for (int i = 0; i < HMtab.GetLength(0); i++)
            {
                for (int j = 0; j < HMtab.GetLength(1); j++)
                {
                    partialHM += HMtab[i, j].ToString()+" ";
                }
                partialHM += "\n";
            }
            return partialHM;
        }

        public static String DisplayBestSolution(double[,] HMtab)
        {
            String partialHM = "";
             for (int j = 0; j < HMtab.GetLength(1); j++)
             {
                partialHM += HMtab[0, j].ToString() + " ";
             }
                partialHM += "\n";
            return partialHM;
        }

        //calculating f(x) for every vector in HM
        public static void PerformHM(Function f, double[,] HMtab)
        {
            double[] valuesOfVariables = new double[HMtab.GetLength(1) - 1];
            for (int i = 0; i < HMtab.GetLength(0); i++)
            {
                for (int j = 0; j < HMtab.GetLength(1) - 1; j++) //going through parameters excluding column with f(x)
                {
                    valuesOfVariables[j] = HMtab[i, j];
                }
                HMtab[i, (HMtab.GetLength(1) - 1)] = HarmonyTool.EvaluateFun(f, valuesOfVariables);
            }
        }

        //calculating f(x) value for new vector
        public static void PperformVec(Function f, double[] vec)
        {
            double[] vecOfValues = new double[f.getArgumentsNumber()];
            for (int i = 0; i < f.getArgumentsNumber(); i++)
            {
                vecOfValues[i] = vec[i]; //assigning value to temporary vector from new found vector
            }
            vec[vec.GetLength(0) - 1] = HarmonyTool.EvaluateFun(f, vecOfValues);
        }

        public static void SortHM(double[,] HMtab)
        {
            int valuePosition = HMtab.GetLength(1) - 1;
            for (int i = 0; i < HMtab.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < HMtab.GetLength(0) - 1; j++)
                {
                    if (HMtab[j, valuePosition] > HMtab[j + 1, valuePosition])
                    {
                        for (int x = 0; x < HMtab.GetLength(1); x++)
                        {
                            HarmonyTool.Swap(HMtab, j, x, j + 1, x);
                        }

                    }
                }
            }
        }


        public static double[] NewHarmonyVector(double[,] HMtab, double HMCR, double PAR, double BW, double[] PVBmin, double[] PVBmax)
        {
            int newIndex;
            int variables = HMtab.GetLength(1);
            System.Random rng = SystemRandomSource.Default;
            double[] vec = new double[variables]; //new harmony vector
            for (int i = 0; i < variables - 1; i++) // -1 because last column contain f(X)
            {
                if (rng.NextDouble() < HMCR)
                {
                    newIndex = rng.Next(0,10);
                    vec[i] = HMtab[newIndex, i];
                    if (rng.NextDouble() < PAR)
                    {
                        if (rng.NextDouble() < 0.5)
                        {
                            double D5 = vec[i] - rng.NextDouble() * BW;
                            if (D5 >= PVBmin[i])
                            {
                                vec[i] = D5; //pitch adjustment
                            }
                        }
                        else
                        {
                            double D5_2 = vec[i] + rng.NextDouble() * BW;
                            if (D5_2 <= PVBmax[i])
                            {
                                vec[i] = D5_2;
                            }
                        }
                    }
                }
                else
                {
                    vec[i] = rng.NextDouble() * (PVBmax[i] - PVBmin[i]) + PVBmin[i]; //randomization
                }
            }
            return vec;
        }


        public static void UpdateHM(double[,] HMtab, Function f, double[] vec)
        {
            HarmonyTool.PperformVec(f, vec);
            if (vec[vec.GetLength(0) - 1] < HMtab[HMtab.GetLength(0) - 1, HMtab.GetLength(1) - 1])
            {
                for (int i = 0; i < HMtab.GetLength(1); i++)
                {
                    HMtab[HMtab.GetLength(0) - 1, i] = vec[i];
                }
                HarmonyTool.SortHM(HMtab);
            }
        }


        public static String HarmonySearchAlgorithm(Function f, int NI, int HMS, double HMCR, double PAR, double BW, double[] PVBmin, double[] PVBmax, ref double[] bestSolution)
        {
            String output = "";
            int numberofVariables = f.getArgumentsNumber();
            double[,] HMtab = new double[HMS, numberofVariables + 1];
            int iterations = 0;
            HarmonyTool.InitializeHM(HMtab, PVBmin, PVBmax);
            HarmonyTool.PerformHM(f, HMtab);
            HarmonyTool.SortHM(HMtab);
            double[] newVec = new double[HMtab.GetLength(1)];
            for (int i = 0; i < NI; i++)
            {
                newVec = HarmonyTool.NewHarmonyVector(HMtab, HMCR, PAR, BW, PVBmin, PVBmax);
                HarmonyTool.UpdateHM(HMtab, f, newVec);
                if (NI-iterations <= 20)
                {
                    output +="Iteration number: "+iterations.ToString()+"\n" + HarmonyTool.DisplayBestSolution(HMtab)+"\n";
                }
                iterations++;
            }
            if(HMtab.GetLength(1)-1 == 2)
            {
                bestSolution[0] = HMtab[0, 0];
                bestSolution[1] = HMtab[0, 1];
            }
            else
            {
                bestSolution[0] = Double.NaN;
                bestSolution[1] = Double.NaN;
            }

            return output;

        }


    }
}
