using System.Security.Cryptography;
using System.Linq;
using UnityEngine;

namespace LazyBot.Utility.Data
{
    /// <summary>
    /// Represents current data state.
    /// </summary>
    public enum DataState
    {
        Unknown,
        Updated,
        Transferred,
        Processed,
        Changed,
        Deleted
    }

    /// <summary>
    /// Performs md5 hashing of string.
    /// </summary>
    public static class Hasher
    {
        /// <summary>
        /// Time reference point.
        /// </summary>
        private static readonly System.DateTime Jan1st1970 = new System.DateTime
            (1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        /// <summary>
        /// Generates md5 hash based on current time + guid object.
        /// </summary>
        /// <returns>String containing md5 hash.</returns>
        public static string GenerateHash()
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(
                System.Text.Encoding.Default.GetBytes(GenerateString())
            );

            return System.BitConverter.ToString(data);
        }

        /// <summary>
        /// Generates string based on guid object + current time in milliseconds.
        /// </summary>
        /// <returns>String containing random number.</returns>
        public static string GenerateString()
        {
            return System.Guid.NewGuid().ToString() + CurrentTimeMillis();
        }

        /// <summary>
        /// Gets current time in milliseconds.
        /// </summary>
        /// <returns>Current time in milliseconds.</returns>
        public static long CurrentTimeMillis()
        {
            return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        /// <summary>
        /// Generate md5 hash as byte array.
        /// </summary>
        /// <returns>Byte array, represents md5 hash.</returns>
        public static byte[] GenerateHashArray()
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(
                System.Text.Encoding.Default.GetBytes(GenerateString())
            );

            return data;
        }

        /// <summary>
        /// Generates md5 hash based on input string.
        /// </summary>        
        /// <param name="input">String, used as buffer.</param>
        /// <returns>String containing md5 hash.</returns>
        public static string GenerateHash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(
                System.Text.Encoding.Default.GetBytes(input)
            );

            return System.BitConverter.ToString(data);
        }

        /// <summary>
        /// Generates md5 hash as byte array, based on input string.
        /// </summary>        
        /// <param name="input">String, used as buffer.</param>
        /// <returns>Byte array, represents md5 hash.</returns>
        public static byte[] GenerateHashArray(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(
                System.Text.Encoding.Default.GetBytes(input)
            );

            return data;
        }

    }

    /// <summary>
    /// Integer extension class.
    /// </summary>
    public static class IntHelper
    {
        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// </summary>
        /// <param name="bas">Number to be raised to a power.</param>
        /// <param name="exp">Number that specifies a power.</param>
        /// <returns>Raised number.</returns>
        public static int Pow(this int bas, int exp)
        {
            return Enumerable
                  .Repeat(bas, exp)
                  .Aggregate(1, (a, b) => a * b);
        }
    }

    /// <summary>
    /// Float extension class.
    /// </summary>
    public static class FloatHelper
    {
        /// <summary>
        /// Maps float value from interval (istart, istop) to (ostart, ostop).
        /// </summary>
        /// <param name="value">Value to be mapped.</param>
        /// <param name="istart">Original min value.</param>
        /// <param name="istop">Original max value.</param>
        /// <param name="ostart">Relative min value.</param>
        /// <param name="ostop">Relative max value.</param>
        /// <returns>Mapped float value.</returns>
        public static float Map(this float value, float istart, float istop, float ostart, float ostop)
        {
            return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
        }

        /// <summary>
        /// Maps Vector3 value from interval (istart, istop) to (ostart, ostop).
        /// </summary>
        /// <param name="value">Value to be mapped.</param>
        /// <param name="istart">Original min value.</param>
        /// <param name="istop">Original max value.</param>
        /// <param name="ostart">Relative min value.</param>
        /// <param name="ostop">Relative max value.</param>
        /// <returns>Mapped Vector3 value.</returns>
        public static Vector3 Map(this float value, Vector3 istart, Vector3 istop, Vector3 ostart, Vector3 ostop)
        {
            return new Vector3(
                value.Map(istart.x, istop.x, ostart.x, ostop.x), 
                value.Map(istart.y, istop.y, ostart.y, ostop.y), 
                value.Map(istart.z, istop.z, ostart.z, ostop.z));
        }

        /// <summary>
        /// Returns direction based on (y) angle.
        /// </summary>
        /// <param name="angleInDegrees">Angle in degrees.</param>
        /// <param name="euler">Euler angle.</param>
        /// <param name="rotation">Rotation value.</param>
        /// <param name="angleIsGlobal">Is angle global.</param>
        /// <returns></returns>
        public static Vector3 DirFromAngleX(this float angleInDegrees, float euler = 0, float rotation = 0, bool angleIsGlobal = false)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += euler + rotation;
            }
            return new Vector3(0, Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        /// <summary>
        /// Returns direction based on (y) angle.
        /// </summary>
        /// <param name="angleInDegrees">Angle in degrees.</param>
        /// <param name="euler">Euler angle.</param>
        /// <param name="rotation">Rotation value.</param>
        /// <param name="angleIsGlobal">Is angle global.</param>
        /// <returns></returns>
        public static Vector3 DirFromAngleY(this float angleInDegrees, float euler = 0, float rotation = 0, bool angleIsGlobal = false)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += euler + rotation;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        /// <summary>
        /// Returns direction based on (z) angle.
        /// </summary>
        /// <param name="angleInDegrees">Angle in degrees.</param>
        /// <param name="euler">Euler angle.</param>
        /// <param name="rotation">Rotation value.</param>
        /// <param name="angleIsGlobal">Is angle global.</param>
        /// <returns></returns>
        public static Vector3 DirFromAngleZ(this float angleInDegrees, float euler = 0, float rotation = 0, bool angleIsGlobal = false)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += euler + rotation;
            }
            return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
        }
    }

    /// <summary>
    /// Vector3 extension class.
    /// </summary>
    public static class VectorHelper
    {
        /// <summary>
        /// Maps Vector3 value from interval (istart, istop) to (ostart, ostop).
        /// </summary>
        /// <param name="value">Value to be mapped.</param>
        /// <param name="istart">Original min value.</param>
        /// <param name="istop">Original max value.</param>
        /// <param name="ostart">Relative min value.</param>
        /// <param name="ostop">Relative max value.</param>
        /// <returns>Mapped Vector3 value.</returns>
        public static Vector3 Map(this Vector3 value, Vector3 istart, Vector3 istop, Vector3 ostart, Vector3 ostop)
        {
            return new Vector3(
                value.x.Map(istart.x, istop.x, ostart.x, ostop.x),
                value.y.Map(istart.y, istop.y, ostart.y, ostop.y),
                value.z.Map(istart.z, istop.z, ostart.z, ostop.z));
        }        
    }    
}
