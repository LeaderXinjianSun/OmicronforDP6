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
using BingLibrary.hjb;

namespace OmicronforDP6.View
{
    /// <summary>
    /// Parameter.xaml 的交互逻辑
    /// </summary>
    public partial class Parameter : UserControl
    {
        private string iniParameterPath = System.Environment.CurrentDirectory + "\\Parameter.ini";
        public Parameter()
        {
            InitializeComponent();
        }
        private void TextBox1_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.TextBox1.IsReadOnly = false;
        }

        private void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            this.TextBox1.IsReadOnly = true;
            try
            {
                Inifile.INIWriteValue(iniParameterPath, "SQLMSG", "BLID", TextBox1.Text);
            }
            catch
            {


            }
        }

        private void TextBox3_LostFocus(object sender, RoutedEventArgs e)
        {
            this.TextBox3.IsReadOnly = true;
            try
            {
                Inifile.INIWriteValue(iniParameterPath, "SQLMSG", "BLMID", TextBox3.Text);
            }
            catch
            {


            }
        }

        private void TextBox3_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.TextBox3.IsReadOnly = false;

        }

        private void TextBox2_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.TextBox2.IsReadOnly = false;
        }

        private void TextBox2_LostFocus(object sender, RoutedEventArgs e)
        {
            this.TextBox2.IsReadOnly = true;
            try
            {
                Inifile.INIWriteValue(iniParameterPath, "SQLMSG", "BLUID", TextBox2.Text);
            }
            catch
            {


            }
        }

        private void TextBox4_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.TextBox4.IsReadOnly = false;
        }

        private void TextBox4_LostFocus(object sender, RoutedEventArgs e)
        {
            this.TextBox4.IsReadOnly = true;
            try
            {
                Inifile.INIWriteValue(iniParameterPath, "SQLMSG", "BLNAME", TextBox4.Text);
            }
            catch
            {


            }
        }
    }
}
