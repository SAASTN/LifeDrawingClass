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

            int breaksDuration = sessionProperties.AddBreaks
                ? sessionProperties.NumberOfBreaks * sessionProperties.BreaksDuration
                : 0;
            int nonBreakDuration = sessionProperties.SessionDuration - breaksDuration;
            int warmUpDuration = sessionProperties.AddWarmUp
                ? (int) Math.Round((sessionProperties.WarmUpPercent * nonBreakDuration) / 100.0d)
                : 0;
            int coolDownDuration = sessionProperties.AddCoolDown
                ? (int) Math.Round((sessionProperties.CoolDownPercent * nonBreakDuration) / 100.0d)
                : 0;

            List<ISessionSegment> results = new();

            List<ISessionSegment> warmupSegments = null;
            if (warmUpDuration > 0)
            {
                warmupSegments = SplitSegmentByDuration(warmUpDuration, sessionProperties.AvailableWarmUpDurations,
                    SessionSegmentType.WarmUp);
                warmUpDuration = warmupSegments.Sum(s => s.DurationMilliseconds / 60000);
            }

            List<ISessionSegment> coolDownSegments = null;
            if (coolDownDuration > 0)
            {
                coolDownSegments = SplitSegmentByDuration(coolDownDuration,
                    sessionProperties.AvailableCoolDownDurations,
                    SessionSegmentType.CoolDown);
                coolDownDuration = coolDownSegments.Sum(s => s.DurationMilliseconds / 60000);
            }

            int longPoseDuration = nonBreakDuration - warmUpDuration - coolDownDuration;
            List<ISessionSegment> longPoseSegments = SplitSegmentByCount(longPoseDuration,
                sessionProperties.NumberOfLongPoses, SessionSegmentType.LongPose);
            if (warmupSegments != null)
            {
                results.AddRange(warmupSegments.OrderBy(s => s.DurationMilliseconds));
            }

            results.AddRange(longPoseSegments);
            if (coolDownSegments != null)
            {
                results.AddRange(coolDownSegments.OrderByDescending(s => s.DurationMilliseconds));
            }

            if (breaksDuration > 0)
            {
                List<int> startTimes = SessionSegment.GetStartTimes(results).Select(s => s / 60000).ToList();
                for (int i = 0; i < sessionProperties.NumberOfBreaks; i++)
                {
                    double perfectTime = (i + 1) * (nonBreakDuration / (sessionProperties.NumberOfBreaks + 1));
                    int bestIndex = startTimes.Select((ind, start) => new { ind, diff = Math.Abs(start - perfectTime) })
                        .OrderBy(p => p.ind).First().ind + i;
                    results.Insert(bestIndex,
                        new SessionSegment()
                        {
                            DurationMilliseconds = sessionProperties.BreaksDuration * 60000, GroupId = -1,
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

        private static List<ISessionSegment> SplitSegmentByCount(int totalMinutes, int numberOfSegments,
            SessionSegmentType segmentType)
        {
            if (numberOfSegments > totalMinutes)
            {
                // we are not going to create segments smaller than 1 minute
                numberOfSegments = totalMinutes;
            }

            int segmentDuration = (int) Math.Floor((double) totalMinutes / numberOfSegments);
            if (segmentDuration > 20)
            {
                // when segments are long enough, we round them to 5 minutes
                segmentDuration = (int) Math.Round(segmentDuration / 5.0d) * 5;
            }

            List<ISessionSegment> results = new(numberOfSegments);
            if (numberOfSegments > 1)
            {
                results.AddRange(
                    Enumerable.Range(0, numberOfSegments - 1).Select(_ => new SessionSegment()
                        { DurationMilliseconds = segmentDuration * 60000, GroupId = -1, Type = segmentType }));
            }

            if (numberOfSegments > 0)
            {
                results.Add(new SessionSegment()
                {
                    DurationMilliseconds = (totalMinutes - (segmentDuration * (numberOfSegments - 1))) * 60000,
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
        private static List<ISessionSegment> SplitSegmentByDuration(int totalMinutes, List<int> availableDurations,
            SessionSegmentType segmentType)
        {
            List<ISessionSegment> results = new();
            availableDurations = availableDurations.Where(d => d <= totalMinutes).OrderByDescending(d => d).ToList();
            int minSegmentDuration = availableDurations.Last();
            int i = 0;
            while (totalMinutes >= minSegmentDuration)
            {
                int currentSegmentDuration = availableDurations[i];
                double currentHalf = totalMinutes / (currentSegmentDuration == minSegmentDuration ? 1d : 2d);
                int numSegments = (int) Math.Ceiling(currentHalf / currentSegmentDuration);
                if (currentSegmentDuration * numSegments > totalMinutes)
                {
                    numSegments--;
                }

                if (numSegments > 0)
                {
                    results.AddRange(Enumerable.Range(0, numSegments).Select(_ => new SessionSegment()
                        { DurationMilliseconds = currentSegmentDuration * 60000, GroupId = -1, Type = segmentType }));
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

            List<int> availableWarmUpDurations;
            List<int> availableCoolDownDurations;
            int warmUpDuration;
            int coolDownDuration;
            if (sessionProperties.SessionDuration < 60)
            {
                warmUpDuration = 10;
                coolDownDuration = 5;
                availableWarmUpDurations = new List<int>() { 1, 2 };
                availableCoolDownDurations = new List<int>() { 1, 2 };
            }
            else if (sessionProperties.SessionDuration < 120)
            {
                warmUpDuration = 30;
                coolDownDuration = 15;
                availableWarmUpDurations = new List<int>() { 1, 2, 5 };
                availableCoolDownDurations = new List<int>() { 1, 2, 5 };
            }
            else
            {
                availableWarmUpDurations = new List<int>() { 1, 5, 10 };
                availableCoolDownDurations = new List<int>() { 1, 5, 10 };
                warmUpDuration = 45;
                coolDownDuration = 20;
            }

            int breaksDuration = 10; // minutes
            // a break each 1:30 hours, it is always greater or equal to 1, if the user does not need 
            // any breaks the they must uncheck AddBreaks
            int numberOfBreaks = (int) Math.Max(1.0, Math.Round(sessionProperties.BreaksDuration / 90.0));

            double nonBreakDuration = sessionProperties.SessionDuration -
                                      (sessionProperties.AddBreaks ? breaksDuration * numberOfBreaks : 0.0);
            double warmUpPercent = (100.0d * warmUpDuration) / nonBreakDuration;
            double coolDownPercent = (100.0d * coolDownDuration) / nonBreakDuration;

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