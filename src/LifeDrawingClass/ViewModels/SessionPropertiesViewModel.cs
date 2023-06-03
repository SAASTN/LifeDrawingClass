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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Models;
    using MahApps.Metro.Controls;

    public class SessionPropertiesViewModel: ObservableObject, IDataErrorInfo
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
            this._propertiesModelCopy.PropertyChanged += this.OnModelPropertyChanged;
            this.Result = this._propertiesModelOriginal;
        }

        #endregion

        #region Properties & Fields - Public

        public ICommand OkCommand => this._ok ??= new RelayCommand(this.Ok);
        public ICommand CancelCommand => this._cancel ??= new RelayCommand(this.Cancel);

        public IReadOnlyList<SessionSegmentModel> MergedSegments => this._propertiesModelCopy.MergedSegments;

        public ObservableCollection<SessionDefinitionParser.ParserMessageItem> ParsingMessages =>
            this._propertiesModelCopy.ParsingMessages;

        public bool HasParsingIssue => this._propertiesModelCopy.HasParsingIssue;
        public SessionPropertiesModel Result { get; private set; }

        /// <inheritdoc cref="ISessionProperties.DesignType" />
        public SessionSegmentDesignType DesignType
        {
            get => this._propertiesModelCopy.DesignType;
            set => this._propertiesModelCopy.DesignType = value;
        }

        /// <inheritdoc cref="ISessionProperties.SessionDuration" />
        public int SessionDurationMinutes
        {
            get => this._propertiesModelCopy.SessionDurationMinutes;
            set => this._propertiesModelCopy.SessionDurationMinutes = value;
        }

        /// <inheritdoc cref="ISessionProperties.NumberOfLongPoses" />
        public int NumberOfLongPoses
        {
            get => this._propertiesModelCopy.NumberOfLongPoses;
            set => this._propertiesModelCopy.NumberOfLongPoses = value;
        }

        /// <inheritdoc cref="ISessionProperties.AddWarmUp" />
        public bool AddWarmUp
        {
            get => this._propertiesModelCopy.AddWarmUp;
            set => this._propertiesModelCopy.AddWarmUp = value;
        }

        /// <inheritdoc cref="ISessionProperties.AddCoolDown" />
        public bool AddCoolDown
        {
            get => this._propertiesModelCopy.AddCoolDown;
            set => this._propertiesModelCopy.AddCoolDown = value;
        }

        /// <inheritdoc cref="ISessionProperties.AddBreaks" />
        public bool AddBreaks
        {
            get => this._propertiesModelCopy.AddBreaks;
            set => this._propertiesModelCopy.AddBreaks = value;
        }

        /// <inheritdoc cref="ISessionProperties.IsSimplified" />
        // ReSharper disable once UnusedMember.Global
        public bool IsSimplified
        {
            get => this._propertiesModelCopy.IsSimplified;
            set => this._propertiesModelCopy.IsSimplified = value;
        }

        /// <inheritdoc cref="ISessionProperties.ManualSegmentsDefinition" />
        public string ManualSegmentsDefinition
        {
            get => this._propertiesModelCopy.ManualSegmentsDefinition;
            set => this._propertiesModelCopy.ManualSegmentsDefinition = value;
        }

        #endregion

        #region Properties Impl - Public

        /// <inheritdoc />
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if ((columnName == nameof(this.ManualSegmentsDefinition)) && this.HasParsingIssue)
                {
                    return "There are some issues, check the messages below.";
                }

                return null;
            }
        }

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void OnModelPropertyChanged(object _, PropertyChangedEventArgs e) =>
            this.OnPropertyChanged(e.PropertyName);

        protected void OnClosingRequest(bool canceled) =>
            this.ClosingRequest?.Invoke(this, new ClosingWindowEventHandlerArgs() { Cancelled = canceled });

        private void Ok()
        {
            this.Result = this._propertiesModelCopy;
            this.OnClosingRequest(false);
        }

        private void Cancel()
        {
            this.Result = this._propertiesModelOriginal;
            this.OnClosingRequest(true);
        }

        #endregion

        #endregion

        #region Events

        public event EventHandler<ClosingWindowEventHandlerArgs> ClosingRequest;

        #endregion
    }
}