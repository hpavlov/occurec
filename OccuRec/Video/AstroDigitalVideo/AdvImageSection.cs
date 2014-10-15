/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace OccuRec.Video.AstroDigitalVideo
{
    public class AdvImageSection : IAdvDataSection
    {
		public enum ImageByteOrder
		{
			BigEndian,
			LittleEndian
		}

        public readonly uint Width;
        public readonly uint Height;
        public readonly byte BitsPerPixel;

    	public ImageByteOrder ByteOrder;

    	internal Dictionary<string, string> ImageSerializationProperties = new Dictionary<string, string>();
    	internal Dictionary<byte, AdvImageLayout> ImageLayouts = new Dictionary<byte, AdvImageLayout>();
		
        public AdvImageSection(ushort width, ushort height, byte bpp)
        {
            Width = width;
            Height = height;
            BitsPerPixel = bpp;
        }

        public string SectionType
        {
            get { return AdvSectionTypes.SECTION_IMAGE; }
        }        

        public AdvImageSection(BinaryReader reader)
        {
            byte version = reader.ReadByte();

            if (version >= 1)
            {
                Width = reader.ReadUInt32();
                Height = reader.ReadUInt32();
                BitsPerPixel = reader.ReadByte();

				byte lyCnt = reader.ReadByte();
				for (int i = 0; i < lyCnt; i++)
				{
					byte layoutId = reader.ReadByte();
					var layout = new AdvImageLayout(this, layoutId, Width, Height, reader);
					ImageLayouts.Add(layoutId, layout);
				}

            	byte propCnt = reader.ReadByte();
				for (int i = 0; i < propCnt; i++)
            	{
					string propName = reader.ReadAsciiString256();
					string propValue = reader.ReadAsciiString256();
					
            		ImageSerializationProperties.Add(propName, propValue);
            	}
				
				InitSerializationProperties();
            }
        }

		private void InitSerializationProperties()
		{
			ByteOrder = ImageByteOrder.LittleEndian;
			string propVal;
			if (ImageSerializationProperties.TryGetValue(AdvKeywords.KEY_IMAGE_BYTE_ORDER, out propVal))
			{
				if (propVal == "BIG-ENDIAN") ByteOrder = ImageByteOrder.BigEndian;
			}
		}

		public object GetDataFromDataBytes(byte[] bytes, ushort[,] prevImageData, int size, int startIndex)
		{
			byte layoutId = bytes[startIndex];
			byte byteMode = bytes[startIndex + 1];
			startIndex+=2;
			size-=2;

			AdvImageData rv = (AdvImageData)ImageLayouts[layoutId].GetDataFromDataBytes(bytes, prevImageData, (AdvImageLayout.GetByteMode)byteMode, size, startIndex);
			rv.LayoutId = layoutId;
			rv.ByteMode = (AdvImageLayout.GetByteMode)byteMode;
			rv.DataBlocksBytesCount = size;

			return rv;
		}

		public AdvImageLayout GetImageLayoutFromLayoutId(byte layoutId)
		{
			return ImageLayouts[layoutId];
		}

    	public enum GetPixelMode
		{
			Raw8Bit,
			StretchTo12Bit, 
			StretchTo16Bit,
		}        

        public static Bitmap ConstructBitmapFromBitmapPixels(byte[] pixels, int width, int height)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bmp.Width * 3;

                for (int y = 0; y < bmp.Height; ++y)
                {
                    for (int x = 0; x < bmp.Width; ++x)
                    {
                        byte color;
                        int index = x + y * width;
                        color = (byte)(pixels[index]);

                        p[0] = color;
                        p[1] = color;
                        p[2] = color;

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            bmp.UnlockBits(bmData);

            return bmp;
        }

        public Bitmap CreateBitmap(AdvImageData imageData)
        {
            byte[] pixels = new byte[Width * Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    pixels[x + y * Width] = (byte)(imageData.ImageData[x, y]);
                }
            }

            Bitmap displayBitmap = ConstructBitmapFromBitmapPixels(pixels, (int)Width, (int)Height);


            return displayBitmap;
        }
    }

    public class AdvImageData
    {
        public DateTime MidExposureUtc;
        public float ExposureMilliseconds;

        public ushort[,] ImageData;
    	public ushort Median;
    	public bool CRCOkay;

    	public byte Bpp;

    	public int LayoutId;
    	public AdvImageLayout.GetByteMode ByteMode;
    	public int DataBlocksBytesCount;
    }
}
