using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation
{
	public class Executer
	{
		private Node node;
		public string Expression
		{
			get;
			private set;
		}
		private Executer() { }
		private VM vm;
		public static Executer Create()
		{
			return new Executer();
		}
		public static Executer Create(string Expr)
		{
			Executer e = new Executer();
			e.Aanalysis(Expr);
			return e;
		}

		private OpCodes OpCodeFromOptr(string optr)
		{
			switch (optr)
			{
				case "+": return OpCodes.PLUS;
				case "-": return OpCodes.MINUS;
				case "*": return OpCodes.MULTI;
				case "/": return OpCodes.DIVIDE;
				case "^": return OpCodes.POWER;
			}
			throw new Exception("No such operator");
		}

		private void Build(Node n)
		{
			if (n is BinaryNode bn)
			{
				Build(bn.Left);
				Build(bn.Right);
				vm.Write(new Instruction(OpCodeFromOptr(bn.Optr)));
			}
			else if (n is ConstantNode cn)
			{
				vm.Write(new Instruction(OpCodes.LDCONST, Double.Parse(cn.Value)));
			}
			else if (n is VariableNode vn)
			{
				vm.Write(new Instruction(OpCodes.LDVAR, vn.Name));
			}
			else if (n is FunctionNode fn)
			{
				foreach (var arg in fn.Args)
					Build(arg);
				Instruction ins = new Instruction(OpCodes.CALL, fn.FunctionName)
				{
					Extra = fn.Args.Count
				};
				vm.Write(ins);
			}
		}

		public void Aanalysis(string Expr)
		{
			Parser p = new Parser(new Lexxer(Expr));
			node = p.Parse();
			node = Optimizer.Optimize(node);
			vm = VM.Create();
			Build(node);
			Expression = Expr;
		}
		public void SetVariable(string name, double value)
		{
			vm.SetVariable(name, value);
		}
		public void RegisterFunction(string name, MethodInfo mi)
		{
			vm.RegisterFunction(name, mi);
		}
		public double Calculate()
		{
			vm.Run();
			return vm.Peek();
		}
		public void PrintTree()
		{
			PrintTree(node, 0);
		}
		private static void PrintTree(Node n, int d)
		{
			if (n is ConstantNode cn)
			{
				Output("Constant: " + cn.Value, d);
			}
			else if (n is VariableNode vn)
			{
				Output("Variable: " + vn.Name, d);
			}
			else if (n is BinaryNode bn)
			{
				Output("OPTR: " + bn.Optr, d);
				Output("Left: ", d + 1);
				PrintTree(bn.Left, d + 2);
				Output("Right: ", d + 1);
				PrintTree(bn.Right, d + 2);
			}
			else if (n is FunctionNode fn)
			{
				Output("Function: " + fn.FunctionName, d);
				Output("Args:", d + 1);
				foreach (var arg in fn.Args)
				{
					PrintTree(arg, d + 2);
				}
			}
		}
		public void PrintCode()
		{
			vm.PrintCode();
		}
		private static void Output(string str, int d)
		{
			for (int i = 0; i < d; i++)
				Console.Write('-');
			Console.WriteLine(str);
		}
	}
}
