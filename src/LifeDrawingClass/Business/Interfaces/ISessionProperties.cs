// *****************************************************************************
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
    using System.Collections.Generic;
    using LifeDrawingClass.Core.Serialization;

    public interface ISessionProperties: ISerializableObject
    {
        SessionSegmentDesignType DesignType { get; set; }

        /// <summary>
        ///     Total class length in minutes.
        /// </summary>
        bool SessionDuration { get; set; }

        /// <summary>
        ///     Specifies whether the session starts with a warm-up.
        /// </summary>
        bool AddWarmUp { get; set; }

        /// <summary>
        ///     For example when it contains [1, 5, 15], it means that designer can create only 1m, 5m, and 15m warm-up segments.
        /// </summary>
        List<int> AvailableWarmUpDurations { get; set; }

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
        List<int> AvailableCoolDownDurations { get; set; }

        /// <summary>
        ///     Determines the <b>approximate</b> ratio of the warm-up duration to the total session time.
        /// </summary>
        double CoolDownPercent { get; set; }

        int NumberOfLongPoses { get; set; }

        bool AddBreaks { get; set; }

        /// <summary>
        ///     Determines the <b>approximate</b> ratio of the breaks duration to the total session time.
        /// </summary>
        double BreakPercent { get; set; }

        double NumberOfBreak { get; set; }

        string ManualSegmentsDefinition { get; set; }

        List<ISessionSegment> GetSegments();
    }
}