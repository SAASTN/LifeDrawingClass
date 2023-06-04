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
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Models;
    using SkiaSharp;

    internal class SlideShowViewModel: INotifyPropertyChanged
    {
        #region Constants & Statics

        private const double SpeedMultiplier = 20.0;

        #endregion

        #region Properties & Fields - Non-Public

        private readonly SlideShowModel _model;
        private readonly DispatcherTimer _timer;

        #endregion

        #region Constructors

        public SlideShowViewModel(SlideShowModel slideShowModel)
        {
            this._model = slideShowModel;
            this._model.PropertyChanged += this.OnModelPropertyChanged;
            this._timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            this._timer.Tick += this.TimerTick;
        }

        #endregion

        #region Properties & Fields - Public

        public SlideShowState State => this._model.State;

        public int CurrentSegmentIndex => this._model.CurrentSegmentIndex;

        #endregion

        #region Methods - Public

        #region Methods Other

        public void StartSession() => this._model.ReStartSession(true);

        public void OnPaintImage(SKCanvas canvas, int width, int height) =>
            this._model.OnPaintImage(canvas, width, height);

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e) =>
            this.OnPropertyChanged(e.PropertyName);

        private void TimerTick(object sender, EventArgs e) => this._model.Tick(SpeedMultiplier);

        private void StartTimer()
        {
            bool timerEnabled = this._model.State switch
            {
                SlideShowState.Running => true,
                SlideShowState.Paused => true,
                SlideShowState.NotStarted => false,
                SlideShowState.Finished => false,
                _ => throw new InvalidOperationException("Unknown state.")
            };

            if (this._timer.IsEnabled != timerEnabled)
            {
                if (timerEnabled)
                {
                    this._timer.Start();
                }
                else
                {
                    this._timer.Stop();
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(SlideShowModel.Image))
            {
                this.OnRequestCanvasRefresh();
                return;
            }

            if (propertyName == nameof(this.State))
            {
                this.StartTimer();
            }

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnRequestCanvasRefresh() => this.RequestCanvasRefresh?.Invoke(this, null!);

        #endregion

        #endregion

        #region Events

        public event EventHandler RequestCanvasRefresh;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}