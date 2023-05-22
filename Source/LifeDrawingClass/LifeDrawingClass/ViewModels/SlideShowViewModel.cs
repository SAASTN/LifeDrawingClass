// *****************************************************************************
//  SlideShowViewModel.cs
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
    using System.ComponentModel;
    using System.Windows.Threading;
    using LifeDrawingClass.Models;

    internal class SlideShowViewModel: INotifyPropertyChanged
    {
        #region Properties & Fields - Non-Public

        private readonly SessionModel _sessionModel;

        private int _currentIndex;

        #endregion

        #region Constructors

        public SlideShowViewModel(SessionModel sessionModel)
        {
            this._sessionModel = sessionModel;
            DispatcherTimer timer = new()
            {
                Interval = new TimeSpan(0, 0, 0, 0, this._sessionModel.Interval)
            };
            timer.Tick += this.Timer_Tick;
            timer.Start();
        }

        #endregion

        #region Properties & Fields - Public

        public int CurrentSegmentIndex;

        public string CurrentImagePath => this._sessionModel.ImagePaths[this._currentIndex];

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void Timer_Tick(object sender, EventArgs e)
        {
            this._currentIndex = (this._currentIndex + 1) % this._sessionModel.ImagePaths.Count;
            this.OnPropertyChanged(nameof(this.CurrentImagePath));
        }

        protected virtual void OnPropertyChanged(string propertyName) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}