// *****************************************************************************
//  BooleanIffConverter.cs
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

    public class BooleanIffConverter: IValueConverter
    {
        #region Properties & Fields - Public

        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        public bool NullValue { get; set; } = false;

        #endregion

        #region Methods - Public

        #region Methods Impl

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool) (value ?? this.NullValue) ? this.TrueValue : this.FalseValue;

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();

        #endregion

        #endregion
    }
}