using Evaluation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FunctionDrawer
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			TextBox tb = new TextBox()
			{
				Size = new Size(300, 350),
				Font = new Font("Arial", 15),
				Multiline = true
			};
			
			Button OK = new Button()
			{
				Text = "确认",
				Location = new Point(0, 350),
				Size = new Size(300, 50)
			};
			OK.Click += (s, ev) =>
			{
				string[] expr = tb.Text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				GLForm gf = new GLForm(10, 10, expr);
				gf.Show();
			};
			Controls.Add(tb);
			Controls.Add(OK);
		}


	}
}
