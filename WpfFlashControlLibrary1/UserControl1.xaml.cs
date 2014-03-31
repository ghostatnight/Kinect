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
using AxShockwaveFlashObjects;
using System.Windows.Forms.Integration;

namespace WpfFlashControlLibrary1
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void flashGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Load flash game
            WindowsFormsHost formHost = new WindowsFormsHost();
            AxShockwaveFlash axShockwaveFlash = new AxShockwaveFlash();

            formHost.Child = axShockwaveFlash;
            flashGrid.Children.Add(formHost);

            string flashPath = Environment.CurrentDirectory;
            flashPath += @"\game.swf";
            axShockwaveFlash.Movie = flashPath;
            axShockwaveFlash.Play();

        }
    }
}
