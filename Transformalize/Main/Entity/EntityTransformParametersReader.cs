#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using Transformalize.Configuration;
using Transformalize.Logging;
using Transformalize.Main.Parameters;

namespace Transformalize.Main {
    public class EntityTransformParametersReader : ITransformParametersReader {
        private readonly Entity _entity;
        private readonly ILogger _logger;

        public EntityTransformParametersReader(Entity entity, ILogger logger)
        {
            _entity = entity;
            _logger = logger;
        }

        public IParameters Read(TflTransform transform) {
            var parameters = new Parameters.Parameters(new DefaultFactory(_logger));

            foreach (var p in transform.Parameters) {
                if (string.IsNullOrEmpty(p.Field) && (string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value))) {
                    throw new TransformalizeException(_logger, "The entity {0} has a {1} transform parameter without a field attribute, or name and value attributes.  Entity parameters require one or the other.", _entity.Alias, transform.Method);
                }

                var fields = new Fields(_entity.Fields, _entity.CalculatedFields);
                if (!string.IsNullOrEmpty(p.Field)) {
                    if (fields.FindByParamater(p).Any()) {
                        var field = fields.FindByParamater(p).Last();
                        var name = string.IsNullOrEmpty(p.Name) ? field.Alias : p.Name;
                        parameters.Add(field.Alias, name, null, field.Type);
                    } else {
                        if (!p.Field.StartsWith("Tfl")) {
                            _logger.EntityWarn(_entity.Name, "The entity {0} has a {1} transform parameter that references field {2}.  This field hasn't been defined yet in {0}.", _entity.Alias, transform.Method, p.Field);
                        }
                        var name = string.IsNullOrEmpty(p.Name) ? p.Field : p.Name;
                        parameters.Add(p.Field, name, p.HasValue() ? p.Value : null, "System.String");
                    }
                } else {
                    var parameter = new Parameter(p.Name, p.Value) {
                        SimpleType = Common.ToSimpleType(p.Type),
                        ValueReferencesField = p.HasValue() && fields.Find(p.Value).Any()
                    };
                    parameters.Add(p.Name, parameter);
                }
            }

            return parameters;
        }
    }
}