using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Evaluation.Token;

namespace Evaluation
{
	public class ParseException : Exception
	{
		public ParseException(string str) : base(str)
		{

		}
	}
	public class Parser
	{
		private Lexxer Lexxer = null;
		private Token CurrentToken = null;
		public Parser(Lexxer lexxer)
		{
			Lexxer = lexxer;
			Accept();
		}
		private bool Match(TokenType type)
		{
			return CurrentToken.Type == type;
		}
		private void Accept(TokenType type)
		{
			if (CurrentToken.Type != type)
			{
				throw new ParseException("Unexpected token");
			}
			Accept();
		}
		private void Accept()
		{
			CurrentToken = Lexxer.NextToken();
		}
		public Node Parse()
		{
			Node e = E();
			if (CurrentToken.Type != TokenType.END)
				throw new ParseException("Unexpected token");
			return e;
		}
		private Node E()
		{
			Node left = D();
			BinaryNode bn = E1(left);
			return bn ?? left;
		}
		private BinaryNode E1(Node left)
		{
			if (Match(TokenType.PLUS))
			{
				Accept();
				BinaryNode bn = new BinaryNode("+");
				Node right = D();
				bn.Left = left;
				bn.Right = right;
				BinaryNode bn1 = E1(bn);
				return bn1 ?? bn;
			}
			if (Match(TokenType.MINUS))
			{
				Accept();
				BinaryNode bn = new BinaryNode("-");
				Node right = D();
				bn.Left = left;
				bn.Right = right;
				BinaryNode bn1 = E1(bn);
				return bn1 ?? bn;
			}
			return null;
		}
		private Node D()
		{
			Node left = R();
			BinaryNode bn = D1(left);
			return bn ?? left;
		}
		private BinaryNode D1(Node left)
		{
			if (Match(TokenType.MULTI))
			{
				Accept();
				BinaryNode bn = new BinaryNode("*");
				Node right = R();
				bn.Left = left;
				bn.Right = right;
				BinaryNode bn1 = D1(bn);
				return bn1 ?? bn;
			}
			if (Match(TokenType.DIVIDE))
			{
				Accept();
				BinaryNode bn = new BinaryNode("/");
				Node right = R();
				bn.Left = left;
				bn.Right = right;
				BinaryNode bn1 = D1(bn);
				return bn1 ?? bn;
			}
			return null;
		}
		private Node R()
		{
			Node left = V();
			BinaryNode bn = R1(left);
			return bn ?? left;
		}
		private BinaryNode R1(Node left)
		{
			if (Match(TokenType.POWER))
			{
				Accept();
				BinaryNode bn = new BinaryNode("^");
				Node right = V();
				bn.Left = left;
				bn.Right = right;
				BinaryNode bn1 = R1(bn);
				return bn1 ?? bn;
			}
			return null;
		}
		private Node V()
		{
			if (Match(TokenType.MINUS))
			{
				Accept();//-
				BinaryNode bn = new BinaryNode("-")
				{
					Left = new ConstantNode("0"),
					Right = V1()
				};
				return bn;
			}
			else
				return V1();
		}
		private Node V1()
		{
			if (Match(TokenType.NUMBER))
			{
				string text = CurrentToken.Text;
				var n = new ConstantNode(CurrentToken.Text);
				Accept();
				if (Match(TokenType.TERM) || Match(TokenType.BRACKET_LEFT))
				{
					BinaryNode bn = new BinaryNode("*")
					{
						Left = n,
						Right = F()
					};
					return bn;
				}
				else
					return n;
			}
			else if (Match(TokenType.BRACKET_LEFT))
			{
				Accept();//(
				Node n = E();
				Accept(TokenType.BRACKET_RIGHT);
				return n;
			}
			else if (Match(TokenType.TERM))
			{
				string text = CurrentToken.Text;
				Accept();
				if (Match(TokenType.BRACKET_LEFT))
				{
					FunctionNode n = new FunctionNode()
					{
						FunctionName = text,
						Args = new List<Node>()
					};
					Accept(TokenType.BRACKET_LEFT);
					A(n.Args);
					Accept(TokenType.BRACKET_RIGHT);
					return n;
				}
				else
				{
					return new VariableNode(text);
				}
			}
			else
			{
				throw new ParseException("Unexpected Token");
			}
		}
		private Node F()
		{
			if (Match(TokenType.BRACKET_LEFT))
			{
				Accept();
				Node n = E();
				Accept(TokenType.BRACKET_RIGHT);
				return n;
			}
			else
			{
				string text = CurrentToken.Text;
				Accept(TokenType.TERM);
				if (Match(TokenType.BRACKET_LEFT))
				{
					FunctionNode n = new FunctionNode()
					{
						FunctionName = text,
						Args = new List<Node>()
					};
					Accept(TokenType.BRACKET_LEFT);
					A(n.Args);
					Accept(TokenType.BRACKET_RIGHT);
					return n;
				}

				else
				{
					return new VariableNode(text);
				}
			}
		}
		private void A(List<Node> args)
		{
			args.Add(E());
			A1(args);
		}
		private void A1(List<Node> args)
		{
			if (Match(TokenType.COMMA))
			{
				Accept();
				args.Add(E());
				A1(args);
			}
		}
	}
}
/*
E  ->  DE'
E' ->  +DE'|-DE'|e

D  ->  RD'
D' ->  *RD'|/RD'|e

R  ->  VR'
R' ->  ^VR'|e

V  ->  -V'|V'
V' ->  number|numberF|var|(E)|term(A)

F  ->  var|term(A)|(E)

A  ->  EA'
A' ->  ,EA'|e

*/
