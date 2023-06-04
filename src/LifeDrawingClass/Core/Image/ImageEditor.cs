// *****************************************************************************
//  ImageEditor.cs
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

namespace LifeDrawingClass.Core.Image
{
    using System;
    using System.IO;
    using LifeDrawingClass.Core.Log;
    using SkiaSharp;

    internal class ImageEditor
    {
        #region Methods - Public

        #region Methods Stat

        public static SKBitmap ImportImageFromFile(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                MemoryStream ms = new(bytes);
                using SKManagedStream stream = new(ms);
                SKBitmap bitmap = SKBitmap.Decode(stream);
                return bitmap;
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to load image from file: \"{path}\"", e);
                return null;
            }
        }

        public static void PaintCanvas(SKCanvas canvas, SKBitmap bitmap, int width, int height)
        {
            canvas.Clear(SKColors.Transparent);

            if (bitmap != null)
            {
                SKRectI bmpRect = SKRectI.Create(width, height).AspectFit(bitmap.Info.Size);

                using SKPaint paint = new();

                canvas.DrawBitmap(bitmap, bmpRect);
            }
        }

        #endregion

        #endregion
    }
}