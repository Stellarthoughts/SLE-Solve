using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RBSelector<SLESolvingMethod> methodSelector;
        private int digitsRounding;

        public MainWindow()
        {
            InitializeComponent();

            methodSelector = new(
                new List<RadioButton>() {rbSimpleIter, rbZeydel, rbLU},
                new List<SLESolvingMethod>() {SLESolver.SimpleIteration, SLESolver.Zeydel, SLESolver.LUDecomp}
                );
        }

        private void BtnSLE_Click(object sender, RoutedEventArgs e)
        {
            SLE sle = null;
            double eps = 0;
            double[,] mat = null;

            try
            {
                mat = Reader.MatrixFromString<double>(tbInput.Text);
            }
            catch (Exception)
            {
                tbOutput.Text = "Неверные данные: ошибка считывания матрицы.";
                return;
            }

            try
            {
                eps = Double.Parse(tbEps.Text, CultureInfo.InvariantCulture);
            }
            catch(Exception)
            {
                tbOutput.Text = "Неверные данные: ошибка считывания точности.";
                return;
            }

            if (mat != null)
            {
                sle = new SLE(mat.GetLength(0));
                sle.FillFromMatrix(mat);
            }
            else
            {
                tbOutput.Text = "Не удалось считать матрицу.";
                return;
            }

            SLESolvingMethod method = methodSelector.GetChoice();
            SLESolverSettings settings = new()
            {
                sle = sle,
                eps = eps
            };
            SLESolverResult solved = method(settings);

            if(!solved.convergence)
            {
                tbOutput.Text = "Метод не сходится для данной СЛАУ.";
                return;
            }

            StringBuilder str = new();
            if(solved.iterative)
                str.Append("Количество итераций для метода: " + solved.iterations + "\n");
            str.Append("Результат работы:\n");
            str.Append(string.Join(" ", solved.solution.Select(x => Math.Round(x, digitsRounding).ToString()).ToArray()) + "\n");
            str.Append("Проверка:\n");
            str.Append(string.Join(" ", sle.VerifySolution(solved.solution).Select(x => Math.Round(x, digitsRounding).ToString()).ToArray()));

            tbOutput.Text = str.ToString();
        }

        private void BtnInverseMat_Click(object sender, RoutedEventArgs e)
        {
            SLE sle;
            double eps;
            double[,] mat;

            try
            {
                mat = Reader.MatrixFromString<double>(tbInput.Text);
            }
            catch (Exception)
            {
                tbOutput.Text = "Неверные данные: ошибка считывания матрицы.";
                return;
            }

            try
            {
                eps = Double.Parse(tbEps.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                tbOutput.Text = "Неверные данные: ошибка считывания точности.";
                return;
            }

            if (mat != null)
            {
                sle = new SLE(mat.GetLength(0));
                sle.FillFromMatrix(mat);
            }
            else
            {
                tbOutput.Text = "Не удалось считать матрицу.";
                return;
            }

            SLESolvingMethod method = methodSelector.GetChoice();

            double[,] SLEMat = sle.GetPrimeMatrix();
            double[,] inverseSLEMat = null;
            double[,] unit = null;

            try
            {
                inverseSLEMat = Matrix.Inverse(method, SLEMat, eps);
            }
            catch(Exception ex)
            {
                tbOutput.Text = ex.Message;
                return;
            }
            
            try
            {
                unit = Matrix.Multiply(SLEMat, inverseSLEMat);
            }
            catch(Exception ex)
            {
                tbOutput.Text = ex.Message;
                return;
            }

            StringBuilder str = new();
            str.Append("Оригинальная матрица:\n");
            str.Append(Matrix.Stringify(SLEMat, digitsRounding) + "\n");
            str.Append("Обратная матрица:\n");
            str.Append(Matrix.Stringify(inverseSLEMat, digitsRounding) + "\n");
            str.Append("Проверка:\n");
            str.Append(Matrix.Stringify(unit, digitsRounding));

            tbOutput.Text = str.ToString();
        }

        private void SlDigits_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            digitsRounding = (int) (sender as Slider).Value;
            if(lblDigits != null) lblDigits.Content = "Знаков после запятой: " + digitsRounding;
        }
    }
}
