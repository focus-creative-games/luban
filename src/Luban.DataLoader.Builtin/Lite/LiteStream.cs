using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Lite;

public class LiteStream
{
    private readonly List<string> _tokens = new List<string>();
    private int _currentIndex = 0;

    public LiteStream(string dataStr)
    {
        ParseTokens(dataStr);
    }

    private void ParseTokens(string dataStr)
    {
        var token = new StringBuilder();
        int nestDepth = 0;
        for (int i = 0; i < dataStr.Length; i++)
        {
            char c = dataStr[i];
            // handle escape characters
            if (c == '\\')
            {
                i++;
                if (i >= dataStr.Length)
                {
                    throw new Exception($"Invalid escape sequence: `{dataStr}`");
                }
                c = dataStr[i];
                if (c == 'n')
                {
                    c = '\n';
                }
                else if (c == 't')
                {
                    c = '\t';
                }
                else if (c == 'r')
                {
                    c = '\r';
                }
                token.Append(c);
            }
            else if (c == ',' || c == '{' || c == '}')
            {
                if (token.Length > 0)
                {
                    _tokens.Add(token.ToString());
                    token.Clear();
                }
                if (c == '{')
                {
                    nestDepth++;
                    _tokens.Add("{");
                }
                else if (c == '}')
                {
                    nestDepth--;
                    if (nestDepth < 0)
                    {
                        throw new Exception($"Unmatched closing brace in: `{dataStr}`");
                    }
                    _tokens.Add("}");
                }
            }
            else
            {
                token.Append(c);
            }
        }
        if (token.Length > 0)
        {
            _tokens.Add(token.ToString());
        }
        if (nestDepth != 0)
        {
            throw new Exception($"Unmatched opening brace in: `{dataStr}`");
        }
    }

    public void ReadStructOrCollectionBegin()
    {
        if (_currentIndex >= _tokens.Count)
        {
            throw new Exception("No more tokens to read.");
        }
        if (_tokens[_currentIndex] != "{")
        {
            throw new Exception($"Expected '{{' but found '{_tokens[_currentIndex]}'");
        }
        ++_currentIndex;
    }

    public void ReadStructOrCollectionEnd()
    {
        if (_currentIndex >= _tokens.Count)
        {
            throw new Exception("No more tokens to read.");
        }
        if (_tokens[_currentIndex] != "}")
        {
            throw new Exception($"Expected '}}' but found '{_tokens[_currentIndex]}'");
        }
        _currentIndex++;
    }

    public bool IsBeginOfStructOrCollection()
    {
        if (_currentIndex >= _tokens.Count)
        {
            return false;
        }
        return _tokens[_currentIndex] == "{";
    }

    public bool IsEndOfStructOrCollection()
    {
        if (_currentIndex >= _tokens.Count)
        {
            return false;
        }
        return _tokens[_currentIndex] == "}";
    }

    public string ReadData()
    {
        if (_currentIndex >= _tokens.Count)
        {
            throw new Exception("No more tokens to read.");
        }
        string token = _tokens[_currentIndex];
        if (token == "{" || token == "}")
        {
            throw new Exception($"Expected data but found '{token}'");
        }
        _currentIndex++;
        return token;
    }
}
