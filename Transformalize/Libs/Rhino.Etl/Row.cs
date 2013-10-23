#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Transformalize.Libs.Rhino.Etl
{
    /// <summary>
    ///     Represent a virtual row
    /// </summary>
    [DebuggerDisplay("Count = {items.Count}")]
    [Serializable]
    public class Row : IEnumerable
    {
        private static readonly Dictionary<Type, List<PropertyInfo>> PropertiesCache = new Dictionary<Type, List<PropertyInfo>>();
        private static readonly Dictionary<Type, List<FieldInfo>> FieldsCache = new Dictionary<Type, List<FieldInfo>>();

        private static readonly StringComparer Ic = StringComparer.InvariantCultureIgnoreCase;
        private IDictionary<string, object> _storage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Row" /> class.
        /// </summary>
        public Row()
        {
            _storage = new Dictionary<string, object>(Ic);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Row" /> class.
        /// </summary>
        /// <param name="itemsToClone">The items to clone.</param>
        protected Row(IDictionary<string, object> itemsToClone)
        {
            _storage = new Dictionary<string, object>(itemsToClone, Ic);
        }


        public IEnumerable<string> Columns
        {
            get { return new List<string>(_storage.Keys); }
        }

        public object this[string key]
        {
            get { return _storage.ContainsKey(key) ? _storage[key] : null; }
            set
            {
                if (value == DBNull.Value)
                    _storage[key] = null;
                else
                    _storage[key] = value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        /// <summary>
        ///     Creates a copy of the given source, erasing whatever is in the row currently.
        /// </summary>
        /// <param name="source">The source row.</param>
        public void Copy(IDictionary<string, object> source)
        {
            _storage = new Dictionary<string, object>(source, Ic);
        }

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Row Clone()
        {
            return new Row(_storage);
        }

        public bool ContainsKey(string key)
        {
            return _storage.ContainsKey(key);
        }

        public bool Contains(string key)
        {
            return _storage.ContainsKey(key);
        }

        /// <summary>
        ///     Creates a key that allow to do full or partial indexing on a row
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public ObjectArrayKeys CreateKey(string[] columns)
        {
            var array = new object[columns.Length];
            for (var i = 0; i < columns.Length; i++)
            {
                array[i] = _storage[columns[i]];
            }
            return new ObjectArrayKeys(array);
        }

        /// <summary>
        ///     Generate a row from the reader
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static Row FromReader(IDataReader reader)
        {
            var row = new Row();
            var count = reader.FieldCount;
            for (var i = 0; i < count; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            return row;
        }

        public void Add(string key, object value)
        {
            _storage.Add(key, value);
        }

        private static List<PropertyInfo> GetProperties(object obj)
        {
            List<PropertyInfo> properties;
            if (PropertiesCache.TryGetValue(obj.GetType(), out properties))
                return properties;

            properties = new List<PropertyInfo>();
            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (property.CanRead == false || property.GetIndexParameters().Length > 0)
                    continue;
                properties.Add(property);
            }
            PropertiesCache[obj.GetType()] = properties;
            return properties;
        }

        private static List<FieldInfo> GetFields(object obj)
        {
            List<FieldInfo> fields;
            if (FieldsCache.TryGetValue(obj.GetType(), out fields))
                return fields;

            fields = new List<FieldInfo>();
            foreach (var fieldInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (Attribute.IsDefined(fieldInfo, typeof (CompilerGeneratedAttribute)) == false)
                {
                    fields.Add(fieldInfo);
                }
            }
            FieldsCache[obj.GetType()] = fields;
            return fields;
        }

        /// <summary>
        ///     Copy all the public properties and fields of an object to the row
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Row FromObject(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            var row = new Row();
            foreach (var property in GetProperties(obj))
            {
                row[property.Name] = property.GetValue(obj, new object[0]);
            }
            foreach (var field in GetFields(obj))
            {
                row[field.Name] = field.GetValue(obj);
            }
            return row;
        }
    }
}