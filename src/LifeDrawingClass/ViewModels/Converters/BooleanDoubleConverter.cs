// *****************************************************************************
//  BooleanDoubleConverter.cs
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

namespace LifeDrawingClass.ViewModels.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal class BooleanDoubleConverter: IValueConverter
    {
        #region Methods - Public

        #region Methods Impl

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool) (value ?? false) ? 1.0 : 0.0 * (parameter is double d ? d : 1.0d);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (double) (value ?? 0.0d) != 0.0d;

        #endregion

        #endregion
    }
}