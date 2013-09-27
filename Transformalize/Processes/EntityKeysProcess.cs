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
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Processes {
    public class EntityKeysProcess : EtlProcess {
        private readonly Entity _entity;
        private readonly Process _process;

        public EntityKeysProcess(Process process, Entity entity) {
            GlobalDiagnosticsContext.Set("entity", Common.LogLength(entity.Alias, 20));
            _process = process;
            _entity = entity;
        }

        protected override void Initialize() {
            
            if (_process.IsFirstRun || !_entity.CanDetectChanges()) {
                Register(new EntityInputKeysExtractAll(_entity));
            } else {
                var operation = new EntityInputKeysExtractDelta(_process, _entity);
                if (operation.NeedsToRun())
                {
                    Register(operation);
                }
            }

            Register(new EntityInputKeysStore(_process, _entity));
        }

        protected override void PostProcessing() {
            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                Environment.Exit(1);
            }

            base.PostProcessing();
        }
    }
}