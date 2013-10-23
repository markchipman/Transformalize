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

using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {

    public class EntityConfigurationLoader {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;

        public EntityConfigurationLoader(Process process) {
            _process = process;
        }

        public Entity Read(int batchId, EntityConfigurationElement element, bool isMaster) {

            GlobalDiagnosticsContext.Set("entity", Common.LogLength(element.Alias, 20));

            var entity = new Entity(batchId) {
                ProcessName = _process.Name,
                Schema = element.Schema,
                Name = element.Name,
                InputConnection = _process.Connections[element.Connection],
                Prefix = element.Prefix,
                Group = element.Group,
                IndexOptimizations = element.IndexOptimizations,
                Alias = string.IsNullOrEmpty(element.Alias) ? element.Name : element.Alias,
                UseBcp = element.UseBcp
            };

            GuardAgainstInvalidGrouping(element, entity);
            GuardAgainstMissingPrimaryKey(element);

            var fieldIndex = 0;
            foreach (FieldConfigurationElement f in element.Fields) {
                var fieldType = GetFieldType(f, isMaster);

                var field = new FieldReader(_process, entity).Read(f, fieldType);

                if (field.Index == 0) {
                    field.Index = fieldIndex;
                }

                entity.Fields[field.Alias] = field;

                if (f.PrimaryKey) {
                    entity.PrimaryKey[field.Alias] = field;
                }

                fieldIndex++;
            }

            foreach (FieldConfigurationElement cf in element.CalculatedFields) {
                var fieldReader = new FieldReader(_process, entity, usePrefix: false);
                var fieldType = GetFieldType(cf, isMaster);
                var field = fieldReader.Read(cf, fieldType);

                if (field.Index == 0) {
                    field.Index = fieldIndex;
                }

                field.Input = false;
                entity.CalculatedFields.Add(cf.Alias, field);
                if (cf.PrimaryKey) {
                    entity.PrimaryKey[cf.Alias] = field;
                }
                fieldIndex++;
            }

            LoadVersion(element, entity);

            return entity;
        }

        private void GuardAgainstMissingPrimaryKey(EntityConfigurationElement element) {
            if (element.Fields.Cast<FieldConfigurationElement>().Any(f => f.PrimaryKey))
                return;

            if (element.CalculatedFields.Cast<FieldConfigurationElement>().Any(cf => cf.PrimaryKey))
                return;

            _log.Info("Adding TflHashCode primary key for {0}.", element.Name);
            var pk = new FieldConfigurationElement {
                Name = "TflHashCode",
                Type = "System.Int32",
                PrimaryKey = true,
                Transforms = new TransformElementCollection {
                    new TransformConfigurationElement {Method = "concat", Parameter = "*"},
                    new TransformConfigurationElement {Method = "gethashcode"}
                }
            };
            element.CalculatedFields.Insert(pk);
        }

        private void LoadVersion(EntityConfigurationElement element, Entity entity) {
            if (String.IsNullOrEmpty(element.Version))
                return;

            if (entity.Fields.ContainsKey(element.Version)) {
                entity.Version = entity.Fields[element.Version];
            } else {
                if (entity.Fields.Any(kv => kv.Value.Name.Equals(element.Version, IC))) {
                    entity.Version = entity.Fields.ToEnumerable().First(v => v.Name.Equals(element.Version, IC));
                } else {
                    if (entity.CalculatedFields.ContainsKey(element.Version)) {
                        entity.Version = entity.CalculatedFields[element.Version];
                    } else {
                        _log.Error("version field reference '{0}' is undefined in {1}.", element.Version, element.Name);
                        Environment.Exit(0);
                    }
                }
            }
            entity.Version.Output = true;
        }

        private void GuardAgainstInvalidGrouping(EntityConfigurationElement element, Entity entity) {
            if (!entity.Group)
                return;

            if (!element.Fields.Cast<FieldConfigurationElement>().Any(f => string.IsNullOrEmpty(f.Aggregate)))
                return;

            _log.Error("Entity {0} is set to group, but not all your fields have aggregate defined.", entity.Alias);
            Environment.Exit(1);
        }

        private static FieldType GetFieldType(FieldConfigurationElement element, bool isMaster) {
            FieldType fieldType;
            if (element.PrimaryKey) {
                fieldType = isMaster ? FieldType.MasterKey : FieldType.PrimaryKey;
            } else {
                fieldType = FieldType.Field;
            }
            return fieldType;
        }
    }
}