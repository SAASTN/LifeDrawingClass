// *****************************************************************************
//  XmlSerializationUtils.cs
//   Copyright (C) 2023 SAASTN <saastn@gmail.com>
//   This file is part of LifeDrawingClass.
// 
//   LifeDrawingClass is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
// 
//   LifeDrawingClass is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//   GNU General Public License for more details.
// 
//   You should have received a copy of the GNU General Public License
//   along with LifeDrawingClass. If not, see <https://www.gnu.org/licenses/>.
// *****************************************************************************

namespace LifeDrawingClass.Core.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using LifeDrawingClass.Core.Log;

    internal static class XmlSerializationUtils
    {
        #region Constants & Statics

        private static readonly Type AttributeConstraintType = typeof(DataContractAttribute);

        #endregion

        #region Methods - Public

        #region Methods Stat

        public static void SerializeToXml<T>(T instance, string fileName)
        {
            try
            {
                Logger.Debug($"Serializing {typeof(T).Name} ...");

                CheckAttribute<T>();
                using FileStream fs = new(fileName, FileMode.Create);
                SerializeToStream(instance, fs);
            }
            catch (Exception e)
            {
                Logger.Error($"Serializing an object of type {typeof(T).Name} to \"{fileName}\" failed.", e);
                throw;
            }
        }

        public static void SerializeToStream<T>(T instance, Stream stream)
        {
            using XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = true });
            DataContractSerializer ser = new(typeof(T));
            ser.WriteObject(writer, instance);
        }

        public static T DeserializeFromXml<T>(string fileName)
        {
            try
            {
                Logger.Debug($"Deserializing {typeof(T).Name} ...");

                CheckAttribute<T>();
                using FileStream fs = new(fileName,
                    FileMode.Open);
                return DeserializeFromStream<T>(fs);
            }
            catch (Exception e)
            {
                Logger.Error($"Deserializing an object of type {typeof(T).Name} from \"{fileName}\" failed.", e);
                throw;
            }
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            using XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new(typeof(T));

            // Deserialize the data and read it from the instance.
            T instance = (T) ser.ReadObject(reader, true);

            return instance;
        }

        public static T DeepCopy<T>(T instance) where T : ISerializableObject
        {
            using MemoryStream stream = new();
            instance.SerializeToStream(stream);
            stream.Position = 0;
            return (T) instance.DeserializeFromStream(stream);
        }

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static void CheckAttribute<T>()
        {
            object[] attributes = typeof(T).GetCustomAttributes(AttributeConstraintType, true);
            if (attributes.Length == 0)
            {
                throw new ArgumentException($"T does not have attribute {AttributeConstraintType.Name}.");
            }
        }

        #endregion

        #endregion
    }
}