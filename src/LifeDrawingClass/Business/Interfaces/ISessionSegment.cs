// *****************************************************************************
//  ISessionSegment.cs
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

namespace LifeDrawingClass.Business.Interfaces
{
    using System;
    using LifeDrawingClass.Core.Serialization;

    public interface ISessionSegment: ISerializableObject
    {
        SessionSegmentType Type { get; set; }
        TimeSpan Duration { get; set; }

        /// <summary>
        ///     Only can be true when <see cref="Type" /> is equal to <see cref="SessionSegmentType.Break" />. See
        ///     <see cref="ISessionProperties.MaxBreakShift" /> for more details.
        /// </summary>
        bool ChangeImageAfterBreak { get; set; }
    }
}