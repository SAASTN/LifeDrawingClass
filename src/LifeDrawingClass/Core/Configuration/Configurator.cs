// *****************************************************************************
//  Configurator.cs
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

namespace LifeDrawingClass.Core.Configuration
{
    using System;
    using System.IO;

    internal static class Configurator
    {
        #region Constants & Statics

        public const string ApplicationName = "LifeDrawingClass";
        public const string ApplicationTitle = "Life-Drawing Class";
        private const string LastSessionFileName = "LastSession.xml";
        private const string LastSessionPropertiesFileName = "LastProperties.xml";

        #endregion

        #region Constructors

        static Configurator()
        {
            CreateAppDataFolder();
        }

        #endregion

        #region Methods - Public

        #region Methods Stat

        public static string GetLastSessionFileName() => Path.Combine(GetAppDataFolderPath(), LastSessionFileName);

        public static string GetLastSessionPropertiesFileName() =>
            Path.Combine(GetAppDataFolderPath(), LastSessionPropertiesFileName);

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static void CreateAppDataFolder() => Directory.CreateDirectory(GetAppDataFolderPath());

        private static string GetAppDataFolderPath() => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            , ApplicationName + Path.DirectorySeparatorChar);

        #endregion

        #endregion
    }
}