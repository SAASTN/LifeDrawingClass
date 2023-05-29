// *****************************************************************************
//  SessionPropertiesModel.cs
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
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using CommunityToolkit.Mvvm.ComponentModel;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Configuration;

    public class SessionPropertiesModel: ObservableObject
    {
        #region Properties & Fields - Non-Public

        private SessionSegmentDesignType _designType;
        private int _sessionDuration;
        private int _numberOfLongPoses;
        private bool _addWarmUp;
        private bool _addCoolDown;
        private bool _addBreaks;
        private bool _isSimplified;

        #endregion

        #region Constructors

        public SessionPropertiesModel(ISessionProperties properties)
        {
            this.Initialize(properties);
        }

        #endregion

        #region Properties & Fields - Public

        public ObservableCollection<SessionSegmentModel> Segments =>
            new(SessionSegmentModel.MergeSegment(SessionProperties.GetSegments(this.GetProperties())));

        public SessionSegmentDesignType DesignType
        {
            get => this._designType;
            set => this.SetProperty(ref this._designType, value, nameof(this.DesignType));
        }

        /// <inheritdoc cref="ISessionProperties.SessionDuration" />
        public int SessionDuration
        {
            get => this._sessionDuration;
            set
            {
                value = (int) (Math.Round(value / 5.0) * 5.0);
                this.SetProperty(ref this._sessionDuration, value, nameof(this.SessionDuration));
            }
        }

        /// <inheritdoc cref="ISessionProperties.NumberOfLongPoses" />
        public int NumberOfLongPoses
        {
            get => this._numberOfLongPoses;
            set => this.SetProperty(ref this._numberOfLongPoses, value, nameof(this.NumberOfLongPoses));
        }

        /// <inheritdoc cref="ISessionProperties.AddWarmUp" />
        public bool AddWarmUp
        {
            get => this._addWarmUp;
            set => this.SetProperty(ref this._addWarmUp, value, nameof(this.AddWarmUp));
        }

        /// <inheritdoc cref="ISessionProperties.AddCoolDown" />
        public bool AddCoolDown
        {
            get => this._addCoolDown;
            set => this.SetProperty(ref this._addCoolDown, value, nameof(this.AddCoolDown));
        }

        /// <inheritdoc cref="ISessionProperties.AddBreaks" />
        public bool AddBreaks
        {
            get => this._addBreaks;
            set => this.SetProperty(ref this._addBreaks, value, nameof(this.AddBreaks));
        }

        /// <inheritdoc cref="ISessionProperties.IsSimplified" />
        public bool IsSimplified
        {
            get => this._isSimplified;
            set => this.SetProperty(ref this._isSimplified, value, nameof(this.IsSimplified));
        }

        #endregion

        #region Methods - Public

        #region Methods Other

        internal void SaveToConfigs() =>
            this.GetProperties().SerializeToXml(Configurator.GetLastSessionPropertiesFileName());

        public ISessionProperties GetProperties() => new SessionProperties()
        {
            DesignType = this.DesignType,
            SessionDuration = this.SessionDuration,
            NumberOfLongPoses = this.NumberOfLongPoses,
            AddWarmUp = this.AddWarmUp,
            AddCoolDown = this.AddCoolDown,
            AddBreaks = this.AddBreaks,
            IsSimplified = this.IsSimplified
        };

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Impl

        /// <inheritdoc />
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if ((e.PropertyName ?? "") != nameof(this.Segments))
            {
                this.OnPropertyChanged(nameof(this.Segments));
            }
        }

        #endregion

        #region Methods Other

        private void Initialize(ISessionProperties properties)
        {
            this._designType = properties.DesignType;
            this._sessionDuration = properties.SessionDuration;
            this._numberOfLongPoses = properties.NumberOfLongPoses;
            this._addWarmUp = properties.AddWarmUp;
            this._addCoolDown = properties.AddCoolDown;
            this._addBreaks = properties.AddBreaks;
            this._isSimplified = properties.IsSimplified;
            this.OnPropertyChanged();
        }

        #endregion

        #endregion
    }
}