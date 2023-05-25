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
// *****************************************************************************

namespace LifeDrawingClass.Business
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Image;
    using LifeDrawingClass.Core.Serialization;

    [DataContract]
    public class Session: ISession
    {
        #region Properties & Fields - Non-Public

        [DataMember] private ImageList _imageList;

        private int _currentSegmentIndex;

        /// <remarks>
        ///     The internal field is declared as a list of <see cref="SessionSegment" />s instead of
        ///     <see cref="ISessionSegment" />s for serialization, as <see cref="DataContractSerializer" /> does not support
        ///     interfaces. if there was other <see cref="ISessionSegment" /> implementations, the serialization part must be
        ///     rewritten.
        /// </remarks>
        [DataMember] private List<SessionSegment> _segments;

        #endregion

        #region Properties Impl - Public

        public IReadOnlyList<string> ImagePaths
        {
            get => (this._imageList ??= new ImageList()).AsList();
            set
            {
                this._imageList = new ImageList(value.ToList());
                this.OnPropertyChanged(nameof(this.ImagePaths));
            }
        }

        /// <inheritdoc />

        public IReadOnlyList<ISessionSegment> Segments
        {
            get => this._segments ??= new List<SessionSegment>()
            {
                new() { DurationMilliseconds = 61234, Type = SessionSegmentType.WarmUp },
                new() { DurationMilliseconds = 61234, Type = SessionSegmentType.WarmUp },
                new() { DurationMilliseconds = 61234, Type = SessionSegmentType.WarmUp },
                new() { DurationMilliseconds = 6023456, Type = SessionSegmentType.LongPose },
                new() { DurationMilliseconds = 16654, Type = SessionSegmentType.CoolDown },
                new() { DurationMilliseconds = 16725, Type = SessionSegmentType.Break }
            };
            set
            {
                if (value != null)
                {
                    this._segments = value.Cast<SessionSegment>().ToList();
                    this.OnPropertyChanged(nameof(this.Segments));
                }
            }
        }

        [DataMember] public int Interval { get; set; }

        /// <inheritdoc />
        [DataMember]
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

        /// <inheritdoc />
        public void SerializeToXml(string fileName) => XmlSerializationUtils.SerializeToXml(this, fileName);

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

        public void StartSession() => this.StartSegment(0);

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void StartSegment(int segmentIndex) => this.CurrentSegmentIndex = segmentIndex;

        #endregion

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}