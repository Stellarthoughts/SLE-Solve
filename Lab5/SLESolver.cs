using System;

namespace Lab5
{
	public struct SLESolverResult
    {
		public bool convergence;
		public int iterations;
		public double[] solution;
    }

	class SLESolver
	{
		public static SLESolverResult SimpleIteration(SLE sle, double eps)
		{
			SLESolverResult result = new SLESolverResult();
			int n = sle.Dimension;

			// Check convergence
			bool convergenceFromNorm = CheckConvergenceNorms(sle);
			result.convergence = convergenceFromNorm;
			if (!convergenceFromNorm) return result;

			// Init all support matricies

			double[] x = new double[n];
			double[,] C = new double[n, n];
			double[] b = new double[n];

			for(int i = 0; i < n; i++)
			{
				b[i] = sle.Get(i, n) / sle.Get(i, i);
				x[i] = b[i];
				for(int j = 0; j < n; j++)
				{
					if(i == j)
					{
						C[i, j] = 0;
					}
					else
					{
						C[i, j] = -sle.Get(i, j) / sle.Get(i, i);
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

					double term = 0;
					for(int j = 0; j < n; j++)
					{
						if (i == j) continue;
						term += -C[i, j] * x[j];
					}

					xNext[i] -= term;
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

		public static SLESolverResult Zeydel(SLE sle, double eps)
		{
			SLESolverResult result = new SLESolverResult();
			int n = sle.Dimension;

			// Check convergence
			bool convergenceFromNorm = CheckConvergenceNorms(sle);
			result.convergence = convergenceFromNorm;
			if (!convergenceFromNorm) return result;

			// Init all support matricies

			double[] x = new double[n];
			double[,] C = new double[n, n];
			double[] b = new double[n];

			for (int i = 0; i < n; i++)
			{
				b[i] = sle.Get(i, n) / sle.Get(i, i);
				x[i] = b[i];
				for (int j = 0; j < n; j++)
				{
					if (i == j)
					{
						C[i, j] = 0;
					}
					else
					{
						C[i, j] = -sle.Get(i, j) / sle.Get(i, i);
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
					double termOne = 0;
					double termTwo = 0;

					for(int j = 0; j < i; j++)
                    {
						termOne += -C[i, j] * xNext[j];
                    }

					for(int j = i + 1; j < n; j++)
                    {
						termTwo += -C[i, j] * x[j];
                    }

					xNext[i] -= termOne + termTwo;
				}

				// Check
				epsReahced = true;
				for (int i = 0; i < n; i++)
				{
					if (Math.Abs(xNext[i] - x[i]) >= eps)
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



		private static bool CheckConvergenceNorms(SLE sle)
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
