﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourcepawnCondenser.SourcemodDefinition;
using SourcepawnCondenser.Tokenizer;

namespace SourcepawnCondenser
{
	public partial class Condenser
	{
		Token[] t = null;
		int position = 0;
		int length = 0;
		SMDefinition def = null;
		string source = string.Empty;


		string FileName = string.Empty;

		public Condenser(string sourceCode, string fileName)
		{
			t = Tokenizer.Tokenizer.TokenizeString(sourceCode, true).ToArray();
			position = 0;
			length = t.Length;
			def = new SMDefinition();
			source = sourceCode;
			FileName = fileName;
		}

		public SMDefinition Condense()
		{
			Token ct = null;
			while ((ct = t[position]).Kind != TokenKind.EOF)
			{
				if (ct.Kind == TokenKind.FunctionIndicator)
				{
					int newIndex = ConsumeSMFunction();
					if (newIndex != -1)
					{
						position = newIndex + 1;
						continue;
					}
				}
				if (ct.Kind == TokenKind.Enum)
				{
					int newIndex = ConsumeSMEnum();
					if (newIndex != -1)
					{
						position = newIndex + 1;
						continue;
					}
				}
				if (ct.Kind == TokenKind.Struct)
				{
					int newIndex = ConsumeSMStruct();
					if (newIndex != -1)
					{
						position = newIndex + 1;
						continue;
					}
				}
				if (ct.Kind == TokenKind.PrePocessorDirective)
				{
					int newIndex = ConsumeSMPPDirective();
					if (newIndex != -1)
					{
						position = newIndex + 1;
						continue;
					}
				}
				if (ct.Kind == TokenKind.Constant)
				{
					int newIndex = ConsumeSMConstant();
					if (newIndex != -1)
					{
						position = newIndex + 1;
						continue;
					}
				}
				if (ct.Kind == TokenKind.MethodMap)
				{
					int newIndex = ConsumeSMMethodmap();
					if (newIndex != -1)
					{
						position = newIndex + 1;
						continue;
					}
				}

				++position;
			}
			def.Sort();
			return def;
		}

		private int BacktraceTestForToken(int StartPosition, TokenKind TestKind, bool IgnoreEOL, bool IgnoreOtherTokens)
		{
			for (int i = StartPosition; i >= 0; --i)
			{
				if (t[i].Kind == TestKind)
				{
					return i;
				}
				else if (IgnoreOtherTokens)
				{
					continue;
				}
				else if (t[i].Kind == TokenKind.EOL && IgnoreEOL)
				{
					continue;
				}
				return -1;
			}
			return -1;
		}
		private int FortraceTestForToken(int StartPosition, TokenKind TestKind, bool IgnoreEOL, bool IgnoreOtherTokens)
		{
			for (int i = StartPosition; i < length; ++i)
			{
				if (t[i].Kind == TestKind)
				{
					return i;
				}
				else if (IgnoreOtherTokens)
				{
					continue;
				}
				else if (t[i].Kind == TokenKind.EOL && IgnoreEOL)
				{
					continue;
				}
				return -1;
			}
			return -1;
		}

        public static string TrimComments(string comment)
        {
            StringBuilder outString = new StringBuilder();
            string[] lines = comment.Split('\r', '\n');
            string line;
            for (int i = 0; i < lines.Length; ++i)
            {
                line = (lines[i].Trim()).TrimStart('/', '*', ' ', '\t');
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line.StartsWith("@"))
                    {
                        outString.Append("  ");
                    }
                    outString.AppendLine(line);
                }
            }
            return outString.ToString();
        }
	}
}
