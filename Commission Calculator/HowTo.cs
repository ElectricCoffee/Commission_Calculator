using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commission_Calculator
{
    public partial class HowTo : Form
    {
        public HowTo()
        {
            InitializeComponent();
            richTextBox1.ReadOnly = true;
            richTextBox1.Text = 
                "How to use this software:\n\n"+
                "1) Type in a base price in the uppermost box\n"+
                "2) The Currency is completely optional\n"+
                "3) Then choose whether you want to assign a price for extra characters;"+
                " this will simply add the number to the base price ten times; or choose to calculate the saved percentage;"+
                " this will apply a saving to your base price 10 times\n\n"+
                "Example output:\n"+
                "Base Price: 20 USD\n"+
                "Discount: 10%\n"+
                "1 Character : USD 20\n"+
                "2 Characters: USD 36\n"+
                "3 Characters: USD 54\n"+
                "4 Characters: USD 72\n\n"+
                "Note that both the fields in the \"Additional Characters\" area are optional";

        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
