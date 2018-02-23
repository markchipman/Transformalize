#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public abstract class StringTransform : BaseTransform {

        protected readonly Func<IRow, IField, string> GetString;

        protected StringTransform(IContext context, string returns) : base(context, returns) {
            GetString = delegate (IRow row, IField field) {
                if (field.Type == "string") {
                    return (string)row[field];  // cast
                }
                return row[field].ToString();  // conversion, assumed to be more expensive
            };
        }
    }
}