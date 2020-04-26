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
using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Series;

namespace HarmonySearchWPFapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private static double[] ParsePVB(String PVB_string, char separator)
        {
            String[] PVB_strTab = PVB_string.Split(separator);
            int tabLength = PVB_strTab.GetLength(0);
            double[] PVB_returnTab = new double[tabLength];
            try
            {
                for(int i=0; i<tabLength; i++)
                {
                    PVB_returnTab[i] = Double.Parse(PVB_strTab[i]);
                }
                return PVB_returnTab;
            }
            catch
            {
                MessageBox.Show("Items in PVB are not correct!");
                return null;
            }
        }

        private void PAR_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PAR_textbox.Text = PAR_slider.Value.ToString();
        }

        private void HMCR_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HMCR_textbox.Text = HMCR_slider.Value.ToString();
        }

        private void HMCR_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                HMCR_slider.Value = Double.Parse(HMCR_textbox.Text);
            }
            catch
            {
                HMCR_slider.Value = 0;
            }
        }

    

        private void PAR_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                PAR_slider.Value = Double.Parse(PAR_textbox.Text);
            }
            catch
            {
                PAR_slider.Value = 0;
            }
        }

        private void Calculate_Btn_Click(object sender, RoutedEventArgs e)
        {
            double HMCR = Double.Parse(HMCR_textbox.Text);
            double PAR = Double.Parse(PAR_textbox.Text);
            int NI = int.Parse(NI_TextBox.Text);
            int HMS = int.Parse(HMS_TextBox.Text);
            double BW = Double.Parse(BW_TextBox.Text);
            String fun_str = ObjFun_ComboBox.Text.ToString();
            double[] PVBmax = ParsePVB(PVBmax_TextBox.Text, ',');
            double[] PVBmin = ParsePVB(PVBmin_TextBox.Text, ',');
            Function fn = new Function(fun_str);
            String result = HarmonyTool.HarmonySearchAlgorithm(fn, NI, HMS, HMCR, PAR, BW, PVBmin, PVBmax);
            Result_TextBox.Text = result;






        }

        private void Reset_Btn_Click(object sender, RoutedEventArgs e)
        {
            HMS_TextBox.Text = null;
            NI_TextBox.Text = null;
            BW_TextBox.Text = null;
            ObjFun_ComboBox.SelectedIndex = 2;
            PVBmax_TextBox.Text = null;
            PVBmin_TextBox.Text = null;
            HMCR_slider.Value = 0;
            PAR_slider.Value = 0;
            HMCR_textbox.Text = null;
            PAR_textbox.Text = null;
            Result_TextBox.Text = null;
           
        }

        private void Plot_btn_Click(object sender, RoutedEventArgs e)
        {

            var tmp = new PlotModel { Title = "Scatter plot", Subtitle = "y = x" };
            var s2 = new LineSeries
            {
                StrokeThickness = 1,
                MarkerSize = 1,
                MarkerStroke = OxyColors.ForestGreen,
                MarkerType = MarkerType.Plus
            };

            for (int i = 0; i < 100; i++)
            {
                s2.Points.Add(new DataPoint(i, i));
            }
            tmp.Series.Add(s2);
            GetMainViewModel().MyModel = tmp;
        }

        // C#6.O
        //public MainViewModel MainViewModel => (MainViewModel)DataContext;
        public MainViewModel GetMainViewModel()
        {
            return (MainViewModel)DataContext;
        }
    }
}
