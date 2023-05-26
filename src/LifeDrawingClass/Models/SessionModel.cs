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
    using System.ComponentModel;
    using System.Linq;
    using CommunityToolkit.Mvvm.ComponentModel;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Configuration;

    public class SessionModel: ObservableObject

    {
        #region Properties & Fields - Non-Public

        private readonly ISession _session;

        /// <summary>
        ///     Key is the name of property of th <see cref="ISession" /> instance, and value is the name of property of the
        ///     <see cref="SessionModel" /> instance.
        ///     When <see cref="SessionModel" /> gets notified that one of properties of its <see cref="ISession" /> has change, it
        ///     notifies its ViewModels by calling
        ///     <see cref="ObservableObject.OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs)" /> with the name of
        ///     its own property.
        /// </summary>
        private readonly Dictionary<string, string> _sessionPropertyBindings = new()
        {
            { nameof(ISession.CurrentSegmentIndex), nameof(CurrentSegmentIndex) },
            { nameof(ISession.ImagePaths), nameof(ImagePaths) }
        };

        #endregion

        #region Constructors

        public SessionModel(ISession session)
        {
            this._session = session;
            session.PropertyChanged += this.SessionPropertyChanged;
        }

        #endregion

        #region Properties & Fields - Public

        public int CurrentSegmentIndex;

        public IReadOnlyList<string> ImagePaths => this._session.ImagePaths;

        public IReadOnlyList<SessionSegmentModel> Segments => this._session.Segments
            .Select(s => new SessionSegmentModel(s.Type, s.DurationMilliseconds, 1)).ToList();

        public IReadOnlyList<SessionSegmentModel> MergedSegments =>
            SessionSegmentModel.MergeSegment(this._session.Segments);

        public int Interval
        {
            get => this._session.Interval;
            set => this._session.Interval = value;
        }

        #endregion

        #region Methods - Public

        #region Methods Other

        public void ImportFolder(string path, bool importSubfolders) =>
            this._session.ImportFolder(path, importSubfolders);

        public void ClearPaths() => this._session.ClearPaths();

        public void AddPaths(string[] fileNames) =>
            this._session.AddPaths(fileNames);

        public void StartSession() => this._session.StartSession();

        internal void SaveToConfigs() => this._session.SerializeToXml(Configurator.GetLastSessionFileName());

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void SessionPropertyChanged(object sender, PropertyChangedEventArgs e) =>
            this.OnPropertyChanged(e.PropertyName != null ? this._sessionPropertyBindings[e.PropertyName] : null);

        #endregion

        #endregion
    }
}