﻿using System;
using Transformalize.Configuration;

namespace Transformalize.Main {

    public class ProcessOperationsLoader {

        private readonly Process _process;
        private readonly FieldElementCollection _elements;

        public ProcessOperationsLoader(ref Process process, FieldElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public void Load() {

            var autoIndex = Convert.ToInt16(_process.MasterEntity == null ? 0 : new Fields(_process.MasterEntity.Fields, _process.MasterEntity.CalculatedFields).Count + 1);

            foreach (FieldConfigurationElement f in _elements) {
                var field = new FieldReader(_process, _process.MasterEntity, false).Read(f);
                
                if (field.Index.Equals(short.MaxValue)) {
                    field.Index = autoIndex;
                }

                field.Input = false;
                field.IsCalculated = true;
                field.Index = field.Index == 0 ? autoIndex : field.Index;
                _process.CalculatedFields.Add(field);

                foreach (TransformConfigurationElement t in f.Transforms) {

                    var factory = new TransformOperationFactory(_process);
                    var parameters = t.Parameter == "*" ?
                        new ProcessParametersReader(_process).Read() :
                        new ProcessTransformParametersReader(_process).Read(t);
                    var operation = factory.Create(field, t, parameters);

                    _process.TransformOperations.Add(operation);
                    foreach (var parameter in parameters) {
                        _process.Parameters[parameter.Key] = parameter.Value;
                    }
                }

                autoIndex++;
            }
        }

    }
}