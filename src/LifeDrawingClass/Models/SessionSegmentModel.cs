﻿// *****************************************************************************
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
    using System.Globalization;
    using System.Linq;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;

    public class SessionSegmentModel
    {
        #region Constructors

        public SessionSegmentModel(SessionSegmentType type, TimeSpan duration, int count, bool changeImageAfterBreak)
        {
            this.Type = type;
            this.Duration = duration;
            this.Count = count;
            this.ChangeImageAfterBreak = changeImageAfterBreak;
        }

        #endregion

        #region Properties & Fields - Public

        public string DurationText
        {
            get
            {
                string text = FormatDuration(this.Duration, this.Count);
                if ((this.Type == SessionSegmentType.Break) & !this.ChangeImageAfterBreak)
                {
                    text = $"◂{text}▸";
                }

                return text;
            }
        }

        public SessionSegmentType Type { get; }
        public TimeSpan Duration { get; }
        public int Count { get; }
        public bool ChangeImageAfterBreak { get; }

        #endregion

        #region Methods - Public

        #region Methods Stat

        public static string FormatDuration(TimeSpan duration, int count)
        {
            string result;
            if (duration.TotalMinutes > 120)
            {
                result = duration.ToString("h\\:mm", CultureInfo.InvariantCulture);
            }
            else
            {
                int minutes = (int) duration.TotalMinutes;
                result = minutes > 0 ? $"{minutes}'" : "";
            }

            int seconds = (int) (duration.Seconds / 10d) * 10;
            result += seconds > 0 ? $"{seconds}\"" : "";
            if (count > 1)
            {
                result = $"{count} × {result}";
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = "-";
            }


            return result;
        }

        public static List<SessionSegmentModel> MergeSegment(IEnumerable<ISessionSegment> segments)
        {
            List<ISessionSegment> segmentsList = new(segments);
            List<SessionSegmentModel> results = new();
            int i = 0;
            while (i < segmentsList.Count)
            {
                int count = 1;
                if (segmentsList[i].Type != SessionSegmentType.Break)
                {
                    for (int j = i + 1; j < segmentsList.Count; j++)
                    {
                        if ((segmentsList[i].Type == segmentsList[j].Type) &&
                            ((int) segmentsList[i].Duration.TotalSeconds ==
                             (int) segmentsList[j].Duration.TotalSeconds))
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                results.Add(new SessionSegmentModel(segmentsList[i].Type, segmentsList[i].Duration, count,
                    segmentsList[i].ChangeImageAfterBreak));
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
                {
                    Duration = segment.Duration, Type = segment.Type,
                    ChangeImageAfterBreak = segment.ChangeImageAfterBreak
                }));
            }

            return results;
        }

        #endregion

        #endregion
    }
}