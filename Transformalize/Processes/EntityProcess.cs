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

using System;
using System.Linq;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Operations;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Processes
{

    public class EntityProcess : EtlProcess
    {

        private Entity _entity;
        private readonly IFields _fieldsWithTransforms;

        public EntityProcess(Entity entity, IEntityBatch entityBatch = null)
        {
            _entity = entity;
            _entity.TflBatchId = (entityBatch ?? new SqlServerEntityBatch()).GetNext(_entity);
            _fieldsWithTransforms = new FieldSqlWriter(entity.All).ExpandXml().HasTransform().Context();
        }

        protected override void Initialize()
        {
            if (Process.Options.UseBeginVersion)
            {
                var keysExtract = new EntityInputKeysExtractDelta(_entity);
                if (!keysExtract.NeedsToRun()) return;
                Register(keysExtract);
            }
            else
            {
                Register(new EntityInputKeysExtractAll(_entity));
            }


            Register(new EntityInputKeysStore(_entity));
            Register(new EntityKeysToOperations(_entity));
            //Register(new ExtractDataDifferently(_entity));
            Register(new SerialUnionAllOperation());
            Register(new ApplyDefaults(_entity.All, _entity.CalculatedFields));
            Register(new TransformFields(_fieldsWithTransforms));
            Register(new TransformFields(_entity.CalculatedFields));

            if (_entity.Group)
                Register(new EntityAggregation(_entity));

            if (Process.OutputRecordsExist)
            {
                Register(new EntityJoinAction(_entity).Right(new EntityOutputKeysExtract(_entity)));
                var branch = new BranchingOperation()
                    .Add(new PartialProcessOperation()
                        .Register(new EntityActionFilter(ref _entity, EntityAction.Insert))
                        .RegisterLast(new EntityBulkInsert(_entity)))
                    .Add(new PartialProcessOperation()
                        .Register(new EntityActionFilter(ref _entity, EntityAction.Update))
                        .RegisterLast(new EntityBatchUpdate(_entity)));
                RegisterLast(branch);
            }
            else
            {
                Register(new EntityAddTflFields(_entity));
                RegisterLast(new EntityBulkInsert(_entity));
            }

        }

        protected override void PostProcessing()
        {

            var errors = GetAllErrors().ToArray();
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                throw new InvalidOperationException("Houstan.  We have a problem in the threads!");
            }

            if (Process.Options.WriteEndVersion)
            {
                new SqlServerEntityVersionWriter(_entity).WriteEndVersion(_entity.End, _entity.RecordsAffected);
            }

            base.PostProcessing();
        }

    }
}
