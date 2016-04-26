﻿using System.Collections;
using System.Linq;

namespace NFirmware
{
	/// <summary>
	/// Represents metadata of the image for LCD-display SSD1327.
	/// </summary>
	public class FirmwareImage2Metadata : FirmwareImageMetadata
	{
		/// <summary>
		/// Gets the length of the byte array that represents an image.
		/// </summary>
		public override long DataLength
		{
			get { return (Width + 7) / 8 * Height; }
		}

		/// <summary>
		/// Gets the block type.
		/// </summary>
		public override BlockType BlockType
		{
			get { return BlockType.Block2; }
		}

		/// <summary>
		/// Reads the image from bytes array and transforms to the two-dimensional bool array.
		/// </summary>
		/// <param name="imageBytes">Image bytes.</param>
		internal override bool[,] Load(byte[] imageBytes)
		{
			var result = new bool[Width, Height];
			var stride = (Width + 7) / 8;
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					var byteIndex = y * stride + x / 8;
					var bitIndex = 7 - x % 8;

					result[x, y] = (imageBytes[byteIndex] & (1 << bitIndex)) != 0;
				}
			}
			return result;
		}

		/// <summary>
		/// Transforms two-dimensional bool array to the bytes array.
		/// </summary>
		/// <param name="imageData">Image data.</param>
		internal override byte[] Save(bool[,] imageData)
		{
			var imageBytes = CreateImageDataWithHeader();
			var imageBytesCounter = HeaderLength;
			var colCounter = 0;

			for (var row = 0; row < Height; row++)
			{
				var byteBits = new bool[8];
				for (var col = 0; col < Width; col++)
				{
					byteBits[colCounter++] = imageData[col, row];
					if (colCounter == 8)
					{
						imageBytes[imageBytesCounter++] = new BitArray(byteBits.Reverse().ToArray()).ToByte();
						byteBits = new bool[8];
						colCounter = 0;
					}
				}

				if (colCounter > 0)
				{
					imageBytes[imageBytesCounter++] = new BitArray(byteBits.Reverse().ToArray()).ToByte();
					colCounter = 0;
				}
			}
			return imageBytes;
		}
	}
}
