using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation
{
	public enum OpCodes
	{
		NONE = 1,
		HLT,

		LDCONST,
		LDVAR,

		PLUS,
		MINUS,
		MULTI,
		DIVIDE,
		POWER,

		CALL
	}
	public class Instruction
	{
		public OpCodes OpCode;
		public object Param = null;
		public object Extra = null;
		public Instruction(OpCodes op)
		{
			OpCode = op;
		}
		public Instruction(OpCodes op, object param) : this(op)
		{
			Param = param;
		}
	}

	public class VM
	{

		private Dictionary<string, double> Variables = new Dictionary<string, double>();
		private Dictionary<string, MethodInfo> Functions = new Dictionary<string, MethodInfo>();
		private List<Instruction> Instructions = new List<Instruction>();
		private Stack<double> stack = new Stack<double>();
		private VM() { }
		public static VM Create()
		{
			VM vm = new VM();
			return vm;
		}
		public void SetVariable(string name, double value)
		{
			Variables[name] = value;
		}
		public void RegisterFunction(string name, MethodInfo mi)
		{
			if (!mi.IsStatic)
				throw new Exception("Only can static method be registered");
			Functions[name] = mi;
		}
		public void Write(Instruction ins)
		{
			Instructions.Add(ins);
		}
		public double Peek()
		{
			return stack.Peek();
		}
		public void PrintCode()
		{
			for (int i = 0; i < Instructions.Count; i++)
			{
				Instruction ins = Instructions[i];
				object param = ins.Param;
				Console.Write(ins.OpCode + "\t");
				switch (ins.OpCode)
				{
					case OpCodes.LDCONST:
						Console.WriteLine((double)param);
						break;
					case OpCodes.LDVAR:
						Console.WriteLine((string)param);
						break;
					case OpCodes.PLUS:
					case OpCodes.MINUS:
					case OpCodes.MULTI:
					case OpCodes.DIVIDE:
					case OpCodes.POWER:
						Console.WriteLine();
						break;
					case OpCodes.CALL:
						{
							string name = (string)param;
							int argcount = (int)ins.Extra;
							Console.WriteLine(name + "\t\t" + argcount);
						}
						break;
				}
			}
		}
		public void Run()
		{
			for (int i = 0; i < Instructions.Count; i++)
			{
				Instruction ins = Instructions[i];
				object param = ins.Param;
				switch (ins.OpCode)
				{
					case OpCodes.LDCONST:
						stack.Push((double)param);
						break;
					case OpCodes.LDVAR:
						if (!Variables.ContainsKey((string)param))
							throw new Exception("No such variable has been assigned: " + (string)param);
						stack.Push(Variables[(string)param]);
						break;
					case OpCodes.PLUS:
						{
							double a = stack.Pop();
							double b = stack.Pop();
							stack.Push(b + a);
						}
						break;
					case OpCodes.MINUS:
						{
							double a = stack.Pop();
							double b = stack.Pop();
							stack.Push(b - a);
						}
						break;
					case OpCodes.MULTI:
						{
							double a = stack.Pop();
							double b = stack.Pop();
							stack.Push(b * a);
						}
						break;
					case OpCodes.DIVIDE:
						{
							double a = stack.Pop();
							double b = stack.Pop();
							if (a == 0) throw new ParseException("除数为0");
							stack.Push(b / a);
						}
						break;
					case OpCodes.POWER:
						{
							double a = stack.Pop();
							double b = stack.Pop();
							stack.Push(Math.Pow(b, a));
						}
						break;
					case OpCodes.CALL:
						{
							string name = (string)param;
							int argcount = (int)ins.Extra;
							if (!Functions.ContainsKey(name))
								throw new Exception("No such functoins can be called: " + name);
							MethodInfo mi = Functions[name];
							List<object> args = new List<object>();
							for (int j = 0; j < argcount; j++)
							{
								double v = stack.Pop();
								args.Insert(0, v);
							}
							double r = (double)mi.Invoke(null, args.ToArray());
							stack.Push(r);
						}
						break;
				}
			}
		}
	}
}
