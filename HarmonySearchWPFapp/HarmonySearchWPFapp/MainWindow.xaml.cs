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
using OxyPlot.Axes;
using OxyPlot.Series;

namespace HarmonySearchWPFapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //array to store best solution from HS algorith, to pass to plot function
        double[] bestSolution = new double[2];

        public MainWindow()
        {
            InitializeComponent();
        }

        private static double[] ParsePVB(String PVB_string, char separator)
        {
            String[] PVB_strTab = PVB_string.Split(separator);
            int tabLength = PVB_strTab.GetLength(0);
            double[] PVB_returnTab = new double[tabLength];
            for(int i=0; i<tabLength; i++)
            {
                PVB_returnTab[i] = Double.Parse(PVB_strTab[i]);
            }
            return PVB_returnTab;

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
            try
            {
                double HMCR = Double.Parse(HMCR_textbox.Text);
                double PAR = Double.Parse(PAR_textbox.Text);
                int NI = int.Parse(NI_TextBox.Text);
                int HMS = int.Parse(HMS_TextBox.Text);
                double BW = Double.Parse(BW_TextBox.Text);
                String fun_str = ObjFun_ComboBox.Text.ToString();
                double[] PVBmax = ParsePVB(PVBmax_TextBox.Text, ';');
                double[] PVBmin = ParsePVB(PVBmin_TextBox.Text, ';');
                Function fn = new Function(fun_str);
                String result = HarmonyTool.HarmonySearchAlgorithm(fn, NI, HMS, HMCR, PAR, BW, PVBmin, PVBmax, ref bestSolution);
                Result_TextBox.Text = result;
            }
            catch(Exception ex)
            {
                MessageBox.Show("A handled exception just occurred: " + ex.Message +"\n Check if forms are filled properly!", "Exception", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
            try
            {
                var tmp = new PlotModel { Title = "Contour Plot", Subtitle = "f(x1,x2)" };

                tmp.Axes.Add(new LinearColorAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Right,
                    //Palette = OxyPalettes.Jet(500)
                    Palette = OxyPalettes.Rainbow(100)
                });

                double[] PVBmax = ParsePVB(PVBmax_TextBox.Text, ';');
                double[] PVBmin = ParsePVB(PVBmin_TextBox.Text, ';');
                String fun_str = ObjFun_ComboBox.Text.ToString();
                Function fn = new Function(fun_str);
                double x1_min;
                double x1_max;
                double x2_min;
                double x2_max;
                double tmpMaxValue = Double.MinValue;

                if (PVBmin.GetLength(0) == 2)
                {
                    x1_min = PVBmin[0];
                    x1_max = PVBmax[0];
                    x2_min = PVBmin[1];
                    x2_max = PVBmax[1];
                    var x1x1 = ArrayBuilder.CreateVector(x1_min, x1_max, 100);
                    var x2x2 = ArrayBuilder.CreateVector(x2_min, x2_max, 100);
                    double[,] peaksData = new double[x1x1.GetLength(0), x2x2.GetLength(0)];
                    double[] xy_tab = new double[2];

                    for (int i = 0; i < x1x1.GetLength(0); i++)
                    {
                        for (int j = 0; j < x2x2.GetLength(0); j++)
                        {
                            xy_tab[0] = x1x1[i];
                            xy_tab[1] = x2x2[j];
                            peaksData[i, j] = HarmonyTool.EvaluateFun(fn, xy_tab);
                            if (peaksData[i, j] > tmpMaxValue)
                            {
                                tmpMaxValue = peaksData[i, j];
                            }
                        }
                    }


                    var heatMapSeries = new HeatMapSeries
                    {
                        X0 = x1_min,
                        X1 = x1_max,
                        Y0 = x2_min,
                        Y1 = x2_max,
                        Interpolate = true,
                        RenderMethod = HeatMapRenderMethod.Bitmap,
                        Data = peaksData
                    };
                    tmp.Series.Add(heatMapSeries);


                    var cs = new ContourSeries
                    {
                        Color = OxyColors.Black,
                        LabelBackground = OxyColors.White,
                        ColumnCoordinates = x1x1,
                        RowCoordinates = x2x2,
                        Data = peaksData
                    };
                    tmp.Series.Add(cs);

                    if (solutionCheckBox.IsChecked == true)
                    {
                        var sc = new ScatterSeries
                        {
                            MarkerType = MarkerType.Circle
                        };
                        sc.BinSize = 10;
                        sc.MarkerType = MarkerType.Cross;
                        sc.MarkerStrokeThickness = 3;
                        sc.MarkerStroke = OxyColors.Black;
                        double x1 = bestSolution[0];
                        double x2 = bestSolution[1];
                        sc.Points.Add(new OxyPlot.Series.ScatterPoint(x1, x2, 5, tmpMaxValue));
                        tmp.Series.Add(sc);
                    }
                    GetMainViewModel().MyModel = tmp;
                }
                else
                {
                    MessageBox.Show("Number of variables does not equal n=2. Ploting only for n=2", "Exception",MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("A handled exception just occurred: " + ex.Message + "\n Check if forms are filled properly! Domain of function has to be set in PVB forms", "Exception", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // C#6.O
        //public MainViewModel MainViewModel => (MainViewModel)DataContext;
        public MainViewModel GetMainViewModel()
        {
            return (MainViewModel)DataContext;
        }
    }
}
