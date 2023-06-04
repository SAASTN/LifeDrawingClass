// *****************************************************************************
//  SlideShowWindow.xaml.cs
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

namespace LifeDrawingClass.Views.Windows
{
    using System;
    using LifeDrawingClass.ViewModels;
    using SkiaSharp.Views.Desktop;

    /// <summary>
    ///     Interaction logic for SlideShowWindow.xaml
    /// </summary>
    public partial class SlideShowWindow
    {
        #region Constructors

        public SlideShowWindow()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods - Public

        #region Methods Other

        public void OnRefreshRequested(object sender, EventArgs e) => this.Canvas.InvalidateVisual();

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e) =>
            (this.DataContext as SlideShowViewModel)?.OnPaintImage(e.Surface.Canvas, e.Info.Width, e.Info.Height);

        #endregion

        #endregion
    }
}