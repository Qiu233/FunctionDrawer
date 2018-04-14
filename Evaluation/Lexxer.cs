using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Evaluation.Token;

namespace Evaluation
{
	public class Token
	{
		public enum TokenType
		{
			NONE = 1,
			TERM,//词语，这里只有函数名称和自变量x可以做词语
			NUMBER,//常数
			BRACKET_LEFT,//(
			BRACKET_RIGHT,//)
			COMMA,//,
			PLUS,//+
			MINUS,//-
			MULTI,//*
			DIVIDE,///
			POWER,//^
			END,
		}
		public TokenType Type;
		public string Text;
		public Token(TokenType type)
		{
			Type = type;
		}
	}
	public class LexException : Exception
	{
		public LexException(string str) : base(str)
		{

		}
	}
	public class Lexxer
	{
		private char[] Expr;
		private int index = 0;
		public Lexxer(string expr)
		{
			Expr = (expr + "#").ToCharArray();
		}

		public Token NextToken()
		{
			Token token = new Token(TokenType.NONE);
			StringBuilder sbuilder = null;
			while (true)
			{
				switch (token.Type)
				{
					case TokenType.NONE://初态
						{
							if (Char.IsLetter(Expr[index]))
							{
								token.Type = TokenType.TERM;
								sbuilder = new StringBuilder();
								sbuilder.Append(Expr[index]);
							}
							else if (Char.IsDigit(Expr[index]))
							{
								token.Type = TokenType.NUMBER;
								sbuilder = new StringBuilder();
								sbuilder.Append(Expr[index]);
							}
							else if (Expr[index] == '+')
								token.Type = TokenType.PLUS;
							else if (Expr[index] == '-')
								token.Type = TokenType.MINUS;
							else if (Expr[index] == '*')
								token.Type = TokenType.MULTI;
							else if (Expr[index] == '/')
								token.Type = TokenType.DIVIDE;
							else if (Expr[index] == '^')
								token.Type = TokenType.POWER;
							else if (Expr[index] == '(')
								token.Type = TokenType.BRACKET_LEFT;
							else if (Expr[index] == ')')
								token.Type = TokenType.BRACKET_RIGHT;
							else if (Expr[index] == ',')
								token.Type = TokenType.COMMA;
							else if (Expr[index] == '#')
							{
								token.Type = TokenType.END;
								return token;
							}
							else
								throw new LexException("Unexpected token");//无法鉴别
							index++;
						}
						break;
					case TokenType.TERM:
						if (Char.IsLetterOrDigit(Expr[index]))
						{
							sbuilder.Append(Expr[index]);
							index++;
						}
						else
						{
							token.Text = sbuilder.ToString();
							return token;
						}
						break;
					case TokenType.NUMBER:
						if (Char.IsDigit(Expr[index]) || Expr[index] == '.')
						{
							sbuilder.Append(Expr[index]);
							index++;
						}
						else
						{
							token.Text = sbuilder.ToString();
							if(token.Text.EndsWith("."))
							{
								token.Text += '0';
							}
							return token;
						}
						break;
					default://符号
						return token;
				}

			}
		}
	}
}
