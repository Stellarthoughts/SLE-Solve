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
            SLE sle = new SLE(3);
            sle.MakeRandom(-5,5);

            sle.Set(0, 0, 10);
            sle.Set(0, 1, 1);
            sle.Set(0, 2, -1);

            sle.Set(1, 0, 1);
            sle.Set(1, 1, 10);
            sle.Set(1, 2, -1);

            sle.Set(2, 0, -1);
            sle.Set(2, 1, 1);
            sle.Set(2, 2, 10);

            sle.Set(0, 3, 11);
            sle.Set(1, 3, 10);
            sle.Set(2, 3, 10);

            double[] solved = Solver.SLESimpleIteration(sle, 0.001);
        }
    }
}
