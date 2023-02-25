using System;
using System.Collections.Generic;
using System.IO;

namespace LMS.Extensions
{
    public static class DirectorySize
    {
        public static long GetCatalogsSize(DirectoryInfo Directory)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = Directory.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = Directory.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += GetCatalogsSize(di);
            }
            return size;
        }

        public static (double, string) AdjustFileSize(long fileSizeInBytes)//метод для подсчета размера файла из байт
        {
            string[] names = { "BYTES", "KB", "MB", "GB" };

            double sizeResult = fileSizeInBytes * 1.0;
            int nameIndex = 0;
            while (sizeResult > 1024 && nameIndex < names.Length)
            {
                sizeResult /= 1024;
                nameIndex++;
            }

            return (sizeResult, names[nameIndex]);
        }
    }
}