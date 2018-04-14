using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation
{
	public class Node
	{

	}
	public class ConstantNode : Node
	{
		public string Value;
		public ConstantNode(string Constant)
		{
			this.Value = Constant;
		}
	}
	public class VariableNode : Node
	{
		public string Name;
		public VariableNode(string Name)
		{
			this.Name = Name;
		}
	}
	public class BinaryNode : Node
	{
		public Node Left, Right;
		public string Optr;
		public BinaryNode(string Optr)
		{
			this.Optr = Optr;
		}
	}
	public class FunctionNode : Node
	{
		public string FunctionName;
		public List<Node> Args;
	}
}
