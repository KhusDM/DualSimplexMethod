using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DualSimplexMethod
{
    class Program
    {
        static private Regex patternOfFunction = new Regex(@"[+-]?[0-9]*");

        static private IList<double> ParseFunction(string function, out IList<double> coefficientOfFunction)
        {
            MatchCollection collection = patternOfFunction.Matches(function);
            coefficientOfFunction = new List<double>();

            foreach (Match match in collection)
            {
                if (match.Value != String.Empty)
                {
                    if (match.ToString() == "+")
                    {
                        coefficientOfFunction.Add(1d);
                    }
                    else if (match.Value == "-")
                    {
                        coefficientOfFunction.Add(-1d);
                    }
                    else if (match.Value.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) != -1)
                    {
                        coefficientOfFunction.Add(double.Parse(match.Value));
                    }
                }
            }

            return coefficientOfFunction;
        }

        static IList<double> InputFunction()
        {
            bool flag = false;
            IList<double> coefficientOfFunction = new List<double>();
            string function = Console.ReadLine();
            while (!flag)
            {
                try
                {
                    ParseFunction(function, out coefficientOfFunction);
                    flag = true;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to recognize the function: {function}\nRepeat again...");
                }
            }

            return coefficientOfFunction;
        }

        static private void PrintSimplexMatrix(IList<double> coefficientOfFunction, IList<List<double>> simplexMatrixCoefficients, IList<double> deltMatrix, IList<string> tetaMatrix)
        {
            string s = "B | CN | Xb | ";
            int n = coefficientOfFunction.Count;
            for (int i = 0; i < n; i++)
            {
                s += coefficientOfFunction[i].ToString() + " | ";
            }
            Console.WriteLine(s);

            foreach (IList<double> list in simplexMatrixCoefficients)
            {
                s = "";
                foreach (double item in list)
                {
                    s += item + " | ";
                }
                Console.WriteLine(s);
            }

            s = " |  | delt | ";
            foreach (double item in deltMatrix)
            {
                s += item + " | ";
            }
            Console.WriteLine();

            s = " |  | teta | ";
            foreach (string item in tetaMatrix)
            {
                s += item + " | ";
            }
            Console.WriteLine();
        }

        static private bool CalcSimplexMatrix(IList<double> coefficientOfFunction, ref IList<List<double>> simplexMatrixCoefficients, ref IList<double> deltMatrix, ref IList<string> tetaMatrix)
        {
            bool flagIteration = true;
            int n = simplexMatrixCoefficients.Count, l = 0;
            double minB = double.MaxValue, minA = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                if (simplexMatrixCoefficients[i][2] < minB)
                {
                    minB = simplexMatrixCoefficients[i][2];
                    l = i;
                }
            }

            if (minB < 0)
            {
                n = simplexMatrixCoefficients[l].Count;
                for (int i = 0; i < n; i++)
                {
                    if (simplexMatrixCoefficients[l][i] < minA)
                    {
                        minA = simplexMatrixCoefficients[l][i];
                    }
                }

                if (minA > 0)
                {
                    Console.WriteLine("Has no solution!");
                    return flagIteration = false;
                }
            }
            else
            {
                double f = 0;
                n = simplexMatrixCoefficients.Count;
                for (int i = 0; i < n; i++)
                {
                    f += simplexMatrixCoefficients[i][1] * simplexMatrixCoefficients[i][2];
                }

                Console.WriteLine("Solution found!");
                Console.WriteLine($"f: {f}");
                return flagIteration = false;
            }

            n = deltMatrix.Count;
            int m = simplexMatrixCoefficients.Count;
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum = 0;
                for (int j = 0; j < m; j++)
                {
                    sum += simplexMatrixCoefficients[j][1] * simplexMatrixCoefficients[j][i + 3];
                }
                deltMatrix[i] = sum - coefficientOfFunction[i];
                if (simplexMatrixCoefficients[l][i + 3] != 0)
                {
                    tetaMatrix[i] = (deltMatrix[i] / (-simplexMatrixCoefficients[l][i + 3])).ToString();
                }
                else
                {
                    tetaMatrix[i] = "-";
                }
            }

            n = tetaMatrix.Count;
            int k = 0;
            double minTeta = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                if (tetaMatrix[i] != "-")
                {
                    if (double.Parse(tetaMatrix[i]) < minTeta)
                    {
                        k = i;
                    }
                }
            }

            n = simplexMatrixCoefficients[k].Count;
            simplexMatrixCoefficients[l][0] = k + 1;
            simplexMatrixCoefficients[l][1] = deltMatrix[k];
            for (int i = 2; i < n; i++)
            {
                simplexMatrixCoefficients[l][i + 3] /= simplexMatrixCoefficients[l][k + 3];
            }

            n = simplexMatrixCoefficients.Count;
            m = simplexMatrixCoefficients[0].Count();
            for (int i = 0; i < n; i++)
            {
                if (i != l)
                {
                    for (int j = 2; j < m; j++)
                    {
                        if (simplexMatrixCoefficients[i][k + 3] > 0)
                        {
                            simplexMatrixCoefficients[i][j] -= simplexMatrixCoefficients[l][j] * simplexMatrixCoefficients[i][k + 3];
                        }
                        else if (simplexMatrixCoefficients[i][k + 3] < 0)
                        {
                            simplexMatrixCoefficients[i][j] += simplexMatrixCoefficients[l][j] * simplexMatrixCoefficients[i][k + 3];
                        }
                    }
                }
            }

            return flagIteration;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter function:");
            IList<double> coefficientOfFunction;
            coefficientOfFunction = InputFunction();
            int numberOfCoefficient = coefficientOfFunction.Count;

            Console.WriteLine("Enter the number of terms:");
            int numberOfTerms = 0;
            bool flag = false;
            while (!flag)
            {
                try
                {
                    numberOfTerms = int.Parse(Console.ReadLine());
                    flag = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("incorrect\nRepeat again...");
                }
            }

            Console.WriteLine("Enter the terms:");
            IList<List<double>> canonicalMatrix = new List<List<double>>(numberOfTerms);
            for (int i = 0; i < numberOfTerms; i++)
            {
                canonicalMatrix.Add((List<double>)InputFunction());
                coefficientOfFunction.Add(0d);
            }

            IList<List<double>> simplexMatrixCoefficients = new List<List<double>>(numberOfTerms);
            for (int i = 0; i < numberOfTerms; i++)
            {
                canonicalMatrix[i].Insert(0, canonicalMatrix[0].Last());
                canonicalMatrix[i].RemoveAt(canonicalMatrix[i].Count - 1);
                canonicalMatrix[i].Insert(0, 0);
                canonicalMatrix[i].Insert(0, numberOfCoefficient + (i + 1));
            }

            bool solved = false;
            IList<double> deltMatrix=new List<double>();
            IList<string> tetaMatrix=new List<string>();
            int n = coefficientOfFunction.Count;
            for (int i = 0; i < n; i++)
            {
                deltMatrix.Add(0);
                tetaMatrix.Add("-");
            }

            PrintSimplexMatrix(coefficientOfFunction, simplexMatrixCoefficients, deltMatrix, tetaMatrix);
            while (CalcSimplexMatrix(coefficientOfFunction, ref simplexMatrixCoefficients, ref deltMatrix, ref tetaMatrix))
            {
                PrintSimplexMatrix(coefficientOfFunction, simplexMatrixCoefficients, deltMatrix, tetaMatrix);
            }
        }
    }
}
