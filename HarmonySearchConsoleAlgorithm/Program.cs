using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace HarmonySearchConsoleAlgorithm
{
    class HarmonyTool
    {
       
        public static double evaluateFun(Function f, double[] variables)
        {
            double result = f.calculate(variables);
            return result;
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
        public static void performHM(Function f, double[,] HMtab)
        {
            double[] valuesOfVariables = new double[HMtab.GetLength(1) - 1];
            for (int i = 0; i < HMtab.GetLength(0); i++) 
            {
                for(int j=0; j< HMtab.GetLength(1) - 1; j++) //przejscie po parametrach, wykluczajac ostatnia kolumne z wartoscia f(x)
                {
                    valuesOfVariables[j] = HMtab[i,j];
                }
                HMtab[i, (HMtab.GetLength(1) - 1)] = HarmonyTool.evaluateFun(f, valuesOfVariables);
            }
        }

        //obliczanie wartosci f(x) nowego wektora 
        public static void performVec(Function f, double[] vec)
        {
            double[] vecOfValues = new double[f.getArgumentsNumber()];
            for(int i=0; i< f.getArgumentsNumber(); i++)
            {
                vecOfValues[i] = vec[i]; //przypisanie wartosci do wektora pomocniczego z wektora nowo znalezionego
            }
            vec[vec.GetLength(0) - 1] = HarmonyTool.evaluateFun(f,vecOfValues);   
        }

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


        public static double[] newHarmonyVector(double[,] HMtab, double HMCR, double PAR, double BW, double PVBmin, double PVBmax)
        {
            int newIndex;
            int variables = HMtab.GetLength(1);
            double[] vec = new double[variables]; //new harmony vector
            Random rand = new Random();
            for (int i=0; i<variables-1; i++) // -1 bo osatnia kolumna dla f(X)
            {
                if (rand.NextDouble() < HMCR)
                {
                    newIndex = rand.Next(0,HMtab.GetLength(0));
                    vec[i] = HMtab[newIndex, i];
                    if (rand.NextDouble() < PAR)
                    {
                        if (rand.NextDouble() < 0.5)
                        {
                            double D5 = vec[i] - rand.NextDouble() * BW;
                            if (D5>=PVBmin)
                            {
                                vec[i] = D5; //pitch adjustment
                            }
                        }
                        else
                        {
                            double D5_2 = vec[i] + rand.NextDouble() * BW;
                            if (D5_2<=PVBmax)
                            {
                                vec[i] = D5_2;
                            }
                        }
                    }
                }
                else
                {
                    vec[i]= rand.NextDouble() * (PVBmax - PVBmin) + PVBmin; //randomizacja
                }
            }
            return vec;
        }


        public static void updateHM(double[,] HMtab,Function f ,double[] vec)
        {
            HarmonyTool.performVec(f,vec);
            if (vec[vec.GetLength(0)-1] < HMtab[HMtab.GetLength(0)-1,HMtab.GetLength(1)-1])
            {
                for(int i=0; i< HMtab.GetLength(1); i++)
                {
                    HMtab[HMtab.GetLength(0) - 1, i] = vec[i];
                }
                HarmonyTool.sortHM(HMtab);
            }
        }


        public static void HarmonySearchAlgorithm(Function f,int NI, int HMS, double HMCR, double PAR, double BW, double PVBmin, double PVBmax)
        {
            int numberofVariables = f.getArgumentsNumber();
            double[,] HMtab = new double[HMS, numberofVariables + 1];
            int iterations = 0;
            HarmonyTool.initializeHM(HMtab, PVBmin, PVBmax);
            HarmonyTool.performHM(f,HMtab);
            HarmonyTool.sortHM(HMtab);
            double[] newVec = new double[HMtab.GetLength(1)];
            for (int i = 0; i < NI; i++)
            {
                newVec = HarmonyTool.newHarmonyVector(HMtab, HMCR, PAR, BW, PVBmin, PVBmax);
                HarmonyTool.updateHM(HMtab,f, newVec);
                iterations++;
                Console.WriteLine("iterancja nr: "+i);
                Console.WriteLine("Wait");
            }
            Console.WriteLine("Koniec obliczen");
            Console.WriteLine("Liczba wykonanych iteracji: " + iterations);
            Console.WriteLine("Ostateczna tabela Harmony Memory:");
            HarmonyTool.displayHM(HMtab);

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
            String fn_string = "f(x1,x2)=(4-2.1*x1^2+x1^4/3)*x1^2+x1*x2+(-4+4*x2^2)*x2^2";
            Function fn = new Function(fn_string);
            HarmonyTool.HarmonySearchAlgorithm(fn,NI, HMS, HMCR, PAR, BW, PVBmin, PVBmax);
            Console.WriteLine("");




        }


    }
}
