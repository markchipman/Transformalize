#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Cfg.Net;

namespace Pipeline.Configuration {
    public class TflType : CfgNode {

        public TflType() {
            Type = "string";
        }

        public TflType(string type) {
            Type = type;
        }

        [Cfg(required = true, unique = true, toLower = true, domain = Constants.TypeDomain)]
        public string Type { get; set; }
    }
}