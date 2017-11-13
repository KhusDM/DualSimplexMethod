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

        static private void ParseFunction(string function, out IList<double> coefficientsOfFunction)
        {
            MatchCollection collection = patternOfFunction.Matches(function);
            coefficientsOfFunction = new List<double>();

            foreach (Match match in collection)
            {
                if (match.Value != String.Empty)
                {
                    if (match.Value == "+")
                    {
                        coefficientsOfFunction.Add(1d);
                    }
                    else if (match.Value == "-")
                    {
                        coefficientsOfFunction.Add(-1d);
                    }
                    else if (match.Value.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) != -1)
                    {
                        coefficientsOfFunction.Add(double.Parse(match.Value));
                    }
                }
            }
        }

        static private IList<double> InputFunction()
        {
            bool flag = false;
            IList<double> coefficientsOfFunction = new List<double>();
            string function = Console.ReadLine();
            while (!flag)
            {
                try
                {
                    ParseFunction(function, out coefficientsOfFunction);
                    flag = true;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to recognize the function: {function}\nRepeat again...");
                }
            }

            return coefficientsOfFunction;
        }

        static private void PrintSimplexMatrix(IList<double> coefficientsOfFunction, IList<List<double>> simplexMatrixCoefficients, IList<double> deltaMatrix, IList<string> tetaMatrix)
        {
            string s = String.Format(" {0,6} | {1,6} | {2,6} |", "B", "CN", "Xb");
            int n = coefficientsOfFunction.Count;
            for (int i = 0; i < n; i++)
            {
                s += String.Format(" {0,6:F2} |", coefficientsOfFunction[i]);
            }
            Console.WriteLine(s);
            Console.WriteLine(new String('-', s.Length));

            foreach (IList<double> list in simplexMatrixCoefficients)
            {
                s = "";
                foreach (double item in list)
                {
                    s += String.Format(" {0,6:F2} |", item);
                }
                Console.WriteLine(s);
            }
            Console.WriteLine(new String('-', s.Length));

            s = String.Format(" {0,6} | {1,6} | {2,6} |", "", "", "delta");
            foreach (double item in deltaMatrix)
            {
                s += String.Format(" {0,6:F2} |", item);
            }
            Console.WriteLine(s);

            s = String.Format(" {0,6} | {1,6} | {2,6} |", "", "", "teta");
            foreach (string item in tetaMatrix)
            {
                if (item != "-")
                {
                    s += String.Format(" {0,6:F2} |", double.Parse(item));
                }
                else
                {
                    s += String.Format(" {0,6} |", item);
                }
            }
            Console.WriteLine(s);
            Console.WriteLine("\n");
        }

        static private bool Check(IList<double> coefficientOfFunction, IList<List<double>> simplexMatrixCoefficients, ref int l)
        {
            int n = simplexMatrixCoefficients.Count, m = simplexMatrixCoefficients[0].Count;
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
                for (int i = 3; i < m; i++)
                {
                    if (simplexMatrixCoefficients[l][i] < minA)
                    {
                        minA = simplexMatrixCoefficients[l][i];
                    }
                }

                if (minA >= 0)
                {
                    Console.WriteLine("Has no solution!");
                    return false;
                }
            }
            else
            {
                double f = 0;
                for (int i = 0; i < n; i++)
                {
                    f += simplexMatrixCoefficients[i][1] * simplexMatrixCoefficients[i][2];
                }

                n = coefficientOfFunction.Count;
                double[] resVector = new double[n];
                for (int i = 0; i < n; i++)
                {
                    resVector[i] = 0;
                }

                for (int i = 0; i < simplexMatrixCoefficients.Count; i++)
                {
                    resVector[Convert.ToInt32(simplexMatrixCoefficients[i][0]) - 1] = simplexMatrixCoefficients[i][2];
                }

                Console.WriteLine("Solution found!");
                Console.WriteLine($"f = {-f}");
                Console.WriteLine("\n");

                string s = "X=(";
                for (int i = 0; i < n; i++)
                {
                    if (i == n - 1)
                    {
                        s += resVector[i].ToString() + ")";
                    }
                    else
                    {
                        s += resVector[i].ToString()+"; ";
                    }
                }
                Console.WriteLine(s);

                return false;
            }

            return true;
        }

        static private void JordanGaussMethod(IList<double> coefficientOfFunction, IList<List<double>> simplexMatrixCoefficients, ref IList<double> deltaMatrix, ref IList<string> tetaMatrix, int l)
        {
            int n = coefficientOfFunction.Count, m = simplexMatrixCoefficients.Count;
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum = 0;
                for (int j = 0; j < m; j++)
                {
                    sum += simplexMatrixCoefficients[j][1] * simplexMatrixCoefficients[j][i + 3];
                }
                deltaMatrix[i] = sum - coefficientOfFunction[i];

                if (simplexMatrixCoefficients[l][i + 3] < 0)
                {
                    tetaMatrix[i] = (deltaMatrix[i] / (-simplexMatrixCoefficients[l][i + 3])).ToString();
                }
                else
                {
                    tetaMatrix[i] = "-";
                }
            }
        }

        static private void GaussMethod(ref IList<List<double>> simplexMatrixCoefficients, int l, int k)
        {
            int n = simplexMatrixCoefficients.Count, m = simplexMatrixCoefficients[0].Count();
            for (int i = 0; i < n; i++)
            {
                if (i != l)
                {
                    double coefficient = simplexMatrixCoefficients[i][k + 3];
                    if (coefficient != 0)
                    {
                        for (int j = 2; j < m; j++)
                        {
                            simplexMatrixCoefficients[i][j] -= simplexMatrixCoefficients[l][j] * coefficient;
                        }
                    }
                }
            }
        }

        static private void CalcSimplexMatrix(IList<double> coefficientOfFunction, ref IList<List<double>> simplexMatrixCoefficients, ref IList<double> deltaMatrix, ref IList<string> tetaMatrix, ref int l)
        {
            int n = tetaMatrix.Count, k = 0;
            double minTeta = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                if (tetaMatrix[i] != "-")
                {
                    if (double.Parse(tetaMatrix[i]) < minTeta)
                    {
                        minTeta = double.Parse(tetaMatrix[0]);
                        k = i;
                    }
                }
            }

            n = simplexMatrixCoefficients[l].Count;
            simplexMatrixCoefficients[l][0] = k + 1;
            simplexMatrixCoefficients[l][1] = coefficientOfFunction[k];
            double lkElement = simplexMatrixCoefficients[l][k + 3];
            for (int i = 2; i < n; i++)
            {
                simplexMatrixCoefficients[l][i] /= lkElement;
            }

            GaussMethod(ref simplexMatrixCoefficients, l, k);

            JordanGaussMethod(coefficientOfFunction, simplexMatrixCoefficients, ref deltaMatrix, ref tetaMatrix, l);

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

            IList<List<double>> simplexMatrixCoefficients = canonicalMatrix;
            for (int i = 0; i < numberOfTerms; i++)
            {
                simplexMatrixCoefficients[i].Insert(0, simplexMatrixCoefficients[i].Last());
                simplexMatrixCoefficients[i].RemoveRange(simplexMatrixCoefficients[i].Count - 2, 2);
                for (int j = 0; j < numberOfTerms; j++)
                {
                    if (j == i)
                    {
                        simplexMatrixCoefficients[i].Add(1d);
                    }
                    else
                    {
                        simplexMatrixCoefficients[i].Add(0d);
                    }

                }
                simplexMatrixCoefficients[i].Insert(0, 0);
                simplexMatrixCoefficients[i].Insert(0, numberOfCoefficient + (i + 1));
            }

            IList<double> deltaMatrix = new List<double>();
            IList<string> tetaMatrix = new List<string>();
            for (int i = 0; i < coefficientOfFunction.Count; i++)
            {
                deltaMatrix.Add(0);
                tetaMatrix.Add("-");
            }
            int l = 0;
            Check(coefficientOfFunction, simplexMatrixCoefficients, ref l);
            JordanGaussMethod(coefficientOfFunction, simplexMatrixCoefficients, ref deltaMatrix, ref tetaMatrix, l);
            PrintSimplexMatrix(coefficientOfFunction, simplexMatrixCoefficients, deltaMatrix, tetaMatrix);

            while (true)
            {
                if (!Check(coefficientOfFunction, simplexMatrixCoefficients, ref l)) break;
                CalcSimplexMatrix(coefficientOfFunction, ref simplexMatrixCoefficients, ref deltaMatrix, ref tetaMatrix, ref l);
                PrintSimplexMatrix(coefficientOfFunction, simplexMatrixCoefficients, deltaMatrix, tetaMatrix);
            }

            Console.ReadKey();
        }
    }
}
