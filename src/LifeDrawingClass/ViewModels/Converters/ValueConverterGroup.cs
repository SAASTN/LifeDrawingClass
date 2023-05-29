// *****************************************************************************
//  ValueConverterGroup.cs
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    ///     Source: https://stackoverflow.com/a/8326207/2093077
    /// </summary>
    public class ValueConverterGroup: List<IValueConverter>, IValueConverter
    {
        #region Methods - Public

        #region Methods Impl

        public object
            Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
            this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) => throw new NotImplementedException();

        #endregion

        #endregion
    }
}