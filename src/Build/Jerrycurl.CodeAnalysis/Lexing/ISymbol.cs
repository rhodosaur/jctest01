﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jerrycurl.CodeAnalysis.Lexing
{
    public interface ISymbol
    {
        bool Parse(Tokenizer tokenizer);
    }
}