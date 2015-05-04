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

namespace Transformalize.Main {

    public class FieldParametersReader : ITransformParametersReader {
        private readonly DefaultFactory _defaultFactory;

        public FieldParametersReader(DefaultFactory defaultFactory) {
            _defaultFactory = defaultFactory;
        }

        public IParameters Read(TflTransform transform) {
            var parameters = new Parameters.Parameters(_defaultFactory);

            foreach (var p in transform.Parameters) {
                if (string.IsNullOrEmpty(p.Name)) {
                    return new Parameters.Parameters(_defaultFactory);
                }

                var value = p.HasValue() ? p.Value : null;
                var alias = p.HasValue() ? p.Name : p.Field;
                parameters.Add(alias, p.Name, value, p.Type);
            }

            return parameters;
        }
    }
}