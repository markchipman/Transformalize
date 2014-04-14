﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Generator
{
    public class VBRazorCodeGenerator : RazorCodeGenerator
    {
        public VBRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
        {
        }

        internal override Func<CodeWriter> CodeWriterFactory
        {
            get { return () => new VBCodeWriter(); }
        }
    }
}