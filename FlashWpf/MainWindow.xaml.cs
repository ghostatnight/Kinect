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
using System.Windows.Forms.Integration;
using AxShockwaveFlashObjects;

namespace FlashWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowsFormsHost formHost = new WindowsFormsHost();

            AxShockwaveFlash axShockwaveFlash = new AxShockwaveFlash();

            formHost.Child = axShockwaveFlash;

            mainGrid.Children.Add(formHost);

            string flashPath = Environment.CurrentDirectory;
            flashPath += @"\game.swf";

            axShockwaveFlash.Movie = flashPath;

        }
    }
}
