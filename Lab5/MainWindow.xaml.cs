using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            SLE sle = null;
            double[,] mat = null;

            try
            {
                mat = Reader.MatrixFromString(tbInput.Text);    
            }
            catch(Exception)
            {
                Console.WriteLine("Некоректные данные");
            }

            if(mat != null)
            {
                sle = new SLE(mat.GetLength(0));
                sle.FillFromMatrix(mat);
            }

            SLESolverResult simple = SLESolver.SimpleIteration(sle, 0.001);
            SLESolverResult zeydel = SLESolver.Zeydel(sle, 0.001);
            double[] verifySimple = sle.VerifySolution(simple.solution);
            double[] verifyZeydel = sle.VerifySolution(zeydel.solution);
        }
    }
}
