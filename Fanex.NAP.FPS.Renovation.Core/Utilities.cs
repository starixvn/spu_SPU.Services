namespace Fanex.NAP.FPS.Renovation.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Linq;
    using System.IO;
    using System.Net.Sockets;
    using System.Numerics;
    using Fanex.NAP.FPS.Renovation.Model;
    using System.Collections;

    public static class Utilities
    {
        public static string GetURLFromDeviceCode(string di)
        {
            if (string.IsNullOrWhiteSpace(di))
            {
                return string.Empty;
            }

            var requestData = di.Split(new[] { "###" }, StringSplitOptions.None);

            var url = requestData.Length >= 8 ? requestData[7] : string.Empty;

            return url;
        }

        public static string GetUserAgentFromDeviceCode(string di)
        {
            if (string.IsNullOrWhiteSpace(di))
            {
                return string.Empty;
            }

            var deviceSplited = di.Split(new[] { "###" }, StringSplitOptions.None);

            return deviceSplited.Length > 3 ? deviceSplited[3] : string.Empty;
        }

        public static bool IsGuidDeviceId(string deviceCode)
        {
            Guid newGuid;

            return Guid.TryParse(deviceCode, out newGuid);
        }

        public static string GetDeviceCodeFromCapturedData(string di)
        {
            string[] splitingDI = di.Split(new[] { "###" }, StringSplitOptions.None);

            return splitingDI.Length >= 7 ? splitingDI[6] : string.Empty;
        }

        public static string XMLConvert(object data)
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };


            var serializer = new XmlSerializer(data.GetType(), new XmlRootAttribute("root"));

            using (var stringWriter = new System.IO.StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(writer, data, emptyNamespaces);

                    return stringWriter.ToString();
                }
            }
        }

        public static IEnumerable<IEnumerable<TValue>> Chunk<TValue>(
            this IEnumerable<TValue> values,
            Int32 chunkSize)
        {
            return values
                .Select((v, i) => new { v, groupIndex = i / chunkSize })
                .GroupBy(x => x.groupIndex)
                .Select(g => g.Select(x => x.v));
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            var numberOfPage = list.Count() / parts;
            var floatPoint = list.Count() % parts;
            if (floatPoint != 0)
            {
                parts += floatPoint;
            }

            for (var i = 0; i < parts; i++)
            {
                var takes = list.Skip(numberOfPage * i).Take(numberOfPage);

                yield return takes;
            }
        }

        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, Int32 size)
        {
            for (int i = 0; i < (source.Count / size) + (source.Count % size > 0 ? 1 : 0); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }

        public static List<T>[] Partition<T>(List<T> list, int totalPartitions)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (totalPartitions < 1)
                throw new ArgumentOutOfRangeException("totalPartitions");

            List<T>[] partitions = new List<T>[totalPartitions];

            int maxSize = (int)Math.Ceiling(list.Count / (double)totalPartitions);
            int k = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                partitions[i] = new List<T>();
                for (int j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                        break;
                    partitions[i].Add(list[j]);
                }
                k += maxSize;
            }

            return partitions;
        }

        public static void WriteLog(string filename, string content)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.AppendAllText(filePath, string.Format("{0}{1}", content, System.Environment.NewLine));
        }

        public static void DeleteLog(string fileName)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void WriteLogAllLine(string filename, string[] content)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.WriteAllLines(filePath, content);

        }

        public static IpModel Ip2Long(this string ipAddress)
        {
            var ipAd = IPAddress.Parse(ipAddress);

            BigInteger ipnum = 0;

            byte[] addrBytes = ipAd.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                var byteList = new List<byte>(addrBytes);
                byteList.Reverse();
                addrBytes = byteList.ToArray();
            }

            if (addrBytes.Length > 8)
            {
                //IPv6
                ipnum = BitConverter.ToUInt64(addrBytes, 8);
                ipnum <<= 64;
                ipnum += BitConverter.ToUInt64(addrBytes, 0);

                return new IpModel
                {
                    IpNumber = ipnum,
                    IsIpV6 = true
                };
            }
            else
            {
                //IPv4
                ipnum = BitConverter.ToUInt32(addrBytes, 0);

                return new IpModel
                {
                    IpNumber = ipnum,
                    IsIpV6 = false
                };
            }
        }

        public static string ToUncompressedString(this IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)  // IPv4
            {
                var strings = ipAddress.GetAddressBytes()
                    .Select(b => string.Format("{0:D3}", b));   // format bytes with padded 0s

                return string.Join(".", strings);   // join padded strings with '.' character
            }

            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)    // IPv6
            {

                var strings = Enumerable.Range(0, 8)    // create index
                                        .Select(i => ipAddress.GetAddressBytes().ToList().GetRange(i * 2, 2))       // get 8 chunks of bytes
                                        .Select(selector: i => { i.Reverse(); return i; })    // reverse bytes for endianness
                                        .Select(bytes => BitConverter.ToInt16(bytes.ToArray(), 0))  // convert bytes to 16 bit int
                                        .Select(int16 => string.Format("{0:X4}", int16).ToUpper()); // format int as a 4 digit hex 

                return string.Join(":", strings);   // join hex ints with ':'
            }

            return ipAddress.ToString();    // all else treat as to string
        }

        public static int ConvertBitArray2Int(BitArray bitArray)
        {
            var binString = ToBitString(bitArray);

            return Convert.ToInt32(binString, 2);
        }

        public static int[] ConvertInt2BitArray(int input)
        {
            string s = Convert.ToString(input, 2);

            return s.PadLeft(8, '0')
                         .Select(c => int.Parse(c.ToString()))
                         .ToArray();
        }

        public  static string ToBitString(BitArray bits)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bits.Count; i++)
            {
                char c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
