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
    using CommunityToolkit.Mvvm.ComponentModel;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Configuration;

    public class SessionPropertiesModel: ObservableObject
    {
        #region Properties & Fields - Non-Public

        private SessionSegmentDesignType _designType;
        private int _sessionDuration;

        #endregion

        #region Constructors

        public SessionPropertiesModel(ISessionProperties properties)
        {
            this.Initialize(properties);
        }

        #endregion

        #region Properties & Fields - Public

        public SessionSegmentDesignType DesignType
        {
            get => this._designType;
            set => this.SetProperty(ref this._designType, value, nameof(this.DesignType));
        }

        public int SessionDuration
        {
            get => this._sessionDuration;
            set
            {
                value = (int) (Math.Round(value / 5.0) * 5.0);
                this.SetProperty(ref this._sessionDuration, value, nameof(this.SessionDuration));
            }
        }

        #endregion

        #region Methods - Public

        #region Methods Other

        internal void SaveToConfigs() =>
            this.GetProperties().SerializeToXml(Configurator.GetLastSessionPropertiesFileName());

        public ISessionProperties GetProperties() => new SessionProperties()
        {
            DesignType = this.DesignType
        };

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void Initialize(ISessionProperties properties)
        {
            this._designType = properties.DesignType;
            this.OnPropertyChanged();
        }

        #endregion

        #endregion
    }
}