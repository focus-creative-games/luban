// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

﻿using System;
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
        bool beginData = false;
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
                string tokenStr = token.ToString().Trim();
                token.Clear();
                if (c == '{')
                {
                    if (tokenStr.Length > 0 && !string.IsNullOrWhiteSpace(tokenStr))
                    {
                        throw new Exception($"Invalid token before '{{': `{dataStr}`");
                    }
                    nestDepth++;
                    beginData = true;
                    _tokens.Add("{");
                }
                else if (c == '}')
                {
                    if (tokenStr.Length > 0)
                    {
                        if (!beginData)
                        {
                            throw new Exception($"Invalid token before '}}': `{dataStr}`");
                        }
                        _tokens.Add(tokenStr);
                    }
                    beginData = false;
                    nestDepth--;
                    if (nestDepth < 0)
                    {
                        throw new Exception($"Unmatched closing brace in: `{dataStr}`");
                    }
                    _tokens.Add("}");
                }
                else
                {
                    if (nestDepth == 0)
                    {
                        throw new Exception($"Invalid token before ',': `{dataStr}`");
                    }
                    if (tokenStr.Length > 0)
                    {
                        if (!beginData)
                        {
                            throw new Exception($"Invalid token before ',': `{dataStr}`");
                        }
                        _tokens.Add(tokenStr);
                    }
                    beginData = true;
                }
            }
            else if (c == '\n' || c == '\r')
            {
                // skip newlines
            }
            else
            {
                token.Append(c);
            }
        }
        if (nestDepth != 0)
        {
            throw new Exception($"Unmatched opening brace in: `{dataStr}`");
        }
        if (token.Length > 0 && !string.IsNullOrEmpty(token.ToString()))
        {
            throw new Exception($"Invalid token at end: `{dataStr}`");
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
