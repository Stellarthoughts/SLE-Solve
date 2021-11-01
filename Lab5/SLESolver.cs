using System;

namespace Lab5
{
	public struct SLESolverResult
    {
		public bool convergence;
		public bool iterative;
		public int iterations;
		public double[] solution;
    }

	public struct SLESolverSettings
    {
		public SLE sle;
		public double eps;
    }

	public delegate SLESolverResult SLESolvingMethod(SLESolverSettings set);

	class SLESolver
	{	
		public static SLESolverResult SimpleIteration(SLESolverSettings set)
		{
			SLE sle = set.sle;
			double eps = set.eps;

			SLESolverResult result = new();
			int n = sle.Dimension;
			result.iterative = true;

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
			bool epsReahced;
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
					for (int j = 0; j < n; j++)
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

		public static SLESolverResult Zeydel(SLESolverSettings set)
		{
			SLE sle = set.sle;
			double eps = set.eps;

			SLESolverResult result = new();
			int n = sle.Dimension;
			result.iterative = true;

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
			bool epsReahced;
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

					for (int j = 0; j < i; j++)
					{
						termOne += -C[i, j] * xNext[j];
					}

					for (int j = i + 1; j < n; j++)
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

		public static SLESolverResult LUDecomp(SLESolverSettings set)
		{
			SLE sle = set.sle;

			SLESolverResult result = new();
			result.iterative = false;

			bool convergence = CheckConvergenceMainMinors(sle);
			result.convergence = convergence;
			if (!convergence) return result;

			int N = sle.Dimension;

			double[,] u = new double[N, N];
			double[,] l = new double[N, N];

			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					if (i > j) continue;
					u[i, j] = sle.Get(i, j);
					for (int k = 0; k < i; k++)
					{
						u[i, j] -= l[i, k] * u[k, j];
					}
				}

				for (int j = 0; j <= i; j++)
				{
					if (j == i)
					{
						l[i, j] = 1;
						continue;
					}
					double term = 0;
					for (int k = 0; k < j; k++)
					{
						term += l[i, k] * u[k, j];
					}
					l[i, j] = (sle.Get(i, j) - term) / u[j, j];
				}
			}

			double[] y = new double[N];

			for (int i = 0; i < N; i++)
			{
				double term = 0;
				for (int k = 0; k < i; k++)
				{
					term += l[i, k] * y[k];
				}
				y[i] = sle.Get(i, N) - term;
			}


			double[] x = new double[N];
			for (int i = N - 1; i >= 0; i--)
			{
				double term = 0;
				for (int k = i + 1; k < N; k++)
				{
					term += u[i, k] * x[k];
				}
				x[i] = (y[i] - term) / u[i, i];
			}

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
					firstNormNext += Math.Abs(sle.Get(i, j) / sle.Get(i, i));
					secondNormNext += Math.Abs(sle.Get(j, i) / sle.Get(j, j));
				}
				if (firstNormNext > firstNormMax) firstNormMax = firstNormNext;
				if (secondNormNext > secondNormMax) secondNormMax = secondNormNext;
			}

			if (!(firstNormMax < 1 || secondNormMax < 1)) convergenceFromNorm = false;
			return convergenceFromNorm;
		}

		private static bool CheckConvergenceMainMinors(SLE sle)
		{
			double[,] mat = sle.GetPrimeMatrix();
			for (int k = 1; k <= sle.Dimension; k++)
				for (int i = 0; i < k; i++)
					for (int j = 0; j < k; j++)
						if (mat[i, j] != 0) return true;
			return false;
		}
	}
}
