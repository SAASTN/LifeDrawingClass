// *****************************************************************************
//  SessionPropertiesViewModel.cs
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

namespace LifeDrawingClass.ViewModels
{
    using System;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Models;

    public class SessionPropertiesViewModel: ObservableObject
    {
        #region Properties & Fields - Non-Public

        /// <summary>
        ///     Used in case user cancels changes.
        /// </summary>
        private readonly SessionPropertiesModel _propertiesModelOriginal;

        /// <summary>
        ///     It is a deep copy of the <see cref="_propertiesModelOriginal" />. It is used in case user accepts changes. View-model
        ///     uses this instance and the window is bound to it.
        /// </summary>
        private readonly SessionPropertiesModel _propertiesModelCopy;

        private ICommand _ok;
        private ICommand _cancel;

        #endregion

        #region Constructors

        public SessionPropertiesViewModel(SessionPropertiesModel propertiesModel)
        {
            this._propertiesModelOriginal = propertiesModel;
            this._propertiesModelCopy = new SessionPropertiesModel(propertiesModel.GetProperties());
        }

        #endregion

        #region Properties & Fields - Public

        public ICommand OkCommand => this._ok ??= new RelayCommand(this.Ok);
        public ICommand CancelCommand => this._cancel ??= new RelayCommand(this.Cancel);
        public SessionPropertiesModel Result { get; private set; }

        public SessionSegmentDesignType DesignType
        {
            get => this._propertiesModelCopy.DesignType;
            set
            {
                this._propertiesModelCopy.DesignType = value;
                this.OnPropertyChanged(nameof(this.DesignType));
            }
        }

        public int SessionDuration
        {
            get => this._propertiesModelCopy.SessionDuration;
            set
            {
                this._propertiesModelCopy.SessionDuration = value;
                this.OnPropertyChanged(nameof(this.SessionDuration));
            }
        }

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        protected void OnClosingRequest() => this.ClosingRequest?.Invoke(this, EventArgs.Empty);

        private void Ok()
        {
            this.Result = this._propertiesModelCopy;
            this.OnClosingRequest();
        }

        private void Cancel()
        {
            this.Result = this._propertiesModelOriginal;
            this.OnClosingRequest();
        }

        #endregion

        #endregion

        #region Events

        public event EventHandler ClosingRequest;

        #endregion
    }
}