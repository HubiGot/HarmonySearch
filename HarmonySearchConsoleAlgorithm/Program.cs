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
            return (4 - 2.1 * Math.Pow(x1, 2) + Math.Pow(x1, 4) / 3) * Math.Pow(x1, 2) + x1 * x2 + (-4 + 4 * Math.Pow(x2, 2)) * Math.Pow(x2, 2);
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


        public static double[] newHarmonyVector(double[,] HMtab,double HMCR, double PAR, double BW, double PVBmin, double PVBmax)
        {
            int newIndex;
            int variables = HMtab.GetLength(1);
            double[] vec = new double[variables]; //new harmony vector
            Random rand = new Random();
            for (int i=0; i<variables-1; i++) // -1 bo osatnia kolumna dla f(X)
            {
                if (rand.NextDouble() < HMCR)
                {
                    newIndex = Convert.ToInt32(rand.NextDouble() * HMtab.GetLength(0));
                    vec[i] = HMtab[newIndex, i];
                    Console.WriteLine("Memory considering");
                    if (rand.NextDouble() < PAR)
                    {
                        if (rand.NextDouble() < 0.5)
                        {
                            double D5 = vec[i] - rand.NextDouble() * BW;
                            if (D5>=PVBmin)
                            {
                                vec[i] = D5; //pitch adjustment
                                Console.WriteLine("Pitch adjustment - bw");
                            }
                        }
                        else
                        {
                            double D5_2 = vec[i] + rand.NextDouble() * BW;
                            if (D5_2<=PVBmax)
                            {
                                vec[i] = D5_2;
                                Console.WriteLine("Pitch adjustment + bw");
                            }
                        }
                    }
                }
                else
                {
                    vec[i]= rand.NextDouble() * (PVBmax - PVBmin) + PVBmin; //randomizacja
                    Console.WriteLine("Randomizacja");
                }
            }
            return vec;
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
            double[] newVec = new double[3];
            HarmonyTool.initializeHM(testTab,-10,10);
            //HarmonyTool.displayHM(testTab);
            HarmonyTool.performHM(testTab);
           // Console.WriteLine("\n");
           // HarmonyTool.displayHM(testTab);
            HarmonyTool.sortHM(testTab);
            //Console.WriteLine("Harmony memory posortowana!");
            HarmonyTool.displayHM(testTab);
            Console.WriteLine();
            Console.WriteLine();
            //Console.WriteLine("Testowy obliczenia evaluate fun: "+HarmonyTool.evaluateFun(3.183,-0.4));
            newVec = HarmonyTool.newHarmonyVector(testTab, HMCR, PAR, BW, PVBmin, PVBmax);
            Console.WriteLine("nowy wektor: " + newVec[0]+" "+newVec[1] );




           
        }


    }
}
