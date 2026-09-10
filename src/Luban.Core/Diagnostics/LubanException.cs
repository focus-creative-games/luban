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

using Luban.Schema;

namespace Luban.Diagnostics;

public class LubanException : Exception
{
    public string MessageKey { get; }

    public object[] Args { get; }

    /// <summary>Optional schema definition location for structured diagnostics.</summary>
    public SchemaSource SchemaOrigin { get; }

    public LubanException(string messageKey, params object[] args)
        : base(MessageCatalog.Format(messageKey, args))
    {
        MessageKey = messageKey;
        Args = args ?? Array.Empty<object>();
    }

    public LubanException(SchemaSource schemaOrigin, string messageKey, params object[] args)
        : base(MessageCatalog.Format(messageKey, args))
    {
        MessageKey = messageKey;
        Args = args ?? Array.Empty<object>();
        SchemaOrigin = schemaOrigin;
    }

    public LubanException(Exception innerException, string messageKey, params object[] args)
        : base(MessageCatalog.Format(messageKey, args), innerException)
    {
        MessageKey = messageKey;
        Args = args ?? Array.Empty<object>();
    }

    public LubanException(Exception innerException, SchemaSource schemaOrigin, string messageKey, params object[] args)
        : base(MessageCatalog.Format(messageKey, args), innerException)
    {
        MessageKey = messageKey;
        Args = args ?? Array.Empty<object>();
        SchemaOrigin = schemaOrigin;
    }
}
