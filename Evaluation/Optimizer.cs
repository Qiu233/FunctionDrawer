using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation
{
	public class Optimizer
	{
		private static string Calc(string optr, string vl, string vr)
		{
			switch (optr)
			{
				case "+":
					return (Double.Parse(vl) + Double.Parse(vr)).ToString();
				case "-":
					return (Double.Parse(vl) - Double.Parse(vr)).ToString();
				case "*":
					return (Double.Parse(vl) * Double.Parse(vr)).ToString();
				case "/":
					return (Double.Parse(vl) / Double.Parse(vr)).ToString();
				case "^":
					return (Math.Pow(Double.Parse(vl), Double.Parse(vr))).ToString();
			}
			return null;
		}
		

		public static Node Optimize(Node n)
		{
			if (n is BinaryNode bn)
			{
				bn.Left = Optimize(bn.Left);
				bn.Right = Optimize(bn.Right);
				if (bn.Left is ConstantNode cnl1 && bn.Right is ConstantNode cnr1)//常数合并
				{
					return new ConstantNode(Calc(bn.Optr, cnl1.Value, cnr1.Value));
				}
			}
			return n;
		}
	}
}
