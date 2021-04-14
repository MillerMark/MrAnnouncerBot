﻿using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;

namespace Imaging
{
	public class ImageCropper
	{
		public int leftMargin = int.MaxValue;
		public int topMargin = int.MaxValue;
		public int rightMargin = int.MaxValue;
		public int bottomMargin = int.MaxValue;

		/// <summary>
		/// The minimum opacity for a pixel to consider it worthy of keeping (not cropping).
		/// If an entire horizontal or vertical line consists of pixels less than this opacity, 
		/// then that line will be cropped out.
		/// </summary>
		public int MinOpacityToKeep { get; set; } = 1;

		public string TargetFolder { get; set; }

		public event EventHandler ProgressChanged;
		public void OnProgressChanged()
		{
			ProgressChanged?.Invoke(null, EventArgs.Empty);
		}

		public void FindCropEdges(string fileName)
		{
			List<string> files = new List<string>();
			files.Add(fileName);
			FindCropEdges(files);
		}
		
		public void FindCropEdges(List<string> files)
		{
			leftMargin = int.MaxValue;
			topMargin = int.MaxValue;
			rightMargin = int.MaxValue;
			bottomMargin = int.MaxValue;

			int filesToProcess = files.Count;
			int counter = 0;
			ProgressScanPercent = 0;
			ProgressApplyPercent = 0;
			foreach (string file in files)
			{
				ActiveFile = file;
				GetCroppingMargins(file, out int left, out int top, out int right, out int bottom);

				ProgressScanPercent = counter * 100 / filesToProcess;
				OnProgressChanged();

				if (left < leftMargin)
					leftMargin = left;
				if (right < rightMargin)
					rightMargin = right;
				if (bottom < bottomMargin)
					bottomMargin = bottom;
				if (top < topMargin)
					topMargin = top;
				counter++;
			}
		}
		bool VerticalLineEmpty(DirectBitmap bitmap, int line)
		{
			for (int y = 0; y < bitmap.Height; y++)
				if (bitmap.GetPixel(line, y).A >= MinOpacityToKeep)
					return false;
			return true;
		}

		bool HorizontalLineEmpty(DirectBitmap bitmap, int line)
		{
			for (int x = 0; x < bitmap.Width; x++)
				if (bitmap.GetPixel(x, line).A >= MinOpacityToKeep)
					return false;
			return true;
		}

		void GetCroppingMargins(string file, out int left, out int top, out int right, out int bottom)
		{
			right = 0;
			bottom = 0;
			// TODO: Fix a bug when last frames are entirely empty. 
			using (DirectBitmap bitmap = DirectBitmap.FromFile(file))
			{
				int line = 0;
				int bitmapWidth = bitmap.Width;
				while (line < bitmapWidth && VerticalLineEmpty(bitmap, line))
				{
					line++;
				}
				left = line;
				if (line < bitmapWidth)
				{
					line = 0;
					int lineToCheck = bitmapWidth - line - 1;
					while (lineToCheck >= 0 && VerticalLineEmpty(bitmap, lineToCheck))
					{
						line++;
						lineToCheck = bitmapWidth - line - 1;
					}
					right = line;
				}

				line = 0;
				int bitmapHeight = bitmap.Height;
				while (line < bitmapHeight && HorizontalLineEmpty(bitmap, line))
				{
					line++;
				}
				top = line;
				if (line < bitmapHeight)
				{
					line = 0;
					int lineToCheck = bitmapHeight - line - 1;
					while (lineToCheck >= 0 && HorizontalLineEmpty(bitmap, lineToCheck))
					{
						line++;
						lineToCheck = bitmapHeight - line - 1;
					}
					bottom = line;
				}
				if (top == bitmapHeight && left == bitmapWidth)
				{
					// This image is entirely transparent - set margins to a point in the center of the image.
					top = bitmapHeight / 2;
					bottom = top;
					left = bitmapWidth / 2;
					right = left;
				}
			}
			
		}
		void CropFile(string file, int leftMargin, int topMargin, int rightMargin, int bottomMargin, string targetFileName = null)
		{
			string workFile = file;
			bool workFileIsTemporary = false;

			if (string.IsNullOrEmpty(TargetFolder))  // Replacing this file.
			{
				workFileIsTemporary = true;
				string tempFileName = Path.GetTempFileName();
				File.Delete(tempFileName);
				File.Copy(file, tempFileName);
				File.Delete(file);
				workFile = tempFileName;
			}

			using (Bitmap image = new Bitmap(workFile))
			{
				int width = image.Width - rightMargin - leftMargin;
				int height = image.Height - bottomMargin - topMargin;
				if (width <= 0)
					width = 1;
				if (height <= 0)
					height = 1;
				using (Bitmap cropped = image.Clone(new Rectangle(leftMargin, topMargin, width, height), PixelFormat.Format32bppArgb))
				{
					if (targetFileName == null)
						targetFileName = Path.GetFileName(file);
					string path = Path.GetDirectoryName(file);
					string targetPath;
					if (!string.IsNullOrEmpty(TargetFolder))
					{
						targetPath = TargetFolder;
						if (!Directory.Exists(targetPath))
							Directory.CreateDirectory(targetPath);
					}
					else  // Replacing the file...
					{
						targetPath = path;
					}

					cropped.Save(Path.Combine(targetPath, targetFileName));
				}
			}
			if (workFileIsTemporary)
				File.Delete(workFile);
		}

		public void ApplyCrop(string fileName, string baseName = null, bool useIndices = false)
		{
			List<string> files = new List<string>();
			files.Add(fileName);
			ApplyCrop(files, baseName, useIndices);
		}

		public void ApplyCrop(List<string> files, string baseName = null, bool useIndices = false)
		{
			ProgressApplyPercent = 0;
			int filesToProcess = files.Count;
			int counter = 0;
			foreach (string file in files)
			{
				string targetFileName = null;
				if (baseName != null)
					if (useIndices)
						targetFileName = $"{baseName}{counter}.png";
					else
						targetFileName = $"{baseName}.png";

				ActiveFile = file;
				ProgressScanPercent = counter * 100 / filesToProcess;
				counter++;
				OnProgressChanged();
				
				CropFile(file, leftMargin, topMargin, rightMargin, bottomMargin, targetFileName);
			}
			ProgressApplyPercent = 100;
			OnProgressChanged();
		}

		public int ProgressScanPercent { get; set; }
		public int ProgressApplyPercent { get; set; }
		public string ActiveFile { get; set; }

		public Margins GetMargins()
		{
			Margins result = new Margins();
			result.Left = leftMargin;
			result.Top = topMargin;
			result.Right = rightMargin;
			result.Bottom = bottomMargin;
			return result;
		}
	}
}
