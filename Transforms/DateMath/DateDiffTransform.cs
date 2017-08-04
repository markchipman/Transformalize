﻿#region license
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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Transforms;

namespace Transformalize.Transform.DateMath {

    public class DateDiffTransform : BaseTransform {

        public static readonly Dictionary<string, Func<DateTime, DateTime, object>> Parts = new Dictionary<string, Func<DateTime, DateTime, object>>() {
            {"d", (x,y) => (y-x).TotalDays},
            {"day", (x,y) => (y-x).TotalDays},
            {"date", (x, y) => new DateTime((y-x).Ticks)},
            {"h", (x,y)=>(y-x).TotalHours},
            {"hour", (x,y)=>(y-x).TotalHours},
            {"ms", (x,y)=>(y-x).TotalMilliseconds},
            {"millisecond", (x,y)=>(y-x).TotalMilliseconds},
            {"m",(x,y)=>(y-x).TotalMinutes},
            {"minute",(x,y)=>(y-x).TotalMinutes},
            {"s",(x,y)=>(y-x).TotalSeconds},
            {"second",(x,y)=>(y-x).TotalSeconds},
            {"tick",(x,y)=>(y-x).Ticks},
            {"M",(x,y)=>(y-x).TotalDays / (365/12.0) },
            {"month",(x,y)=>(y-x).TotalDays / (365/12.0) },
            { "y",(x,y)=>(y-x).TotalDays / 365 },
            { "year",(x,y)=>(y-x).TotalDays / 365 }
        };

        public static readonly Dictionary<string, string> PartReturns = new Dictionary<string, string> {
            {"d", "double"},
            {"day", "double"},
            {"date", "date"},
            {"h", "double"},
            {"hour", "double"},
            {"ms", "double"},
            {"millisecond", "double"},
            {"m","double"},
            {"minute","double"},
            {"s","double"},
            {"second","double"},
            {"tick","long"},
            {"y","double" },
            {"year","double" },
            {"M","double" },
            {"month","double" }
        };

        private readonly Field _start;
        private readonly Field _end;
        private readonly Action<IRow> _transform;

        public DateDiffTransform(IContext context) : base(context, PartReturns[context.Transform.TimeComponent]) {
            var input = MultipleInput().TakeWhile(f => f.Type.StartsWith("date")).ToArray();

            _start = input[0];

            if (Context.Transform.TimeComponent.In("year", "month")) {
                Context.Warn("datediff can not determine exact years or months.  For months, it returns (days / (365/12.0)).  For years, it returns (days / 365).");
            }

            if (PartReturns.ContainsKey(context.Transform.TimeComponent)) {
                if (input.Count() > 1) {
                    // comparing between two dates in pipeline
                    _end = input[1];
                    _transform = row => row[context.Field] = Parts[context.Transform.TimeComponent]((DateTime)row[_start], (DateTime)row[_end]);
                } else {
                    // comparing between one date in pipeline and now (depending on time zone)
                    var fromTimeZone = Context.Transform.FromTimeZone == Constants.DefaultSetting ? "UTC" : Context.Transform.FromTimeZone;
                    var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, fromTimeZone);
                    _transform = row => row[context.Field] = Parts[context.Transform.TimeComponent](now, (DateTime)row[_start]);
                }

            } else {
                Context.Warn($"datediff does not support time component {Context.Transform.TimeComponent}.");
                var defaults = Constants.TypeDefaults();
                _transform = row => row[Context.Field] = Context.Field.Default != Constants.DefaultSetting ? Context.Field.Convert(Context.Field.Default) : defaults[Context.Field.Type];
            }

        }

        public override IRow Transform(IRow row) {
            _transform(row);
            Increment();
            return row;
        }
    }
}