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
                using FileStream writer = new(fileName, FileMode.Create);
                DataContractSerializer ser = new(typeof(T));
                ser.WriteObject(writer, instance);
            }
            catch (Exception e)
            {
                Logger.Error($"Serializing an object of type {typeof(T).Name} to \"{fileName}\" failed.", e);
                throw;
            }
        }

        public static T DeserializeFromXml<T>(string fileName)
        {
            try
            {
                Logger.Debug($"Deserializing {typeof(T).Name} ...");

                CheckAttribute<T>();
                using FileStream fs = new(fileName,
                    FileMode.Open);
                using XmlDictionaryReader reader =
                    XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                DataContractSerializer ser = new(typeof(T));

                // Deserialize the data and read it from the instance.
                T instance = (T) ser.ReadObject(reader, true);

                return instance;
            }
            catch (Exception e)
            {
                Logger.Error($"Deserializing an object of type {typeof(T).Name} from \"{fileName}\" failed.", e);
                throw;
            }
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