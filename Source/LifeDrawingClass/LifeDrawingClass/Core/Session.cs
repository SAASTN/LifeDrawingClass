// *****************************************************************************
//  Session.cs
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
// *******************************************************************************

namespace LifeDrawingClass.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using LifeDrawingClass.Core.Image;

    public class Session: ISession
    {
        #region Properties & Fields - Non-Public

        private readonly ImageList _imageList = new();
        private int _currentSegmentIndex;

        #endregion

        #region Properties Impl - Public

        public IReadOnlyList<string> ImagePaths => this._imageList;
        public int Interval { get; set; }

        /// <inheritdoc />
        public int CurrentSegmentIndex
        {
            get => this._currentSegmentIndex;
            set
            {
                this._currentSegmentIndex = value;
                this.OnPropertyChanged(nameof(this.CurrentSegmentIndex));
            }
        }

        #endregion

        #region Methods - Public

        #region Methods Impl

        public void ImportFolder(string path, bool importSubfolders)
        {
            this._imageList.ImportFolder(path, importSubfolders);
            this.OnPropertyChanged(nameof(this.ImagePaths));
        }

        public void ClearPaths()
        {
            this._imageList.Clear();
            this.CurrentSegmentIndex = -1;
            this.OnPropertyChanged(nameof(this.ImagePaths));
        }

        public void AddPaths(string[] fileNames)
        {
            this._imageList.AddRange(fileNames);
            this.OnPropertyChanged(nameof(this.ImagePaths));
        }

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}