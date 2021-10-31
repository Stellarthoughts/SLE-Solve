using ExtremumScan;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RBSelector<SLESolvingMethod> methodSelector;

        public MainWindow()
        {
            InitializeComponent();

            methodSelector = new(
                new List<RadioButton>() {rbSimpleIter, rbZeydel, rbLU},
                new List<SLESolvingMethod>() {SLESolver.SimpleIteration, SLESolver.Zeydel, SLESolver.LUDecomp}
                );

            SLE sle = null;
            double[,] mat = null;

            try
            {
                mat = Reader.MatrixFromString<double>(tbInput.Text);    
            }
            catch(Exception)
            {
                
            }

            if(mat != null)
            {
                sle = new SLE(mat.GetLength(0));
                sle.FillFromMatrix(mat);
            }

            SLESolverSettings set = new()
            {
                sle = sle,
                eps = 0.001
            };

            SLESolverResult simple = SLESolver.SimpleIteration(set);
            SLESolverResult zeydel = SLESolver.Zeydel(set);
            SLESolverResult lu = SLESolver.LUDecomp(set);
            double[] verifySimple = sle.VerifySolution(simple.solution);
            double[] verifyZeydel = sle.VerifySolution(zeydel.solution);
            double[] verifyLU = sle.VerifySolution(lu.solution);

            double[,] SLEMat = sle.GetPrimeMatrix();
            double[,] inverseSLEMat = Matrix.Inverse(SLESolver.LUDecomp, SLEMat, 0.0001);
            double[,] unit = Matrix.Multiply(SLEMat, inverseSLEMat);
        }

        private void btnSLE_Click(object sender, RoutedEventArgs e)
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

            StringBuilder str = new StringBuilder();
            if(solved.iterative)
                str.Append("Количество итераций для метода: " + solved.iterations + "\n");
            str.Append("Результат работы:\n");
            str.Append(string.Join(" ", solved.solution.Select(x => x.ToString()).ToArray()) + "\n");
            str.Append("Проверка:\n");
            str.Append(string.Join(" ", sle.VerifySolution(solved.solution).Select(x => x.ToString()).ToArray()));

            tbOutput.Text = str.ToString();
        }

        private void btnInverseMat_Click(object sender, RoutedEventArgs e)
        {
            SLE sle = null;
            double[,] mat = null;
            try
            {
                mat = Reader.MatrixFromString<double>(tbInput.Text);
            }
            catch (Exception)
            {
                tbOutput.Text = "Неверные данные: ошибка считывания матрицы";
                return;
            }
            SLESolvingMethod method = methodSelector.GetChoice();
        }
    }
}
