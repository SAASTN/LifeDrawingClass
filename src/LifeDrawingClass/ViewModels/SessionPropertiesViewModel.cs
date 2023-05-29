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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Models;

    public class SessionPropertiesViewModel: ObservableObject
    {
        #region Properties & Fields - Non-Public

        /// <summary>
        ///     Used in case user cancels changes.
        /// </summary>
        private readonly SessionPropertiesModel _propertiesModelOriginal;

        /// <summary>
        ///     It is a deep copy of the <see cref="_propertiesModelOriginal" />. It is used in case user accepts changes.
        ///     View-model
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
            this.Result = this._propertiesModelOriginal;
        }

        #endregion

        #region Properties & Fields - Public

        public ICommand OkCommand => this._ok ??= new RelayCommand(this.Ok);
        public ICommand CancelCommand => this._cancel ??= new RelayCommand(this.Cancel);

        public ObservableCollection<SessionSegmentModel> Segments => this._propertiesModelCopy.Segments;
        public SessionPropertiesModel Result { get; private set; }

        /// <inheritdoc cref="ISessionProperties.DesignType" />
        public SessionSegmentDesignType DesignType
        {
            get => this._propertiesModelCopy.DesignType;
            set
            {
                this._propertiesModelCopy.DesignType = value;
                this.OnPropertyChanged(nameof(this.DesignType));
            }
        }

        /// <inheritdoc cref="ISessionProperties.SessionDuration" />
        public int SessionDuration
        {
            get => this._propertiesModelCopy.SessionDuration;
            set
            {
                this._propertiesModelCopy.SessionDuration = value;
                this.OnPropertyChanged(nameof(this.SessionDuration));
            }
        }

        /// <inheritdoc cref="ISessionProperties.NumberOfLongPoses" />
        public int NumberOfLongPoses
        {
            get => this._propertiesModelCopy.NumberOfLongPoses;
            set
            {
                this._propertiesModelCopy.NumberOfLongPoses = value;
                this.OnPropertyChanged(nameof(this.NumberOfLongPoses));
            }
        }

        /// <inheritdoc cref="ISessionProperties.AddWarmUp" />
        public bool AddWarmUp
        {
            get => this._propertiesModelCopy.AddWarmUp;
            set
            {
                this._propertiesModelCopy.AddWarmUp = value;
                this.OnPropertyChanged(nameof(this.AddWarmUp));
            }
        }

        /// <inheritdoc cref="ISessionProperties.AddCoolDown" />
        public bool AddCoolDown
        {
            get => this._propertiesModelCopy.AddCoolDown;
            set
            {
                this._propertiesModelCopy.AddCoolDown = value;
                this.OnPropertyChanged(nameof(this.AddCoolDown));
            }
        }

        /// <inheritdoc cref="ISessionProperties.AddBreaks" />
        public bool AddBreaks
        {
            get => this._propertiesModelCopy.AddBreaks;
            set
            {
                this._propertiesModelCopy.AddBreaks = value;
                this.OnPropertyChanged(nameof(this.AddBreaks));
            }
        }

        /// <inheritdoc cref="ISessionProperties.IsSimplified" />
        public bool IsSimplified
        {
            get => this._propertiesModelCopy.IsSimplified;
            set
            {
                this._propertiesModelCopy.IsSimplified = value;
                this.OnPropertyChanged(nameof(this.IsSimplified));
            }
        }

        #endregion

        #region Methods - Non-Public

        #region Methods Impl

        /// <inheritdoc />
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if ((e.PropertyName ?? "") != nameof(this.Segments))
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Segments)));
            }
        }

        #endregion

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