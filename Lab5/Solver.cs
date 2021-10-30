using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
	public struct SLESolveResult
    {
		public bool convergence;
		public int iterations;
		public double[] solution;
    }
	class Solver
	{
		public static SLESolveResult SLESimpleIteration(SLE sle, double eps)
		{
			SLESolveResult result = new SLESolveResult();
			int n = sle.Dimension;

			// Check convergence
			bool convergenceFromNorm = CheckConvergenceFromNorms(sle);
			result.convergence = convergenceFromNorm;
			if (!convergenceFromNorm) return result;

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
			int iterations = 0;
			do
			{
				iterations++;
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

			result.iterations = iterations;
			result.solution = x;

			return result;
		}

		private static bool CheckConvergenceFromNorms(SLE sle)
		{
			bool convergenceFromNorm = true;

			double firstNormMax = Double.NegativeInfinity;
			double secondNormMax = Double.NegativeInfinity;
			for (int i = 0; i < sle.Dimension; i++) 
			{
				double firstNormNext = 0;
				double secondNormNext = 0;
				for (int j = 0; j < sle.Dimension; j++) 
				{
					if (i == j) continue;
					firstNormNext += Math.Abs(sle.Get(i, j) / sle.Get(i,i));
					secondNormNext += Math.Abs(sle.Get(j, i) / sle.Get(j, j));
				}
				if (firstNormNext > firstNormMax) firstNormMax = firstNormNext;
				if (secondNormNext > secondNormMax) secondNormMax = secondNormNext;
			}

			if (!(firstNormMax < 1 || secondNormMax < 1)) convergenceFromNorm = false;
			return convergenceFromNorm;
		}
	}
}
