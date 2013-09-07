/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Linq;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Template_;
using Transformalize.Core.Transform_;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine.Core;
using Transformalize.Providers;

namespace Transformalize.Core.Process_ {

    public class Process {

        public string Name;
        public string View;
        public Encoding TemplateContentType = Encoding.Raw;
        public Dictionary<string, Map> MapEquals = new Dictionary<string, Map>();
        public Dictionary<string, Map> MapStartsWith = new Dictionary<string, Map>();
        public Dictionary<string, Map> MapEndsWith = new Dictionary<string, Map>();
        public Dictionary<string, Script> Scripts = new Dictionary<string, Script>();
        public Dictionary<string, Template> Templates = new Dictionary<string, Template>();
        public Dictionary<string, IConnection> Connections = new Dictionary<string, IConnection>();
        public bool OutputRecordsExist;
        public List<Entity> Entities = new List<Entity>();
        public IFields CalculatedFields = new Fields();
        public Entity MasterEntity;
        public Options Options = new Options();
        public List<Relationship> Relationships = new List<Relationship>();
        public IEnumerable<Field> RelatedKeys;
        
        public bool IsReady() {
            return Connections.Select(connection => connection.Value.IsReady()).All(b => b.Equals(true));
        }

        public Process() : this("TEST") {}

        public Process(string name)
        {
            Name = name;
            GlobalDiagnosticsContext.Set("process", name);
        }

        public IFields OutputFields()
        {
            var fields = new Fields();
            foreach (var entity in Entities)
            {
                fields.AddRange(new FieldSqlWriter(entity.All, entity.CalculatedFields, CalculatedFields).ExpandXml().Output().ToArray());
            }
            return fields;
        }

        public IParameters Parameters()
        {
            var parameters = new Parameters();

            foreach (var calculatedField in CalculatedFields)
            {
                if (calculatedField.Value.HasTransforms)
                {
                    foreach (AbstractTransform transform in calculatedField.Value.Transforms)
                    {
                        if (transform.HasParameters)
                        {
                            foreach (var parameter in transform.Parameters)
                            {
                                parameters[parameter.Key] = parameter.Value;
                            }
                        }
                    }
                }
            }
            return parameters;
        }
    }
}