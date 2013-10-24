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

        public enum TextBoxType { Error, Price, Percent }

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
                    var modifier = PriceOrPercent(); // a tuple containing the price per character or percent, and the type of textbox used or error

                    Action<Func<double, double, double, double>> writeFileHelper = inputFunction =>
                    {
                        WriteFile(price, modifier.Item1, inputFunction, CurrencyBox.Text);
                    };

                    switch (modifier.Item2)
                    {
                        case TextBoxType.Price: writeFileHelper((p, m, n) => p + m * n); break; // p is price, m is modifier, n is number of extra items

                        case TextBoxType.Percent: writeFileHelper((p, m, n) => ((n + 1) * p) - ((n + 1) * p) * (m / 100)); break;

                        case TextBoxType.Error: ErrorMessage("Input in numbers only!"); break;

                        default: ErrorMessage("Something went horribly wrong, contact me on slench102@gmail.com"); break;// unreachable
                    }
                }
            }
        }

        /// <summary>
        /// Generates a Tuple that contains a price value and a descriptive text
        /// </summary>
        /// <returns>A tuple that contains the type of operation as well as a value</returns>
        private Tuple<double, TextBoxType> PriceOrPercent()
        {
            Func<TextBox, TextBoxType, Tuple<double, TextBoxType>> generalFunction = (textBox, type) =>
            {
                if (string.IsNullOrEmpty(textBox.Text)) // if the given textbox is empty, assume to just use the price
                    return new Tuple<double, TextBoxType>(double.Parse(PriceBox.Text), TextBoxType.Price);

                double result; // allocate space for the 'out' parameter below
                if (double.TryParse(textBox.Text, out result)) // if the textbox does contain parsable text, return said text with the textbox's type
                    return new Tuple<double, TextBoxType>(result, type);

                // if neither of the above returns anything go here
                return new Tuple<double, TextBoxType>(0xDEAD, TextBoxType.Error);
            };

            if (ExtraPriceBox.ReadOnly == false) // if the ExtraPriceBox is editable, use it
                return generalFunction(ExtraPriceBox, TextBoxType.Price);

            else // if not assume it to be the PercentSavedBox instead.
                return generalFunction(PercentSavedBox, TextBoxType.Percent);

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
