using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    public class Matrix
    {
        public static double[,] Inverse(SolveSLE method, double[,] mat, double eps)
        {
            int N = mat.GetLength(0);
            int M = mat.GetLength(1);
            if (N != M) return null;

            double[,] inverse = new double[N,N];
            for(int j = 0; j < N; j++)
            {
                SLE row = new SLE(N);
                row.FillFromMatrix(mat);
                row.Set(j, row.Dimension, 1);
                SLESolverResult solved = method(new SLESolverSettings()
                {
                    sle = row,
                    eps = eps
                });
                for(int i = 0; i < N; i++)
                    inverse[i,j] = solved.solution[i];
            }
            return inverse;
        }

        public static double[,] Multiply(double[,] left, double[,] right)
        {
            if (left.GetLength(1) != right.GetLength(0)) return null;
            double[,] multiple = new double[left.GetLength(0), right.GetLength(1)];

            for(int i = 0; i < left.GetLength(0); i++)
            {
                for(int j = 0; j < right.GetLength(1); j++)
                {
                    double sum = 0;
                    for(int k = 0; k < left.GetLength(1); k++)
                    {
                        sum += left[i, k] * right[k, j];
                    }
                    multiple[i, j] = sum;
                }
            }

            return multiple;
        }
    }
}
