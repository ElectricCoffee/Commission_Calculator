using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Commission_Calculator
{
    public partial class MainWindow : Form
    {

        public MainWindow()
        {
            InitializeComponent();
            RadioPrice.Checked = true;
            PercentSavedBox.ReadOnly = true;
        }

        private void RadioPercent_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioPrice.Checked)
            {
                PercentSavedBox.Text = "";
                PercentSavedBox.ReadOnly = true;
            }
            else
                PercentSavedBox.ReadOnly = false;

            if (RadioPercent.Checked)
            {
                ExtraPriceBox.Text = "";
                ExtraPriceBox.ReadOnly = true;
            }
            else
                ExtraPriceBox.ReadOnly = false;
        }

        private void ErrorMessage(string str)
        {
            MessageBox.Show(str, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PriceBox.Text))
                ErrorMessage("You forgot to fill in the base price!");
            else
            {
                var price = 0.0;

                if (!double.TryParse(PriceBox.Text, out price))
                    ErrorMessage("Price in numbers only!");

                else
                {
                    var modifier = PriceOrPercent();

                    Action<Func<double, double, double, double>> writeFileHelper = inputFunction =>
                    {
                        WriteFile(price, modifier.Item1, inputFunction, CurrencyBox.Text);
                    };

                    switch (modifier.Item2)
                    {
                        case "price": writeFileHelper((p, m, n) => p + m * n); break; // p is price, m is modifier, n is number of extra items

                        case "percent": writeFileHelper((p, m, n) => ((n + 1) * p) - ((n + 1) * p) * (m / 100)); break;

                        case "error": ErrorMessage("Input in numbers only!"); break;

                        default: ErrorMessage("Something went horribly wrong, contact me on slench102@gmail.com"); break;// unreachable
                    }
                }
            }
        }

        private Tuple<double, string> PriceOrPercent()
        {
            Func<TextBox, string, Tuple<double, string>> generalFunction = (textBox, type) =>
            {
                if (string.IsNullOrEmpty(textBox.Text))
                    return new Tuple<double, string> (double.Parse(PriceBox.Text), "price");

                double result;
                if (double.TryParse(textBox.Text, out result))
                    return new Tuple<double, string> (result, type.ToLower());

                // if neither of the above returns anything go here
                return new Tuple<double, string>(0xDEAD, "error");
            };

            if (ExtraPriceBox.ReadOnly == false) 
                return generalFunction(ExtraPriceBox, "Price");

            else 
                return generalFunction(PercentSavedBox, "Percent");

        }

        private void WriteFile(double price, double input, Func<double, double, double, double> inputFunction, string currency)
        {
            var dialog = new SaveFileDialog() { Filter = "Text files(*.txt)|*.txt" };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(dialog.FileName))
                {
                    sw.WriteLine("Pricing list :");
                    sw.WriteLine("01 Character : {0} {1:F2}", currency, price);
                    for(int i = 1; i < 10; i++)
                        sw.WriteLine("{2:D2} Characters: {0} {1:F2}", currency, inputFunction(price, input, i), i+1);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveButton_Click(sender, e);
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new HowTo().ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
