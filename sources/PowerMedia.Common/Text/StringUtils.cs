using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace PowerMedia.Common.Text
{
    public static class StringUtils
    {
        private static readonly CultureInfo _polishCulture = new CultureInfo("pl-PL");
        public static CultureInfo PolishCulture
        {
            get
            {
                return _polishCulture;
            }
        }

        private static readonly List<char> _punctuationCharacters = new List<char>() {
            ',', ':', '`'
        };

        private readonly static Dictionary<string, string> _polishCharactersMap = new Dictionary<string, string>();


        static StringUtils()
        {
            _polishCharactersMap.Add("ł", "l");
            _polishCharactersMap.Add("ó", "o");
            _polishCharactersMap.Add("ś", "s");
            _polishCharactersMap.Add("ć", "c");
            _polishCharactersMap.Add("ń", "n");
            _polishCharactersMap.Add("ą", "a");
            _polishCharactersMap.Add("ę", "e");
            _polishCharactersMap.Add("ź", "z");
            _polishCharactersMap.Add("ż", "z");
        }


        private static Converter<TElement, string> GetDefaultConverter<TElement>()
        {
            return delegate(TElement element)
                                    {
                                        if (element == null)
                                        {
                                            return null;
                                        }
                                        return element.ToString();
                                    };
        }

        private static Converter<TElement, string> MakePrefixedConverter<TElement>(Converter<TElement, string> converter, string prefix)
        {
            return delegate(TElement o) { return prefix + converter(o); };
        }

        public static string ImplodeCollectionIgnoringNulls<TElement>
            (
            IList<TElement> collection
            )
        {
            return ImplodeCollectionIgnoringNulls(collection, null, null, null, null);
        }

        public static string ImplodeCollectionIgnoringNulls<TElement>
            (
            IList<TElement> collection,
            string separator,
            int? limit
            )
        {
            return ImplodeCollectionIgnoringNulls(collection, separator, limit, null, null);
        }


        public static string ImplodeCollectionIgnoringNulls<TElement>
            (
            IList<TElement> collection,
            string separator,
            int? limit,
            string elementPrefix
            )
        {
            return ImplodeCollectionIgnoringNulls(collection, separator, limit, elementPrefix, null);
        }

        /// <summary>
        /// Function which implodes an collection to a string.
        /// </summary>
        /// <param name="enumerable">Collection to implode</param>
        /// <param name="funcConvert">Function which converts every element of the collection to a string (default function, ToStringNoConvertion, is used when null)</param>
        /// <param name="separator">A separator used to join strings representing objects in the collection</param>
        /// <param name="count">Number of non-null elements to implode (no restriction if null)</param>
        /// <param name="appendEmpty">should append empty strings?</param>
        /// <returns>String containing of collection's elements as strings, separated by given separator.</returns>
        public static string ImplodeCollectionIgnoringNulls<TElement>
            (
            IList<TElement> collection,
            string separator,
            int? limit,
            string elementPrefix,
            Converter<TElement, string> converterFunction
            ) //where TElement : class
        {
            if (collection == null)
            {
                throw new ArgumentNullException();
            }
            if (collection.Count == 0)
            {
                return string.Empty;
            }
            if (limit != null && limit <= 0)
            {
                throw new ArgumentOutOfRangeException("limit cannot be 0 or less");
            }

            Converter<TElement, string> converter = converterFunction;
            if (converter == null)
            {
                converter = GetDefaultConverter<TElement>();
            }
            Converter<TElement, string> converterWithPrefix = MakePrefixedConverter(converter, elementPrefix);

            IList<TElement> collectionWithNoNulls = PowerMedia.Common.Collections.CollectionUtils.CopyOnlyNonNullElements(collection);


            //calculate breaking point
            int breakingPoint = collectionWithNoNulls.Count;
            if (limit != null)
            {
                breakingPoint = limit.Value;
            }

            int numberOfElementsEncountered = 0;
            StringBuilder output = new StringBuilder();
            foreach (var element in collectionWithNoNulls)
            {
                ++numberOfElementsEncountered;

                string elementWithPrefix = converterWithPrefix(element);

                if (elementWithPrefix != null && elementWithPrefix != "")
                {
                    output.Append(elementWithPrefix);
                    if (numberOfElementsEncountered == breakingPoint)
                    {
                        break;
                    }//stop before adding new separator i.e. separator can't be the last value on the string
                    output.Append(separator);
                }
            }


            return output.ToString();
        }

        /// <summary>
        /// Function which implodes an NameValueCollection  to string in format"key1=value1&key2=value2.".
        /// </summary>
        /// <param name="parameters">Collection to implode</param>
        public static string ConvertParameterCollectionToString(NameValueCollection parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException();
            }
            List<string> parametersHelper = new List<string>(parameters.Count);

            foreach (string key in parameters.Keys)
            {
                if (key == null || parameters[key] == null)
                {
                    throw new ArgumentNullException();
                }
                parametersHelper.Add(key + "=" + parameters[key]);
            }

            return StringUtils.ImplodeCollectionIgnoringNulls<string>(parametersHelper, "&", null, null, null);
        }

        public delegate string TagResolver(string str);

        /// <summary>
        /// Passes tokens of text of form [openingString][phrase][closingString] found in input string to a resolver function, which can transform them. For example function can find strings of form "[photo]photo1.jpg[/photo]" and replace it by "&lt;img src="photo.jpg"&gt;" using the adequate resolver delegate.
        /// </summary>
        /// <param name="inputString">The input string to run replacing process on.</param>
        /// <param name="openingString">Token's opening string. Is a regular expression string.</param>
        /// <param name="closingString">Token's closing string. Is a regular expression string.</param>
        /// <param name="resolver">The phrase resolving function.</param>
        /// <returns>Input string in which the tokens have been processed by resolver.</returns>
        public static string ReplaceEnclosedPhrases(string inputString, string openingString, string closingString, TagResolver resolver)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException("inputString");
            }
            else if (openingString == null)
            {
                throw new ArgumentNullException("openingString");
            }
            else if (closingString == null)
            {
                throw new ArgumentNullException("closingString");
            }
            else if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }

            Regex regex;

            string regexString = string.Format(@"(?<openingString>{0})(?<tag>[^{0}{1}]*)(?<closingString>{1})",
                openingString, closingString);

            regex = new Regex(regexString, RegexOptions.Compiled);

            MatchEvaluator tagEvaluator = delegate(Match match)
            {
                return resolver(match.Groups["tag"].Value);
            };

            return (inputString == null) ? "" : regex.Replace(inputString, tagEvaluator);
        }

        /// <summary>
        /// This overload allows you to specify a mapping of phrases to their replacements instead of a resolver function.
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="openingString"></param>
        /// <param name="closingString"></param>
        /// <param name="tagMapping">A dictionary containing the mappings of form phrase=>replacement.</param>
        /// <param name="textIfNoTag">Replacement for tokens for which the phrase is not contained in the tagMapping.</param>
        /// <returns>For example mapping: "image1"=>"&lt;img src="bird.jpg"&gt;", openingPhrase "[", closingPhrase "]" and the text "This is the image of a bird: [image1]" the result is "This is the image of a bird: &lt;img src="bird.jpg"&gt;"</returns>
        public static string ReplaceEnclosedPhrases(string inputString, string openingString, string closingString, Dictionary<string, string> tagMapping, string textIfNoTag)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException("inputString");
            }
            else if (openingString == null)
            {
                throw new ArgumentNullException("openingString");
            }
            else if (closingString == null)
            {
                throw new ArgumentNullException("closingString");
            }
            else if (tagMapping == null)
            {
                throw new ArgumentNullException("tagMapping");
            }
            else if (textIfNoTag == null)
            {
                throw new ArgumentNullException("textIfNoTag");
            }


            TagResolver resolver = delegate(string tag)
            {
                if (tagMapping.ContainsKey(tag))
                {
                    return tagMapping[tag];
                }

                return textIfNoTag;
            };

            return ReplaceEnclosedPhrases(inputString, openingString, closingString, resolver).Trim();
        }

        public static string ReplaceMatchedEnclosedPhrases(string inputString, string openingString, string closingString, Dictionary<string, string> tagMapping)
        {
            TagResolver resolver = delegate(string tag)
            {
                if (tagMapping.ContainsKey(tag))
                {
                    return tagMapping[tag];
                }

                return openingString + tag + closingString;
            };

            return ReplaceEnclosedPhrases(inputString, openingString, closingString, resolver);
        }


        /// <summary>
        /// Calls <code>ReplaceEnclosedPhrases(string inputString, string openingString, string closingString, Dictionary&lt;string, string&gt; tagMapping, string textIfNoTag)</code> providing an empty string as the "textIfNoTag" parameter.
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="openingString"></param>
        /// <param name="closingString"></param>
        /// <param name="tagMapping"></param>
        /// <returns></returns>
        public static string ReplaceEnclosedPhrases(string inputString, string openingString, string closingString, Dictionary<string, string> tagMapping)
        {
            return ReplaceEnclosedPhrases(inputString, openingString, closingString, tagMapping, ""); //, RegexOptions.Compiled);
        }



        /// <summary>
        /// Checks whether the character can be encoded in the specified encoding.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static bool CanEncodeChar(char c, Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            byte[] arr = encoding.GetBytes(new char[] { c });
            string encoded = encoding.GetString(arr);
            if (encoded.Length != 1)
            {
                return false;
            }
            if (encoded[0] != c)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Makes unicode content displayable on a webpage in a certain encoding, i.e. fixes and prepares unicode content from the DB by replacing unicode out-of-encoding's-range characters with entities.
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="outputEncoding"></param>
        /// <returns></returns>

        public static string FixWwwContent(string inputString, Encoding outputEncoding)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException("inputString");
            }
            else if (outputEncoding == null)
            {
                throw new ArgumentNullException("outputEncoding");
            }

            // Encoding string to output encoding to see which characters are out of encoding's range - they are replaced by questionmarks
            string encodedString = outputEncoding.GetString(outputEncoding.GetBytes(inputString));
            // We are going to look for questionmarks
            Regex regex = new Regex("\\?");

            MatchEvaluator evaluator = delegate(Match match)
            {
                // if the questionmark is also a questionmark in the original string, leave it
                if (inputString[match.Index] == '?')
                {
                    return "?";
                }

                // if it wasn't a questionmark in the original string, it means that encoding cannot properly encode the symbol - we have to replace it with a html entity
                return GetUnicodeHtmlEntity(inputString[match.Index]);
            };

            string properlyEncodedString = regex.Replace(encodedString, evaluator);

            return properlyEncodedString;
        }

        /// <summary>
        /// Returns an unicode entity for a specified unicode character.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static string GetUnicodeHtmlEntity(char ch)
        {
            return string.Format("&#{0};", (int)ch);
        }

        /// <summary>
        /// Removes HTML tags from the input string. Html tag is meant to be a string enclosed in sharp brackets (i. e. "&lt;" and "&gt;").
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>        
        public static string RemoveHtmlTags(string inputString)
        {
            return ReplaceEnclosedPhrases(inputString, "\\<", "\\>", new Dictionary<string, string>());
        }

        /// <summary>
        /// Trims string to certain length, assuring that the last word of shortened text will not be cut. Then adds "." at the end.
        /// </summary>
        /// <param name="text">Text to trim.</param>
        /// <param name="maxTextLenght">Maximal length of the processed text.</param>
        /// <returns></returns>
        public static string TrimDescriptionToSpecifiedLength(string text, int maxTextLenght)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length < maxTextLenght)
            {
                return text;
            }
            string properLenghtText = text.Substring(0, maxTextLenght);
            string properLengthTextWithNoWordCut = properLenghtText.Substring(0, properLenghtText.LastIndexOf(" ") + 1);
            return properLengthTextWithNoWordCut + ".";
        }

        /// <summary>
        /// Converts string from one encoding to another.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="textToConvert">String encoded in src encoding.</param>
        /// <returns></returns>
        public static string ConvertTextEncoding(Encoding src, Encoding dest, string textToConvert)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }
            else if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }
            else if (textToConvert == null)
            {
                throw new ArgumentNullException("textToConvert");
            }

            //Encoding src = Encoding.GetEncoding(srcEncoding);
            //Encoding dest = Encoding.GetEncoding(destEncoding);
            byte[] srcTextBytes = src.GetBytes(textToConvert);
            byte[] destTextBytes = Encoding.Convert(dest, src, srcTextBytes);
            char[] destChars = new char[src.GetCharCount(destTextBytes, 0, destTextBytes.Length)];
            src.GetChars(destTextBytes, 0, destTextBytes.Length, destChars, 0);
            return new string(destChars);
        }

        public static string ConvertTextEncoding(string srcEncoding, string destEncoding, string textToConvert)
        {
            return ConvertTextEncoding(Encoding.GetEncoding(srcEncoding), Encoding.GetEncoding(destEncoding), textToConvert);
        }

        public static string ReplacePolishCharacters(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            StringBuilder output = new StringBuilder(input);
            foreach (string plChar in _polishCharactersMap.Keys)
            {
                output = output.Replace(plChar, _polishCharactersMap[plChar]);
                output = output.Replace(plChar.ToUpper(), _polishCharactersMap[plChar].ToUpper());
            }
            return output.ToString();
        }

        public static string RemovePunctuationCharacters(string input)
        {

            if (input == null)
            {
                throw new ArgumentNullException();
            }
            StringBuilder output = new StringBuilder(input);
            foreach (char punctuationChar in _punctuationCharacters) {
                output.Replace(punctuationChar, ' ');
            }
            return output.ToString();
        }

        public static string RemoveDoubleSpaces(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            string output = RemoveDoubleSpaces(new StringBuilder(input));
            return output;
            
        }

        private static string RemoveDoubleSpaces(StringBuilder stringBuilder)
        {
            while (stringBuilder.ToString().Contains("  "))
            {
                stringBuilder =  stringBuilder.Replace("  ", " ");    
            }

            return stringBuilder.ToString();
        }

        public static string FormatFullDate(DateTime dateTime)
        {
            string dateStr = dateTime.ToString("D", PolishCulture.DateTimeFormat);
            return dateStr;
        }

        public static string FormatPrice(decimal price, string paymentCurrency)
        {
            if (paymentCurrency == null)
            {
                throw new ArgumentNullException("paymentCurrency");
            }
            string priceStr = price.ToString("C", PolishCulture).ToLower().Replace("zł", paymentCurrency.ToUpper());
            return priceStr;
        }

        public static string FormatPrice(float price, string paymentCurrency)
        {
            if (paymentCurrency == null)
            {
                throw new ArgumentNullException("paymentCurrency");
            }
            return FormatPrice((decimal)price, paymentCurrency);
        }

        //name don`t tell what function does - name-change needed
        public static string ConvertDocumentToHTML(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            string htmlText = text.Trim();
            htmlText = htmlText.Replace("\r\n", "<br />");
            htmlText = htmlText.Replace("\n", "<br />");
            htmlText = htmlText.Replace("\r", "<br />");
            htmlText = htmlText.Replace("”", "\"");
            htmlText = htmlText.Replace("„", "\"");
            htmlText = htmlText.Replace("•?", "");
            htmlText = htmlText.Replace("'", "");
            htmlText = htmlText.Replace("–", "-");
            htmlText = htmlText.Replace("&amp;", "&");
            return htmlText;
        }


        public static string MD5(string s)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(s);
            HashAlgorithm md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(plainTextBytes);
            string result = "";
            string tmp = "";
            foreach (byte b in hash)
            {
                tmp = ((int)b).ToString("X");
                if (tmp.Length == 1)
                    tmp = "0" + tmp;
                result += tmp;
            }
            return result.ToLower();
        }

        public static long CountNonEmptyLinesInTextFile(string filePath)
        {

            StreamReader sr = new StreamReader(filePath);
            long count = 0;
            while (sr.EndOfStream == false)
            {
                if (sr.ReadLine() != Environment.NewLine)
                {
                    count++;
                }
            }
            sr.Close();
            return count;
        }

        public static bool ContainsChar(string text, char[] chars)
        {
            foreach (char aChar in chars)
            {
                if (text.IndexOf(aChar) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsChars(string text, char[] chars)
        {
            foreach (char aChar in chars)
            {
                if (text.IndexOf(aChar) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static string TrimSafely(string stringToTrim)
        {
            if (stringToTrim == null)
            {
                return stringToTrim;
            }
            return stringToTrim.Trim();
        }
    }
}
