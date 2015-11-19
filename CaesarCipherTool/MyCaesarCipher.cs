using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CaesarCipherTool
{
    /// <summary>
    /// My Caesar Cipher Algorithm Implementation with Encrypt Method and Automatic Decrypt Method using 
    /// By Mahmoud Youssef Mahmoud 
    /// </summary>
    public static class MyCaesarCipher
    {
        /// <summary>
        /// Initialize English letters and its frequencies distribution 
        /// </summary>
        private static readonly Dictionary<char, double> EngFrequencyDic = EnglishFrequencyDictionary();
        /// <summary>
        /// Number Of English Letters
        /// </summary>
        private const int NumberOfLetters = 26;

        /// <summary>
        /// Encrypt Plain text using Caesar Cipher Algorithm by shifting letters with given key 
        /// </summary>
        /// <param name="plainText"> Plain Text</param>
        /// <param name="key">key</param>
        /// <returns>Cipher Text </returns>
        public static string Encrypt(string plainText, int key)
        {
            try
            {
                var dic = EnglishLetterDictionary();
                var text = "";

                for (var i = 0; i < plainText.Length; i++)
                {
                    if (!dic.Any(k => k.Key.Equals(plainText[i])))
                    {
                        text += plainText[i];
                        continue;
                    }
                    var tempVal = (dic.FirstOrDefault(k => k.Key.Equals(plainText[i])).Value + key) % 26;
                    text += dic.FirstOrDefault(v => v.Value == tempVal).Key;
                }
                return text;
            }
            catch (Exception e)
            {
                return "Error: --> " + e.Message;
            }
        }
        /// <summary>
        /// Cracking cipher text by Caesar Cipher Algorithm using Correlation Coefficient Method by
        /// calculation message letter frequency distribution
        /// calculate Correlation between message letter frequency List and English letter frequency distribution
        /// shift message frequency list by one step and calculate Correlation again 
        /// repeat it for 26 time number of English letters
        /// find max Correlation Coefficient
        /// get index of max Correlation Coefficient this is the KEY
        /// use the KEY to Decrypt cipher message 
        /// </summary>
        /// <param name="cipherText">cipher message</param>
        /// <param name="key"> detected KEY </param>
        /// <param name="plainText">detected plain message</param>
        public static void AutoDecrypt(string cipherText, out int key, out string plainText)
        {
            try
            {
                key = DetectKey(cipherText);
                plainText = Decrypt(cipherText, key);
            }
            catch (Exception e)
            {
                key = 0;
                plainText = "Error: --> " + e.Message;
            }
        }
        /// <summary>
        /// shift message letter by given key in reverse direction 
        /// </summary>
        /// <param name="cipherText">cipherText</param>
        /// <param name="key">key</param>
        /// <returns>plain message</returns>
        private static string Decrypt(string cipherText, int key)
        {
            try
            {
                var dic = EnglishLetterDictionary();
                var text = "";

                for (var i = 0; i < cipherText.Length; i++)
                {
                    if (!dic.Any(k => k.Key.Equals(cipherText[i])))
                    {
                        text += cipherText[i];
                        continue;
                    }
                    var tempVal = dic.FirstOrDefault(k => k.Key.Equals(cipherText[i])).Value - key;
                    if (tempVal < 0) tempVal += 26;
                    tempVal %= 26;
                    text += dic.FirstOrDefault(v => v.Value == tempVal).Key;
                }

                return text;
            }
            catch (Exception e)
            {
                return "Error: --> " + e.Message;
            }
        }
        /// <summary>
        /// Detect cipher key by calculation message letter frequency distribution
        /// calculate Correlation between message letter frequency List and English letter frequency distribution
        /// shift message frequency list by one step and calculate Correlation again 
        /// repeat it for 26 time number of English letters
        /// find max Correlation Coefficient
        /// get index of max Correlation Coefficient this is the KEY
        /// </summary>
        /// <param name="cipherText">cipherText</param>
        /// <returns>cipher key</returns>
        private static int DetectKey(string cipherText)
        {
            var correlationList = new List<double>();
            var msgfrqList = GetMessageFrequency(cipherText);

            for (var i = 0; i < NumberOfLetters; i++)
            {
                var r = CalculateCorrelationCoeff(msgfrqList);
                msgfrqList = ShiftOneStep(msgfrqList);
                correlationList.Add(r);
            }

            var maxVal = correlationList.Max(x => Math.Abs(x));
            var key = (correlationList.IndexOf(maxVal) < 0)
                ? correlationList.IndexOf(-maxVal)
                : correlationList.IndexOf(maxVal); // KEY

            //MessageBox.Show(@"Correlation= " + maxVal + @" --> Key= " + key);

            return key;
        }
        /// <summary>
        /// shift list of frequencies by one step 
        /// </summary>
        /// <param name="list">message frequency list</param>
        /// <returns>Shifted List</returns>
        private static List<double> ShiftOneStep(List<double> list)
        {
            var returnList = new List<double>();
            for (var i = 1; i < list.Count; i++)
            {
                returnList.Insert(i - 1, list[i]);
            }
            returnList.Add(list[0]);
            return returnList;
        }
        /// <summary>
        /// Calculate message letters frequency distribution 
        /// </summary>
        /// <param name="message">Cipher Text</param>
        /// <returns>Message frequency list</returns>
        private static List<double> GetMessageFrequency(string message)
        {
            var msgLength =(double) message.Length;
            var messageFrequencyList = new List<double>();

            foreach (var ele in EngFrequencyDic)
            {
                var avg = message.Count(m => m == ele.Key)/msgLength;
                messageFrequencyList.Add(avg);
            }

            return messageFrequencyList;
        }
        /// <summary>
        /// Calculate Correlation Coefficient function between each character frequency in message and its equivalent
        /// English letter frequency  
        /// </summary>
        /// <param name="msgFrqList"> list contains all frequencies distribution for all letters in message </param>
        /// <returns>Correlation Coefficient</returns>
        private static double CalculateCorrelationCoeff(List<double> msgFrqList)
        {
            const double n = NumberOfLetters;
            var sumX = EngFrequencyDic.Values.Sum();
            var sumY = msgFrqList.Sum();

            // ReSharper disable once InconsistentNaming
            double sumXY = 0;
            for (var i = 0; i < EngFrequencyDic.Count; i++)
            {
                sumXY += EngFrequencyDic.ElementAt(i).Value * msgFrqList[i];
            }

            var sumXSqur = EngFrequencyDic.Values.Sum(v => Math.Pow(v, 2));
            var sumYSqur = msgFrqList.Sum(v => Math.Pow(v, 2));

            var r = ((n * sumXY) - (sumX * sumY)) /
                (Math.Sqrt(((n * sumXSqur) - Math.Pow(sumX, 2d)) * ((n * sumYSqur) - Math.Pow(sumY, 2d))));
            return r;
        }
        /// <summary>
        /// constant dictionary contains all English characters and its equivalent number 
        /// </summary>
        /// <returns></returns>
        private static Dictionary<char, int> EnglishLetterDictionary()
        {
            return new Dictionary<char, int>
            {
                {'a', 0},
                {'b', 1},
                {'c', 2},
                {'d', 3},
                {'e', 4},
                {'f', 5},
                {'g', 6},
                {'h', 7},
                {'i', 8},
                {'j', 9},
                {'k', 10},
                {'l', 11},
                {'m', 12},
                {'n', 13},
                {'o', 14},
                {'p', 15},
                {'q', 16},
                {'r', 17},
                {'s', 18},
                {'t', 19},
                {'u', 20},
                {'v', 21},
                {'w', 22},
                {'x', 23},
                {'y', 24},
                {'z', 25}
            };
        }
        /// <summary>
        /// constant dictionary contains all English characters and its frequencies distribution 
        /// </summary>
        /// <returns></returns>
        private static Dictionary<char, double> EnglishFrequencyDictionary()
        {
            return new Dictionary<char, double>
            {
                {'a', 0.07487792},
                {'b', 0.01295442},
                {'c', 0.03544945},
                {'d', 0.03621812},
                {'e', 0.1399891},
                {'f', 0.02183939},
                {'g', 0.0173856},
                {'h', 0.04225448},
                {'i', 0.06653554},
                {'j', 0.00269036},
                {'k', 0.00465726},
                {'l', 0.03569814},
                {'m', 0.0339121},
                {'n', 0.06741725},
                {'o', 0.07372491},
                {'p', 0.02428106},
                {'q', 0.00262254},
                {'r', 0.06140351},
                {'s', 0.06945198},
                {'t', 0.09852595},
                {'u', 0.03004612},
                {'v', 0.01157533},
                {'w', 0.01691083},
                {'x', 0.00278079},
                {'y', 0.01643606},
                {'z', 0.00036173}
            };
        }

    }
}
