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
using System.Windows.Shapes;
using System.IO;

namespace GeneticAlgorithm
{
    /// <summary>
    /// Логика взаимодействия для Reference.xaml
    /// </summary>
    public partial class Reference : Window
    {
        FileStream f;
        public Reference()
        {
            try
            {
                InitializeComponent();
                f = new FileStream("ddd.rtf", FileMode.Open);
            }
            catch (Exception ex)
            {
                try
                {
                    f = new FileStream(@"GeneticAlgorithm/ddd.rtf", FileMode.Open);
                }
                catch (Exception ex1)
                {
                    MessageBox.Show("Error:" + ex1.Message);
                }
            }
        }
        public void ShowDialog()
        {
            try
            {

                rtbReference.Selection.Load(f, DataFormats.Rtf);
                this.Show();

            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Error:" + ex.Message);

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                f.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        
    }
}
