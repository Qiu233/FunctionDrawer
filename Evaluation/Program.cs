using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation
{
	class Program
	{
		static void Main(string[] args)
		{
			string Expr = "3*x^2+2x+5+abs(x)+5(5+6)+8/4";
			Console.WriteLine(Expr + "\n");
			Executer e = Executer.Create(Expr);
			e.PrintTree();
			Console.WriteLine("\nOpcode:");
			e.PrintCode();
			Console.ReadKey();
		}
	}
}
//3x*4
//4x-3x*5