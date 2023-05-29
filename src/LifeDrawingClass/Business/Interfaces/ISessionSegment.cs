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
    using LifeDrawingClass.Core.Serialization;

    public interface ISessionSegment: ISerializableObject
    {
        /// <summary>
        ///     in cases when a break is in middle of a long-pose segment, the long-pose will break into two segments, but both
        ///     need to show same slide. So, they are grouped together.
        /// </summary>
        int GroupId { get; set; }

        SessionSegmentType Type { get; set; }
        int DurationMilliseconds { get; set; }
    }
}