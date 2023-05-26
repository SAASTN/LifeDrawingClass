// *****************************************************************************
//  SessionSegmentDesignerViewModel.cs
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
    using LifeDrawingClass.Models;

    public class SessionSegmentDesignerViewModel: ObservableObject
    {
        #region Properties & Fields - Non-Public

        private readonly SessionSegmentDesignerModel _designerModelOriginal;
        private ICommand _ok;
        private ICommand _cancel;

        #endregion

        #region Constructors

        public SessionSegmentDesignerViewModel(SessionSegmentDesignerModel designerModel)
        {
            this._designerModelOriginal = designerModel;
            this.DesignerModel = designerModel.DeepCopy();
        }

        #endregion

        #region Properties & Fields - Public

        public SessionSegmentDesignerModel DesignerModel { get; }

        public ICommand OkCommand => this._ok ??= new RelayCommand(this.Ok);
        public ICommand CancelCommand => this._cancel ??= new RelayCommand(this.Cancel);
        public SessionSegmentDesignerModel Result { get; private set; }

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        protected void OnClosingRequest()
        {
            if (this.ClosingRequest != null)
            {
                this.ClosingRequest(this, EventArgs.Empty);
            }
        }

        private void Ok()
        {
            this.Result = this.DesignerModel;
            this.OnClosingRequest();
        }

        private void Cancel()
        {
            this.Result = this._designerModelOriginal;
            this.OnClosingRequest();
        }

        #endregion

        #endregion

        #region Events

        public event EventHandler ClosingRequest;

        #endregion
    }
}