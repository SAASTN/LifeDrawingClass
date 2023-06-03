// *****************************************************************************
//  SessionModel.cs
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

namespace LifeDrawingClass.Models
{
    using System.Collections.Generic;
    using CommunityToolkit.Mvvm.ComponentModel;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Configuration;
    using LifeDrawingClass.Core.Image;

    public class NewSessionModel: ObservableObject

    {
        #region Properties & Fields - Non-Public

        private ImageList _imageList;
        private IReadOnlyList<SessionSegmentModel> _mergedSegments;

        #endregion

        #region Constructors

        public NewSessionModel(ISession session)
        {
            this.Initialize(session);
        }

        #endregion

        #region Properties & Fields - Public

        public int CurrentSegmentIndex;

        public IReadOnlyList<string> ImagePaths => this._imageList.AsList();

        public IReadOnlyList<SessionSegmentModel> MergedSegments
        {
            get => this._mergedSegments;
            set => this.SetProperty(ref this._mergedSegments,
                SessionSegmentModel.MergeSegment(SessionSegmentModel.ExpandSegments(value)),
                nameof(this.MergedSegments));
        }


        #endregion

        #region Methods - Public

        #region Methods Other

        public void ImportFolder(string path, bool importSubfolders) =>
            this._imageList.ImportFolder(path, importSubfolders);

        public void ClearPaths() => this._imageList.Clear();

        public void AddPaths(string[] fileNames) =>
            this._imageList.AddRange(fileNames);

        internal void SaveToConfigs() => this.GetSession().SerializeToXml(Configurator.GetLastSessionFileName());

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void Initialize(ISession session)
        {
            this._imageList = new ImageList(session.ImagePaths);
            this._mergedSegments = SessionSegmentModel.MergeSegment(session.Segments);
            this.OnPropertyChanged();
        }

        internal ISession GetSession() => new Session()
        {
            ImagePaths = this.ImagePaths,
            Segments = SessionSegmentModel.ExpandSegments(this.MergedSegments),
            CurrentSegmentIndex = -1
        };

        #endregion

        #endregion
    }
}