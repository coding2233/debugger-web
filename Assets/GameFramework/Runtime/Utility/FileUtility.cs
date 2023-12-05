using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Wanderer
{
    public static class FileUtility
    {
        //public static string Encrypt(string path)
        //{
        //    string fileName = Path.GetFileName(path);
        //    byte crcKey = CRC8(fileName);
        //    string targetPath = Path.Combine(Application.temporaryCachePath, fileName);
        //    byte[] data = File.ReadAllBytes(path);
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        data[i] ^= crcKey;
        //    }
        //    File.WriteAllBytes(targetPath, data);
        //    return targetPath;
        //}


        //public static string Decode(string path)
        //{
        //    string fileName = Path.GetFileName(path);
        //    byte crcKey = CRC8(fileName);
        //    //这个文件使用后建议直接删掉
        //    string targetPath = Path.Combine(Application.temporaryCachePath, $"{Guid.NewGuid().ToString()}{Path.GetExtension(path)}");
        //    byte[] data = File.ReadAllBytes(path);
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        data[i] ^= crcKey;
        //    }
        //    File.WriteAllBytes(targetPath, data);
        //    return targetPath;
        //}

        //public static byte CRC8(this string path)
        //{
        //    string saltFileName = Path.Combine(path, Application.companyName, Application.productName);
        //    byte[] buff = System.Text.Encoding.UTF8.GetBytes(saltFileName);
        //    byte crc = 0;
        //    for (int i = 0; i < buff.Length; i++)
        //    {
        //        crc ^= buff[i];
        //        for (int j = 0; j < 8; j++)
        //        {
        //            if ((crc & 0x80) > 0)
        //                crc = (byte)((crc << 1) ^ 0x07);
        //            else
        //                crc = (byte)(crc << 1);
        //        }
        //    }
        //    crc = (byte)(crc ^ 0x55);
        //      //for (int i = 0; i < buff.Length; i++)
        //      //{
        //      //    crc ^= buff[i];
        //      //    for (int j = 0; j < 8; j++)
        //      //    {
        //      //        if ((crc & 0x01) != 0)
        //      //        {
        //      //            crc >>= 1;
        //      //            crc ^= 0x8c;
        //      //        }
        //      //        else
        //      //        {
        //      //            crc >>= 1;
        //      //        }
        //      //    }
        //      //}
        //      //if (crc == 0)
        //      //{
        //      //    crc = 1;
        //      //}

        //  Debug.Log($"[CRC8] {path} {saltFileName} {crc}");
        //    return crc;
        //}

        public static byte HashKey(this string name)
        {
            //string saltFileName = Path.Combine(name, Application.companyName).Replace("\\","/");
            string saltFileName = name.Replace("\\", "/");
            int hash = 0;
            int length = saltFileName.Length;
            for (int i = 0; i < length; ++i)
            {
                hash += saltFileName[i] * 79;
            }

            byte key = (byte)((hash % 1415) & 0xff);
            if (key == 0)
            {
                key = 64;
            }

            return key; // 1413
        }

        public static string GetFileMD5(string path)
        {
            if (File.Exists(path))
            {
                string md5 = null;
                using (var stream = File.OpenRead(path))
                {
                    using (MD5 md5Hash = MD5.Create())
                    {
                       var md5Data = md5Hash.ComputeHash(stream);
                        StringBuilder sBuilder = new StringBuilder();

                        for (int i = 0; i < md5Data.Length; i++)
                        {
                            sBuilder.Append(md5Data[i].ToString("x2"));
                        }
                        md5 = sBuilder.ToString();
                    }
                }

                return md5;
            }
            return null;
        }

        public static long GetFileSize(string path)
        {
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    return stream.Length;
                }
            }
            return 0;
        }


    }

    /// <summary>
    /// 先做个^的简单加密
    /// </summary>
    public class EncryptFileStream : FileStream
    {
        private byte _crcKey;
        public EncryptFileStream(string path, FileMode mode) : base(path, mode)
        {
            _crcKey = Path.GetFileName(path).HashKey();
        }
        public EncryptFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
        {
            _crcKey = Path.GetFileName(path).HashKey();
        }

        public override int Read(byte[] array, int offset, int count)
        {
            int index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= _crcKey;
            }
            return index;
        }

        public override int Read(Span<byte> buffer)
        {
            return base.Read(buffer);
        }

        //public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        //{
        //    var task = base.ReadAsync(buffer, offset, count, cancellationToken);

        //    for (int i = 0; i < task.Result; i++)
        //    {
        //        buffer[i] ^= _crcKey;
        //    }

        //    return task;
        //}

        public override void Write(byte[] array, int offset, int count)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= _crcKey;
            }
            base.Write(array, offset, count);
        }
    }

    public class EncryptMemoryStream : MemoryStream
    {
        private byte _crcKey;
        public EncryptMemoryStream(byte[] buffer, byte crcKey) : base(buffer)
        {
            _crcKey = crcKey;
        }

        public EncryptMemoryStream(byte[] buffer, string filePath) : base(buffer)
        {
            _crcKey = Path.GetFileName(filePath).HashKey();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int index = base.Read(buffer, offset, count);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= _crcKey;
            }
            return index;
            // return base.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= _crcKey;
            }
            base.Write(buffer, offset, count);
        }

        //public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        //{
        //    var task = base.ReadAsync(buffer, offset, count, cancellationToken);

        //    for (int i = 0; i < task.Result; i++)
        //    {
        //        buffer[i] ^= _crcKey;
        //    }

        //    return task;
        //}


    }


}