// *****************************************************************************
//  SessionDefinitionParser.cs
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
    using System.ComponentModel;
    using System.Linq;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Models;

    /// <example>
    ///     [Nx]D[.5][W|L|C|B[!]]
    ///     5x2W        : 5 warm-up segments, each for 2 minutes
    ///     5x2.5C      : 5 cool-down segments, each for 2:30 minutes
    ///     40          : a 40 minute long pose segment, it is long pose as its type is not explicitly declared
    ///     40L         : a 40 minute long pose segment
    ///     10B         : a 10 min break
    ///     10B!        : a 10 min break in the middle of a segment, meaning the slide won't change after the break
    ///     20,10B!,24  : 2 long poses with a break in between, but same image will be displayed in both segments
    ///     So, the <see cref="ValidChars">acceptable characters</see>  are: 0 to 9 x . W L C B ! and ,
    /// </example>
    public static class SessionDefinitionParser
    {
        #region Constants & Statics

        private static readonly char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private static readonly char[] NonDigitValidChars =
            { 'X', '.', 'W', 'L', 'C', 'B', '!' }; // it will be controlled after the string was split by ','

        private static readonly char[] ValidChars = Digits.Concat(NonDigitValidChars).ToArray();

        private static readonly Dictionary<string, SessionSegmentType> TypeMap =
            new()
            {
                { "W", SessionSegmentType.WarmUp },
                { "L", SessionSegmentType.LongPose },
                { "", SessionSegmentType.LongPose },
                { "C", SessionSegmentType.CoolDown },
                { "B", SessionSegmentType.Break },
                { "B!", SessionSegmentType.Break }
            };

        #endregion

        #region Methods - Public

        #region Methods Stat

        public static bool AreValidChars(string text) => text.All(IsValidChar);

        public static List<ISessionSegment> Parse(string text, out List<ParserMessageItem> messages)
        {
            List<ISessionSegment> results = new();
            messages = new List<ParserMessageItem>();
            if (string.IsNullOrEmpty(text))
            {
                return results;
            }

            string[] parts = text.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                ParseSegment(part, results, messages, i);
            }

            return results;
        }


        /// <summary>
        ///     source: https://stackoverflow.com/a/20175/2093077
        /// </summary>
        public static string AddOrdinal(int num)
        {
            if (num <= 0)
            {
                return num.ToString();
            }

            return (num % 100) switch
            {
                11 => num + "th",
                12 => num + "th",
                13 => num + "th",
                _ => (num % 10) switch
                {
                    1 => num + "st",
                    2 => num + "nd",
                    3 => num + "rd",
                    _ => num + "th"
                }
            };
        }

        public static string GetDefinitionTextAutomaticSession(IEnumerable<SessionSegmentModel> segments) =>
            string.Join(",", segments.Select(GetDefinitionTextOfSegment));

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static bool IsValidChar(char c) => ValidChars.Contains(char.ToUpperInvariant(c)) || (c == ',');

        private static void ParseSegment(in string segmentStr, List<ISessionSegment> results,
            List<ParserMessageItem> messages,
            int index)
        {
            string segmentStrCopy = segmentStr;
            if (string.IsNullOrEmpty(segmentStrCopy))
            {
                messages.Add(new ParserMessageItem(segmentStr, index, "Empty section. Remove consecutive commas."));
                return;
            }

            if (!CheckValidity(segmentStrCopy, out List<char> invalidChars))
            {
                // well, this must not happen! normally, the ui should prevent it, but the mad-user
                // still can edit the config file
                messages.Add(new ParserMessageItem(segmentStr, index,
                    $"Section contains invalid characters. Remove the these: \"{string.Join("", invalidChars)}\""));
                return;
            }

            if (!CheckCharacterRepetition(segmentStrCopy, out List<char> repeatedChars))
            {
                messages.Add(new ParserMessageItem(segmentStr, index,
                    $"The following characters can only appear once in each section : \"{string.Join("", repeatedChars)}\""));
                return;
            }

            segmentStrCopy = segmentStrCopy.ToUpperInvariant();
            // ReSharper disable once RedundantExplicitParamsArrayCreation
            string[] splitByX = segmentStrCopy.Split(new[] { 'x', 'X' });
            int count = 1;
            if (splitByX.Length > 1)
            {
                if (!int.TryParse(splitByX[0], out count))
                {
                    messages.Add(new ParserMessageItem(segmentStr, index,
                        $"The part before the 'x' is not a valid integer: \"{splitByX[0]}\""));
                    return;
                }

                if (count < 1)
                {
                    messages.Add(new ParserMessageItem(segmentStr, index,
                        $"The part before the 'x' must be greater than 0: \"{splitByX[0]}\""));
                    count = 1;
                }

                segmentStrCopy = splitByX[1];
            }

            //string durationStr = segmentStrCopy.TrimEnd(NonDigitValidChars);
            string durationStr = new(segmentStrCopy.TakeWhile(c => char.IsDigit(c) || (c == '.')).ToArray());
            if (!double.TryParse(durationStr, out double durationMinutes))
            {
                messages.Add(new ParserMessageItem(segmentStr, index,
                    $"The part after the 'x' must be a valid number: \"{durationStr}\""));
                return;
            }

            TimeSpan duration = TimeSpan.FromMinutes(durationMinutes);
            string typeStr = segmentStrCopy[durationStr.Length..];
            if (!TypeMap.ContainsKey(typeStr))
            {
                messages.Add(new ParserMessageItem(segmentStr, index,
                    $"Unknown segment type: \"{typeStr}\". Use 'W', 'L', 'C', 'B', or 'B!'."));
                return;
            }

            SessionSegmentType type = TypeMap[typeStr];
            bool changeImageAfterBreak = typeStr != "b!";
            results.AddRange(Enumerable.Repeat(
                new SessionSegment()
                    { Duration = duration, Type = type, ChangeImageAfterBreak = changeImageAfterBreak }, count));
        }

        private static bool CheckCharacterRepetition(string segmentStr, out List<char> repeatedChars)
        {
            repeatedChars = segmentStr.Where(c => NonDigitValidChars.Contains(char.ToUpperInvariant(c))).GroupBy(c => c)
                .Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            return !repeatedChars.Any();
        }

        private static bool CheckValidity(string segmentStr, out List<char> invalidChars)
        {
            invalidChars = segmentStr.Where(c => !ValidChars.Contains(char.ToUpperInvariant(c))).ToList();
            return !invalidChars.Any();
        }


        private static string GetDefinitionTextOfSegment(SessionSegmentModel segment)
        {
            string countStr = segment.Count > 1 ? $"{segment.Count}x" : "";
            string typeStr = segment.Type switch
            {
                SessionSegmentType.WarmUp => "W",
                SessionSegmentType.LongPose => "",
                SessionSegmentType.CoolDown => "C",
                SessionSegmentType.Break => "B" + (segment.ChangeImageAfterBreak ? "" : "!"),
                _ => throw new InvalidEnumArgumentException()
            };

            return $"{countStr}{segment.Duration.TotalMinutes:0.#}{typeStr}";
        }

        #endregion

        #endregion

        #region Nested Types

        public class ParserMessageItem
        {
            #region Constructors

            public ParserMessageItem(string section, int sectionIndex, string message)
            {
                this.Section = section;
                this.SectionIndex = sectionIndex;
                this.Message = message;
            }

            #endregion

            #region Properties & Fields - Public

            public int SectionIndex { get; }
            public string Section { get; }
            public string Message { get; }

            #endregion

            #region Methods - Public

            #region Methods Impl

            /// <inheritdoc />
            public override string ToString() =>
                (this.SectionIndex > -1
                    ? $"{AddOrdinal(this.SectionIndex + 1)} part ({this.Section}): "
                    : "") + this.Message;

            #endregion

            #endregion
        }

        #endregion
    }
}