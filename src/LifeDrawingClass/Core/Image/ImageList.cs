// *****************************************************************************
//  ImageList.cs
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

namespace LifeDrawingClass.Core.Image
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    internal class ImageList
    {
        #region Constants & Statics

        internal static readonly HashSet<string> Extensions;

        #endregion

        #region Properties & Fields - Non-Public

        [DataMember] private List<string> _paths = new();

        #endregion

        #region Constructors

        static ImageList()
        {
            Extensions = new HashSet<string> { ".png", ".bmp", ".jpg", ".jpeg" };
        }

        public ImageList()
        {
        }

        public ImageList(ICollection<string> items)
        {
            this.AddRange(items);
        }

        #endregion

        #region Methods - Public

        #region Methods Other

        public IReadOnlyList<string> AsList() => this._paths;

        internal void AddRange(ICollection<string> newItems)
        {
            this._paths.AddRange(newItems.Where(np => !this._paths.Contains(np)));
            this._paths.Sort();
        }


        internal void ImportFolder(string path, bool subFolders) =>
            this.AddRange(FindImageFilesInFolder(path, subFolders));

        public void Clear() => this._paths.Clear();

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static List<string> FindImageFilesInFolder(string directoryPath, bool importSubDirectories)
        {
            List<string> filePaths = new();

            try
            {
                DirectoryInfo directory = new(directoryPath);

                foreach (FileInfo file in directory.GetFiles())
                {
                    if (Extensions.Contains(file.Extension.ToLower()))
                    {
                        filePaths.Add(file.FullName);
                    }
                }

                if (importSubDirectories)
                {
                    foreach (DirectoryInfo subdirectory in directory.GetDirectories())
                    {
                        filePaths.AddRange(FindImageFilesInFolder(subdirectory.FullName, true));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while searching for files: {ex.Message}");
            }

            return filePaths;
        }

        #endregion

        #endregion
    }
}