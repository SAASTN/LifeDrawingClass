// *****************************************************************************
//  SessionDesigner.cs
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

namespace LifeDrawingClass.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LifeDrawingClass.Business.Interfaces;

    internal static class SessionSegmentsDesigner
    {
        #region Methods - Public

        #region Methods Stat

        public static List<ISessionSegment> DesignSessionSegments(ISessionProperties sessionProperties)
        {
            if (sessionProperties == null)
            {
                throw new ArgumentNullException(nameof(sessionProperties));
            }

            sessionProperties = FillSimplifiedProperties(sessionProperties);

            TimeSpan breaksDuration = sessionProperties.AddBreaks
                ? sessionProperties.NumberOfBreaks * sessionProperties.BreaksDuration
                : new TimeSpan();
            TimeSpan nonBreakDuration = sessionProperties.SessionDuration - breaksDuration;
            TimeSpan warmUpDuration = TimeSpan.FromMinutes(sessionProperties.AddWarmUp
                ? Math.Round((sessionProperties.WarmUpPercent * nonBreakDuration).TotalMinutes / 100.0d)
                : 0d);
            TimeSpan coolDownDuration = TimeSpan.FromMinutes(sessionProperties.AddCoolDown
                ? Math.Round((sessionProperties.CoolDownPercent * nonBreakDuration).TotalMinutes / 100.0d)
                : 0d);

            List<ISessionSegment> results = new();

            List<ISessionSegment> warmupSegments = null;
            if (warmUpDuration > TimeSpan.Zero)
            {
                warmupSegments = SplitSegmentByDuration(warmUpDuration, sessionProperties.AvailableWarmUpDurations,
                    SessionSegmentType.WarmUp);
                warmUpDuration = TimeSpan.FromTicks(warmupSegments.Sum(s => s.Duration.Ticks));
            }

            List<ISessionSegment> coolDownSegments = null;
            if (coolDownDuration > TimeSpan.Zero)
            {
                coolDownSegments = SplitSegmentByDuration(coolDownDuration,
                    sessionProperties.AvailableCoolDownDurations,
                    SessionSegmentType.CoolDown);
                coolDownDuration = TimeSpan.FromTicks(coolDownSegments.Sum(s => s.Duration.Ticks));
            }

            TimeSpan longPoseDuration = nonBreakDuration - warmUpDuration - coolDownDuration;
            List<ISessionSegment> longPoseSegments = SplitSegmentByCount(longPoseDuration,
                sessionProperties.NumberOfLongPoses, SessionSegmentType.LongPose);
            if (warmupSegments != null)
            {
                results.AddRange(warmupSegments.OrderBy(s => s.Duration));
            }

            results.AddRange(longPoseSegments);
            if (coolDownSegments != null)
            {
                results.AddRange(coolDownSegments.OrderByDescending(s => s.Duration));
            }

            if (breaksDuration > TimeSpan.Zero)
            {
                List<TimeSpan> startTimes = SessionSegment.GetStartTimes(results).Select(s => s).ToList();
                for (int i = 0; i < sessionProperties.NumberOfBreaks; i++)
                {
                    TimeSpan perfectTime = (i + 1) * (nonBreakDuration / (sessionProperties.NumberOfBreaks + 1));
                    int bestIndex = startTimes
                        .Select((start, ind) => new { ind, diff = (start - perfectTime).Duration() })
                        .OrderBy(p => p.ind).First().ind + i;
                    results.Insert(bestIndex,
                        new SessionSegment()
                        {
                            Duration = sessionProperties.BreaksDuration, GroupId = -1,
                            Type = SessionSegmentType.Break
                        });
                }
            }

            return results;
        }

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static List<ISessionSegment> SplitSegmentByCount(TimeSpan duration, int numberOfSegments,
            SessionSegmentType segmentType)
        {
            if (numberOfSegments > (int) duration.TotalMinutes)
            {
                // we are not going to create segments smaller than 1 minute
                numberOfSegments = (int) duration.TotalMinutes;
            }

            TimeSpan segmentDuration = TimeSpan.FromMinutes((int) (duration / numberOfSegments).TotalMinutes);
            if (segmentDuration.TotalMinutes > 20)
            {
                // when segments are long enough, we round them to 5 minutes
                segmentDuration = TimeSpan.FromMinutes(Math.Round(segmentDuration.TotalMinutes / 5.0d) * 5);
            }

            List<ISessionSegment> results = new(numberOfSegments);
            if (numberOfSegments > 1)
            {
                results.AddRange(
                    Enumerable.Range(0, numberOfSegments - 1).Select(_ => new SessionSegment()
                        { Duration = segmentDuration, GroupId = -1, Type = segmentType }));
            }

            if (numberOfSegments > 0)
            {
                results.Add(new SessionSegment()
                {
                    Duration = duration - (segmentDuration * (numberOfSegments - 1)),
                    GroupId = -1,
                    Type = segmentType
                });
            }

            return results;
        }

        /// <remarks>
        ///     Total duration of returned segments may be smaller than <paramref name="totalMinutes" />, but the difference is
        ///     always smaller than smallest duration in <paramref name="availableDurations" />.
        /// </remarks>
        private static List<ISessionSegment> SplitSegmentByDuration(TimeSpan totalMinutes,
            List<TimeSpan> availableDurations,
            SessionSegmentType segmentType)
        {
            List<ISessionSegment> results = new();
            availableDurations = availableDurations.Where(d => d <= totalMinutes).OrderByDescending(d => d).ToList();
            TimeSpan minSegmentDuration = availableDurations.Last();
            int i = 0;
            while (totalMinutes >= minSegmentDuration)
            {
                TimeSpan currentSegmentDuration = availableDurations[i];
                TimeSpan currentHalf = totalMinutes / (currentSegmentDuration == minSegmentDuration ? 1d : 2d);
                int numSegments = (int) Math.Ceiling(currentHalf / currentSegmentDuration);
                if (currentSegmentDuration * numSegments > totalMinutes)
                {
                    numSegments--;
                }

                if (numSegments > 0)
                {
                    results.AddRange(Enumerable.Range(0, numSegments).Select(_ => new SessionSegment()
                        { Duration = currentSegmentDuration, GroupId = -1, Type = segmentType }));
                    totalMinutes -= numSegments * currentSegmentDuration;
                }

                i++;
            }

            return results;
        }

        private static ISessionProperties FillSimplifiedProperties(ISessionProperties sessionProperties)
        {
            if (!sessionProperties.IsSimplified)
            {
                return sessionProperties;
            }

            if (sessionProperties.DesignType != SessionSegmentDesignType.Automatic)
            {
                throw new ArgumentException("This is a manually declared session.", nameof(sessionProperties));
            }

            List<TimeSpan> availableWarmUpDurations;
            List<TimeSpan> availableCoolDownDurations;
            TimeSpan warmUpDuration;
            TimeSpan coolDownDuration;
            if (sessionProperties.SessionDuration.TotalMinutes < 60)
            {
                warmUpDuration = TimeSpan.FromMinutes(10);
                coolDownDuration = TimeSpan.FromMinutes(5);
                availableWarmUpDurations = new List<TimeSpan>() { TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2) };
                availableCoolDownDurations = new List<TimeSpan>() { TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2) };
            }
            else if (sessionProperties.SessionDuration.TotalMinutes < 120)
            {
                warmUpDuration = TimeSpan.FromMinutes(30);
                coolDownDuration = TimeSpan.FromMinutes(15);
                availableWarmUpDurations = new List<TimeSpan>()
                    { TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5) };
                availableCoolDownDurations = new List<TimeSpan>()
                    { TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5) };
            }
            else
            {
                availableWarmUpDurations = new List<TimeSpan>()
                    { TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10) };
                availableCoolDownDurations = new List<TimeSpan>()
                    { TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10) };
                warmUpDuration = TimeSpan.FromMinutes(45);
                coolDownDuration = TimeSpan.FromMinutes(20);
            }

            TimeSpan breaksDuration = TimeSpan.FromMinutes(10);
            // a break each 1:30 hours, it is always greater or equal to 1, if the user does not need 
            // any breaks the they must uncheck AddBreaks
            int numberOfBreaks = (int) Math.Max(1.0, Math.Round(sessionProperties.BreaksDuration.TotalMinutes / 90.0));

            TimeSpan nonBreakDuration = sessionProperties.SessionDuration -
                                        (sessionProperties.AddBreaks ? breaksDuration * numberOfBreaks : TimeSpan.Zero);
            double warmUpPercent = 100.0d * (warmUpDuration / nonBreakDuration);
            double coolDownPercent = 100.0d * (coolDownDuration / nonBreakDuration);

            return new SessionProperties
            {
                DesignType = SessionSegmentDesignType.Automatic,
                SessionDuration = sessionProperties.SessionDuration,
                NumberOfLongPoses = sessionProperties.NumberOfLongPoses,
                AddWarmUp = sessionProperties.AddWarmUp,
                AddCoolDown = sessionProperties.AddCoolDown,
                AddBreaks = sessionProperties.AddBreaks,
                IsSimplified = false,
                ManualSegmentsDefinition = sessionProperties.ManualSegmentsDefinition,

                AvailableWarmUpDurations = availableWarmUpDurations,
                WarmUpPercent = warmUpPercent,
                AvailableCoolDownDurations = availableCoolDownDurations,
                CoolDownPercent = coolDownPercent,
                BreaksDuration = breaksDuration,
                NumberOfBreaks = numberOfBreaks
            };
        }

        #endregion

        #endregion
    }
}