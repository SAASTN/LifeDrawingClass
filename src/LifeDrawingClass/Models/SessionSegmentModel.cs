// *****************************************************************************
//  SessionSegmentModel.cs
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

namespace LifeDrawingClass.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;

    public class SessionSegmentModel
    {
        #region Constructors

        public SessionSegmentModel(SessionSegmentType type, int duration, int count)
        {
            this.Type = type;
            this.Duration = duration;
            this.Count = count;
        }

        #endregion

        #region Properties & Fields - Public

        public string DurationText
        {
            get
            {
                TimeSpan duration = TimeSpan.FromMilliseconds(this.Duration);
                int minutes = (int) duration.TotalMinutes;
                int seconds = (duration.Seconds / 10) * 10;
                string result = (minutes > 0 ? $"{minutes}'" : "") + (seconds > 0 ? $"{seconds}\"" : "");
                if (this.Count > 1)
                {
                    result = $"{this.Count} × {result}";
                }

                return result;
            }
        }

        public SessionSegmentType Type { get; }
        public int Duration { get; }
        public int Count { get; }

        #endregion

        #region Methods - Public

        #region Methods Stat

        public static List<SessionSegmentModel> MergeSegment(IEnumerable<ISessionSegment> segments)
        {
            List<ISessionSegment> segmentsList = new(segments);
            List<SessionSegmentModel> results = new();
            int i = 0;
            while (i < segmentsList.Count)
            {
                int count = 1;
                for (int j = i + 1; j < segmentsList.Count; j++)
                {
                    if ((segmentsList[i].Type == segmentsList[j].Type) && (segmentsList[i].DurationMilliseconds ==
                                                                           segmentsList[j].DurationMilliseconds))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }

                results.Add(new SessionSegmentModel(segmentsList[i].Type, segmentsList[i].DurationMilliseconds, count));
                i += count;
            }

            return results;
        }

        public static List<ISessionSegment> ExpandSegments(IEnumerable<SessionSegmentModel> segments)
        {
            List<ISessionSegment> results = new();
            foreach (SessionSegmentModel segment in segments)
            {
                results.AddRange(Enumerable.Range(0, segment.Count).Select(_ => new SessionSegment()
                    { DurationMilliseconds = segment.Duration, GroupId = -1, Type = segment.Type }));
            }

            return results;
        }
        #endregion

        #endregion
    }
}