using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonySearchConsoleAlgorithm
{
    class HarmonyTool
    {
        public static double evaluateFun(double x1, double x2)
        {
           return 4 * Math.Pow(x1, 2) - 2.1 * Math.Pow(x1, 4) + (1 / 3) * Math.Pow(x1, 6) + x1 * x2 - 4 * Math.Pow(x2, 2) + 4 * Math.Pow(x2, 4);
        }

        public static void swap(double[,] tab, int i1, int j1, int i2, int j2)
        {
            double tmp = tab[i1, j1];
            tab[i1, j1] = tab[i2, j2];
            tab[i2, j2] = tmp;
        }

        public static void initializeHM(double[,] HMtab, double PVBmin, double PVBmax)
        {
            Random rand = new Random();
            double tmp;
            for(int i=0; i<HMtab.GetLength(0); i++)
            {
                for(int j=0; j<HMtab.GetLength(1)-1; j++)
                {
                    tmp = rand.NextDouble() * (PVBmax-PVBmin)+PVBmin;
                    HMtab[i, j] = tmp;
                }
                HMtab[i, HMtab.GetLength(1) - 1] = 0;
            } 
        }

        public static void displayHM(double[,] HMtab)
        {
            for (int i = 0; i < HMtab.GetLength(0); i++)
            {
                for (int j = 0; j < HMtab.GetLength(1); j++)
                {
                    Console.Write(HMtab[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        //obliczanie f(x) dla kazdego wektora z HM
        public static void performHM(double[,] HMtab)
        {
            for (int i = 0; i < HMtab.GetLength(0); i++)
            {
                HMtab[i, (HMtab.GetLength(1) - 1)] = HarmonyTool.evaluateFun(HMtab[i, 0], HMtab[i, 1]);
            }
        }


        //tutaj jako argument bedzie podawany string funkcji (mxParser)
        public static void sortHM(double [,] HMtab)
        {
            int valuePosition = HMtab.GetLength(1) - 1;
            for (int i = 0; i < HMtab.GetLength(0)-1; i++)
            {
                for(int j=0; j < HMtab.GetLength(0)-1; j++)
                {
                    if(HMtab[j,valuePosition] > HMtab[j + 1, valuePosition])
                    {
                        for(int x=0; x<HMtab.GetLength(1); x++)
                        {
                            HarmonyTool.swap(HMtab, j, x, j+1, x );
                        }
                    
                    }
                }
            }
        }


    }




    class Program
    {
        static void Main(string[] args)
        {
            double HMCR=0.85;
            double PAR=0.45;
            double BW=0.2;
            int HMS = 10;
            int NI=5000;
            double PVBmin = -10;
            double PVBmax = 10;
            double[,] testTab = new double[10,3];
            HarmonyTool.initializeHM(testTab,-10,10);
            HarmonyTool.displayHM(testTab);
            HarmonyTool.performHM(testTab);
            Console.WriteLine("\n");
            HarmonyTool.displayHM(testTab);
            HarmonyTool.sortHM(testTab);
            Console.WriteLine("Harmony memory posortowana!");
            HarmonyTool.displayHM(testTab);
          

           
        }


    }
}
