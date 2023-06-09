﻿// *****************************************************************************
//  ISessionProperties.cs
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
    using System.Collections.Generic;
    using LifeDrawingClass.Core.Serialization;

    public interface ISessionProperties: ISerializableObject
    {
        SessionSegmentDesignType DesignType { get; set; }

        /// <summary>
        ///     Total class length.
        /// </summary>
        TimeSpan SessionDuration { get; set; }

        /// <summary>
        ///     Specifies whether the session starts with a warm-up.
        /// </summary>
        bool AddWarmUp { get; set; }

        /// <summary>
        ///     For example when it contains [1, 5, 15], it means that designer can create only 1m, 5m, and 15m warm-up segments.
        /// </summary>
        List<TimeSpan> AvailableWarmUpDurations { get; set; }

        /// <summary>
        ///     Determines the <b>approximate</b> ratio of the warm-up duration to the total session time.
        /// </summary>
        double WarmUpPercent { get; set; }

        /// <summary>
        ///     Specifies whether the session ends with a cool-down.
        /// </summary>
        bool AddCoolDown { get; set; }

        /// <summary>
        ///     For example when it contains [1, 5, 15], it means that designer can create only 1m, 5m, and 15m cool-down segments.
        /// </summary>
        List<TimeSpan> AvailableCoolDownDurations { get; set; }

        /// <summary>
        ///     Determines the <b>approximate</b> ratio of the warm-up duration to the total session time.
        /// </summary>
        double CoolDownPercent { get; set; }

        int NumberOfLongPoses { get; set; }

        bool AddBreaks { get; set; }

        /// <summary>
        ///     Duration of each of break segments.
        /// </summary>
        TimeSpan BreaksDuration { get; set; }

        int NumberOfBreaks { get; set; }

        /// <remarks>
        ///     The designer tries to put breaks in a way the the whole session be divided to <see cref="NumberOfBreaks" /> + 1
        ///     parts. But when the calculated time for the break is in the middle of one of the segments, the designer tries to
        ///     move the break to the closest point between the two segments. however, when this displacement is greater than this
        ///     value (the <see cref="MaxBreakShift" />),the designer divides the segment into two parts. In the second part
        ///     of the segment, which starts after the break, the previous image will still be displayed. This is done by
        ///     setting the <see cref="ISessionSegment.ChangeImageAfterBreak" /> of the break segment to false.
        /// </remarks>
        TimeSpan MaxBreakShift { get; set; }

        /// <summary>
        ///     If true, the designer only uses <see cref="SessionDuration" />, <see cref="NumberOfLongPoses" />,
        ///     <see cref="AddWarmUp" />, <see cref="AddCoolDown" />, and <see cref="AddBreaks" /> and decides about the rest of
        ///     properties.
        /// </summary>
        bool IsSimplified { get; set; }

        string ManualSegmentsDefinition { get; set; }
    }
}