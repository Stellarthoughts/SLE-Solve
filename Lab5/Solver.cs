using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
	class Solver
	{
		public static double[] SLESimpleIteration(SLE sle, double eps)
		{
			int n = sle.Dimension;

			// Check convergence

			// Norm
			bool convergenceFromNorm = CheckConvergenceFromNorms(sle);
			if(!convergenceFromNorm) SleGetMaxOnDiagonals(sle);
			convergenceFromNorm = CheckConvergenceFromNorms(sle);

			// Another method
			if(!convergenceFromNorm)
            {

            }

			// Init all support matricies

			double[] x = new double[n];
			double[,] B = new double[n, n];
			double[] b = new double[n];

			for(int i = 0; i < n; i++)
			{
				b[i] = sle.Get(i, n) / sle.Get(i, i);
				x[i] = b[i];
				for(int j = 0; j < n; j++)
				{
					if(i == j)
					{
						B[i, j] = 0;
					}
					else
					{
						B[i, j] = -sle.Get(i, j) / sle.Get(i, i);
					}
				}
			}     

			// Calculate
			bool epsReahced = true;
			do
			{
				// Next iteration
				double[] xNext = new double[n];
				for (int i = 0; i < n; i++)
				{
					xNext[i] = b[i];
					for(int j = 0; j < n; j++)
					{
						if (i == j) continue;
						xNext[i] += B[i, j] * x[j];
					}
				}

				// Check
				epsReahced = true;
				for (int i = 0; i < n; i++)
				{
					if(Math.Abs(xNext[i] - x[i]) >= eps)
					{
						epsReahced = false;
						break;
					}
				}

				x = xNext;
			}
			while (!epsReahced);

			return x;
		}

		private static bool CheckConvergenceFromNorms(SLE sle)
		{
			bool convergenceFromNorm = true;

			double firstNormMax = Double.NegativeInfinity;
			for (int i = 0; i < sle.Dimension; i++) {
				double firstNormNext = 0;
				for (int j = 0; j < sle.Dimension; j++) {
					if (i == j) continue;
					firstNormNext += Math.Abs(sle.Get(i, j) / sle.Get(i,i));
				}
				if (firstNormNext > firstNormMax) firstNormMax = firstNormNext;
			}

			double secondNormMax = Double.NegativeInfinity;
			for (int j = 0; j < sle.Dimension; j++) {
				double secondNormNext = 0;
				for (int i = 0; i < sle.Dimension; i++) {
					if (i == j) continue;
					secondNormNext += Math.Abs(sle.Get(i, j) / sle.Get(j, j));
				}
				if (secondNormNext > secondNormMax) secondNormMax = secondNormNext;
			}

			if (!(firstNormMax < 1 || secondNormMax < 1)) convergenceFromNorm = false;
			return convergenceFromNorm;
		}

		private static void SleGetMaxOnDiagonals(SLE sle)
        {

        }
	}
}
