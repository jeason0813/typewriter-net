using System;
using System.Collections.Generic;
using System.Text;
using TinyJSON;
using MulticaretEditor;

public class CSTextNodeParser : TextNodeParser
{
	public CSTextNodeParser(string name) : base(name)
	{
	}
	
	public override Node Parse(LineArray lines)
	{
		LineIterator iterator = lines.GetLineRange(0, lines.LinesCount);
		List<Node> stack = new List<Node>();
		StringBuilder builder = new StringBuilder();
		int NORMAL = 0;
		int WAIT_OPEN = 1;
		int state = NORMAL;
		Node root = (Node)(new Dictionary<string, Node>());
		root["name"] = "FILE";
		root["line"] = -1;
		root["childs"] = new List<Node>();
		stack.Add(root);
		while (iterator.MoveNext())
		{
			string line = iterator.current.Text;
			for (int i = 0; i < line.Length; ++i)
			{
				if (state == NORMAL)
				{
					char c = line[i];
					if (c == 'c' && i + 6 < line.Length && line[i + 1] == 'l' && line[i + 2] == 'a' &&
						line[i + 3] == 's' && line[i + 4] == 's' && (i == 0 || char.IsWhiteSpace(line[i - 1])) &&
						(char.IsWhiteSpace(line[i + 5])))
					{
						i += 5;
						for (; i < line.Length && char.IsWhiteSpace(line[i]); ++i)
						{
						}
						if (i >= line.Length)
						{
							break;
						}
						builder.Length = 0;
						for (; i < line.Length; ++i)
						{
							c = line[i];
							if (char.IsLetterOrDigit(c) || c == '_')
							{
								builder.Append(c);
							}
						}
						string className = builder.ToString();
						builder.Length = 0;
						Node node = (Node)(new Dictionary<string, Node>());
						node["name"] = "class " + className;
						node["line"] = iterator.Index + 1;
						node["childs"] = new List<Node>();
						if (stack.Count > 0)
						{
							((List<Node>)stack[stack.Count - 1]["childs"]).Add(node);
						}
						stack.Add(node);
						state = WAIT_OPEN;
					}
					if (c == 'n' && i + 9 < line.Length && line[i + 1] == 'a' && line[i + 2] == 'm' &&
						line[i + 3] == 'e' && line[i + 4] == 's' && line[i + 5] == 'p' && line[i + 6] == 'a' &&
						line[i + 7] == 'c' && line[i + 8] == 'e' && (i == 0 || char.IsWhiteSpace(line[i - 1])) &&
						(char.IsWhiteSpace(line[i + 9])))
					{
						i += 5;
						for (; i < line.Length && char.IsWhiteSpace(line[i]); ++i)
						{
						}
						if (i >= line.Length)
						{
							break;
						}
						builder.Length = 0;
						for (; i < line.Length; ++i)
						{
							c = line[i];
							if (char.IsLetterOrDigit(c) || c == '_')
							{
								builder.Append(c);
							}
						}
						string className = builder.ToString();
						builder.Length = 0;
						Node node = (Node)(new Dictionary<string, Node>());
						node["name"] = "namespace " + className;
						node["line"] = iterator.Index + 1;
						node["childs"] = new List<Node>();
						if (stack.Count > 0)
						{
							((List<Node>)stack[stack.Count - 1]["childs"]).Add(node);
						}
						stack.Add(node);
						state = WAIT_OPEN;
					}
					else if (c == '}')
					{
						if (stack.Count > 0)
						{
							stack.RemoveAt(stack.Count - 1);
						}
					}
					else if (c == '{')
					{
						Node node = (Node)(new Dictionary<string, Node>());
						node["childs"] = new List<Node>();
						node["name"] = "";
						if (stack.Count > 0)
						{
							Node parent = stack[stack.Count - 1];
							if (((string)parent["name"]).StartsWith("class "))
							{
								bool empty = true;
								for (int j = 0; j < i; ++j)
								{
									if (!char.IsWhiteSpace(line[j]) && line[j] != '(' && line[j] != ')')
									{
										empty = false;
									}
								}
								string name;
								if (empty && iterator.Index > 0)
								{
									name = lines[iterator.Index - 1].Text;
									node["line"] = iterator.Index;
								}
								else
								{
									name = line;
									int index = name.IndexOf('{');
									name = name.Substring(0, index);
									node["line"] = iterator.Index + 1;
								}
								name = name.Replace("private ", "- ");
								name = name.Replace("protected ", "- ");
								name = name.Replace("public ", "+ ");
								name = name.Replace("internal ", "+ ");
								name = name.Replace("override ", "");
								name = name.Replace("virtual ", "");
								name = name.Replace("sealed ", "");
								name = name.Trim();
								node["name"] = name;
								((List<Node>)parent["childs"]).Add(node);
							}
						}
						stack.Add(node);
					}
				}
				else if (state == WAIT_OPEN)
				{
					for (; i < line.Length; ++i)
					{
						if (line[i] == '{')
						{
							state = NORMAL;
							break;
						}
					}
				}
			}
		}
		return ((List<Node>)root["childs"]).Count == 1 ? ((List<Node>)root["childs"])[0] : root;
	}
}