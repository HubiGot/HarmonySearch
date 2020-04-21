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

        private void PAR_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PAR_textbox.Text = PAR_slider.Value.ToString();
        }

        private void HMCR_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HMCR_textbox.Text = HMCR_slider.Value.ToString();
        }
    }
}
