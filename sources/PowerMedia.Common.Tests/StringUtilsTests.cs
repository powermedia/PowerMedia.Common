using PowerMedia.Common.Text;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PowerMedia.Common.Tests.Text
{
    [TestFixture]
    [Timeout(50)]
    public class StringUtilsTests
    {

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ImplodeCollection_NullArgumentsOnlyTest()
        {
            const List<object> list = null;
            StringUtils.ImplodeCollectionIgnoringNulls(list, null, null, null, null);
        }

        [Test]
        public void ImplodeCollection_StringCollection_WithNulls_WithPrefix()
        {
            List<string> test = new List<string>()
            {
                "0",null,"1",null,"2",null,"3","4",null,"5"
            };
            string result = StringUtils.ImplodeCollectionIgnoringNulls(test, "|", null, "a");
            Assert.AreEqual("a0|a1|a2|a3|a4|a5",result);
        }

        [Test]
        public void ImplodeCollection_IntCollection_WithLimit_WithPrefix()
        {
            List<int> test = new List<int>()
            {
                0,1,2,3,4,5
            };
            string result = StringUtils.ImplodeCollectionIgnoringNulls(test, "|", 5, "a");
            Assert.AreEqual("a0|a1|a2|a3|a4", result);
        }

        [Test]
        public void ImplodeCollection_IntCollection_WithNulls_WithLimit_WithPrefix()
        {
            List<string> stringList= new List<string>()
           {
                "0",null,"1",null,"2",null,"3","4",null,"5"
            };
            string result = StringUtils.ImplodeCollectionIgnoringNulls(stringList, "|",5, "a");
            Assert.AreEqual("a0|a1|a2|a3|a4", result);
        }
        
        private string ConvertIntByAdding5(int x)
        {
            x = x + 5;
            return x.ToString();
        }

        [Test]
        public void ImplodeCollection_IntArray_WithConverter_WithLimit_WithPrefix()
        {
            int[] test = new int[6] {0, 1, 2, 3, 4, 5};
            string result = StringUtils.ImplodeCollectionIgnoringNulls(test, "|", 6, "a",ConvertIntByAdding5);
            Assert.AreEqual  ("a5|a6|a7|a8|a9|a10",  result );
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ImplodeCollection_EmptyCollection_BelowZeroLimit()
        {
        	int[] test = new int[1] {0};
        	StringUtils.ImplodeCollectionIgnoringNulls(test,string.Empty,-5);
        }
        
        [Test]
        public void ImplodeCollection_EmptyCollection()
        {
        	string result = StringUtils.ImplodeCollectionIgnoringNulls(new List<int>());
        	Assert.AreEqual(string.Empty, result);
        }
        
        private string ConvertStringToLetter_b_(string s)
        {
            return "b";
        }

        [Test]
        public void ImplodeCollection_StringCollection_WithNulls_WithConverter_WithLimit_WithPrefix()
        {
            List<string> test = new List<string>()
            {
                "0",null,"1",null,"2",null,"3","4",null,"5"
            };
            string result = StringUtils.ImplodeCollectionIgnoringNulls(test, "|", 3, "a",ConvertStringToLetter_b_);
            Assert.AreEqual("ab|ab|ab", result);
        }

        [Test]
        public void ImplodeCollection_StringCollection_WithNulls_WithConverter_NoLimit_WithPrefix()
        {
            List<string> test = new List<string>()
            {
                "0",null,"1",null,"2",null,"3","4",null,"5"
            };
            string result =StringUtils.ImplodeCollectionIgnoringNulls(test, "|",  null, "a",ConvertStringToLetter_b_);
            Assert.AreEqual( "ab|ab|ab|ab|ab|ab", result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanEncodeChar_NullArgumentTest()
        {
            StringUtils.CanEncodeChar('ą', null);
        }

        [Test]
        public void CanEncodeChar_Test()
        {
            {
                Encoding encoding = Encoding.ASCII;
                Assert.IsFalse(StringUtils.CanEncodeChar('ą', encoding), "ą character and ASCII encoding");
            }
            {
                Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
                Assert.IsFalse(StringUtils.CanEncodeChar('ą', encoding), "ą character and ISO-8859-1 encoding");
            }
            {
                Encoding encoding = Encoding.GetEncoding("ISO-8859-2");
                Assert.IsTrue(StringUtils.CanEncodeChar('ą', encoding), "ą character and ISO-8859-2 encoding");
            }
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertDocumentToHTML_TestNullArgumentTest()
        {
            StringUtils.ConvertDocumentToHTML(null);
        }

        [Test]
        public void ConvertDocumentToHTML_Test()
        {
            {
                string result = StringUtils.ConvertDocumentToHTML("");
                Assert.IsNotNull(result);
                Assert.AreEqual("", result);
            }
            ///@TODO: extend test: add strings with normal text in C# format, add another string with code 
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertParameterCollectionToString_NullArgumentTest()
        {
            StringUtils.ConvertParameterCollectionToString(null);
        }

        [Test]
        public void ConvertParameterCollectionToString_Test()
        {
            NameValueCollection nv = new NameValueCollection()
            {
            {"1", "a"},
            {"2", "b"},
            {"3", "c"},
            {"4", "d"},
            {"5", "e"}
            };
            string result=StringUtils.ConvertParameterCollectionToString(nv);
            Assert.AreEqual("1=a&2=b&3=c&4=d&5=e", result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertParameterCollectionToString_WithNullsArgumentTest()
        {
            NameValueCollection nv = new NameValueCollection()
            {
            {"1", "a"},
            {null, "b"},
            {"3", "c"},
            {null,null},
            {"4", null},
            {"5", "e"}
            };
            string result = StringUtils.ConvertParameterCollectionToString(nv);
            Assert.Fail("implicitely skipping null arguments = wrong");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FixWWWContent_BothNullArgumentTest()
        {
            StringUtils.FixWwwContent(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FixWWWContent_FirstNullArgumentTest()
        {
            StringUtils.FixWwwContent(null, Encoding.ASCII);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FixWWWContent_SecondNullArgumentTest()
        {
            StringUtils.FixWwwContent("aa", null);
        }

        [Test]
        [Timeout(300)]
        public void FixWWWContent_ASCIITest()
        {

            
            Encoding encoding = Encoding.ASCII;
            string result=StringUtils.FixWwwContent("§«µ aasa¬", encoding);
            Assert.AreEqual
                (
                "&#167;&#171;&#181; aasa&#172;",
                result,
                "test string: §«µ aasa¬"
                );

        }

        [Test]
        [Timeout(150)]
        public void FixWWWContent_Latin1Test()
        {
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            string result = StringUtils.FixWwwContent("§«µ aasa¬", encoding);
            Assert.AreEqual
                (
                "§«µ aasa¬",
                result, 
                "test string: §«µ aasa¬"
                );

        }

        [Test]
        [Timeout(300)]
        public void RemoveHtmlTags_Test()
        {
            string test = "<abcd>efgh<ijklmn>oprst</uwxyz>01<23456> <789><33333><0>";
            string result = StringUtils.RemoveHtmlTags(test);
            Assert.AreEqual
                (
                "efghoprst01", 
                result
                );
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveHtmlTags_NullArgumentTest()
        {
            StringUtils.RemoveHtmlTags(null);

        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TrimDescriptionToSpecifiedLenght_NullArgumentTest()
        {
            StringUtils.TrimDescriptionToSpecifiedLength(null, 4);
        }

        [Test]
        public void TrimDescriptionToSpecifiedLength_NoTrimTest()
        {
            string test = "Ostatni z samolotów, który był dotąd sprawny";
            string result = StringUtils.TrimDescriptionToSpecifiedLength(test, 200);
            Assert.AreEqual(test, result);
        }

        [Test]
        public void TrimDescriptionToSpecifiedLength_Test()
        {
            string test = "Ostatni z samolotów, który był dotąd sprawny";
            string result = StringUtils.TrimDescriptionToSpecifiedLength(test, 12);
            Assert.AreEqual("Ostatni z .", result);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertTextEncoding_EncodingVersion_AllNullArgumentTest()
        {
            StringUtils.ConvertTextEncoding(null as Encoding, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertTextEncoding_EncodingVersion_FirstNullArgumentTest()
        {
            StringUtils.ConvertTextEncoding(null, Encoding.ASCII, "asaas");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertTextEncoding_EncodingVersion_SecondNullArgumentTest()
        {
            StringUtils.ConvertTextEncoding(Encoding.ASCII, null, "asdasasd");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertTextEncoding_EncodingVersion_ThirdNullArgumentTest()
        {
            StringUtils.ConvertTextEncoding(Encoding.ASCII, Encoding.Unicode, null);
        }

        [Test]
        public void ConvertTextEncoding_EncodingVersion_SameEncodingTest()
        {
            string test = "January";
            string result=StringUtils.ConvertTextEncoding(Encoding.ASCII, Encoding.ASCII, test);
            Assert.AreEqual(test,result,"test string: \"January\"");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertTextEncoding_NameVersion_AllNullArgumentTest()
        {

            StringUtils.ConvertTextEncoding(null as string, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertTextEncoding_NameVersion_FirstNullArgumentTest()
        {
   
            StringUtils.ConvertTextEncoding(null, "ISO-8859-1", "asaas");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertTextEncoding_NameVersion_SecondNullArgumentTest()
        {
            StringUtils.ConvertTextEncoding("ISO-8859-1", null, "asdasasd");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertTextEncoding_NameVersion_ThirdNullArgumentTest()
        {

            StringUtils.ConvertTextEncoding("ISO-8859-1", "ISO-8859-2", null);
        }

        [Test]
        public void ConvertTextEncoding_NameVersion_SameEncodingTest()
        {
            string test = "January";
            string result = StringUtils.ConvertTextEncoding("ISO-8859-1", "ISO-8859-1", test);
            Assert.AreEqual(test, result, "test string: \"January\"");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertTextEncoding_NameVersion_BothIllegallEncodingTest()
        {
            StringUtils.ConvertTextEncoding("asdasadas", "asdasdasd", "asdasdas");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertTextEncoding_NameVersion_FirstIllegallEncodingTest()
        {
            StringUtils.ConvertTextEncoding("asdasadas", "ISO-8859-1", "asdasdas");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertTextEncoding_NameVersion_SecondIllegallEncodingTest()
        {
            StringUtils.ConvertTextEncoding("ISO-8859-1", "asdasdasd", "asdasdas");
        }

        [Test]
        public void ConvertTextEncoding_CompareTest()
        {
            string test = "ßれDÅちó金譱わ罪わへDFebruaryÙぞZÇ2ÆòW";
            string encoding1 = "unicodeFFFE";
            string encoding2 = "us-ascii";
            string result = StringUtils.ConvertTextEncoding(encoding1, encoding2, test);
            string result2 = StringUtils.ConvertTextEncoding(Encoding.GetEncoding(encoding1), Encoding.GetEncoding(encoding2), test);
            Assert.AreEqual
                (
                result2,
                result,
                "unicode to ascii conversion, test string with Japanese characters:\"ßれDÅちó金譱わ罪わへDFebruaryÙぞZÇ2ÆòW\""
                );
        }



        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplacePolishCharacters_NullArgumentTest()
        {
            StringUtils.ReplacePolishCharacters(null);
        }

        [Test]
        public void ReplacePolishCharacters_Test()
        {
            string test = "Niegdyś jedna z wielu bananowych republik, która w swej naiwności wierzyła,ćŁĆŚĄąĘęŻżŹźÓ";
            const string wynik = "Niegdys jedna z wielu bananowych republik, ktora w swej naiwnosci wierzyla,cLCSAaEeZzZzO";
            string result = StringUtils.ReplacePolishCharacters(test);
            Assert.AreEqual
                (
                wynik,
                result,
                "test string: \"Niegdyś jedna z wielu bananowych republik, która w swej naiwności wierzyła,ćŁĆŚĄąĘęŻżŹźÓ\""
                );
        }

        [Test]
        public void ReplacePolishCharacters_NoChangeTest()
        {
            const string test = "asasdasdasd";
            string result = StringUtils.ReplacePolishCharacters(test);
            Assert.AreEqual
                (
                test, 
                result,
                "test string: \"asasdasdasd\""
                );
        }
        
        [Test]
        public void ReplacePolishCharacters_JapaneseCharactersTest()
        {
            string test = "aﾘぜべdD3hよﾆﾃYJ1月öô土ぅﾆ媾2琉z水曜日涖ÅNiegdyś jedna z wielu bananowych republik, która w swej naiwności wierzyła,ćŁĆŚĄąĘęŻżŹźÓ";
            const string wynik = "aﾘぜべdD3hよﾆﾃYJ1月öô土ぅﾆ媾2琉z水曜日涖ÅNiegdys jedna z wielu bananowych republik, ktora w swej naiwnosci wierzyla,cLCSAaEeZzZzO";
            string result = StringUtils.ReplacePolishCharacters(test);
            Assert.AreEqual
                (
                wynik, 
                result,
                "test string: \"aﾘぜべdD3hよﾆﾃYJ1月öô土ぅﾆ媾2琉z水曜日涖ÅNiegdyś jedna z wielu bananowych republik, która w swej naiwności wierzyła,ćŁĆŚĄąĘęŻżŹźÓ\""
                );
        }
        
        [Test]
        public void ReplacePolishCharacters_JapaneseCharactersNoChangeTest()
        {
            const string test = "aﾘぜべdD3hよﾆﾃYJ1月öô土ぅﾆ媾2琉z水曜日涖Åasasdasdasd";
            string result = StringUtils.ReplacePolishCharacters(test);
            Assert.AreEqual
                (
                test, 
                result,
                "test string: \"aﾘぜべdD3hよﾆﾃYJ1月öô土ぅﾆ媾2琉z水曜日涖Åasasdasdasd\""
                );
        }

        [Test]
        public void FormatFullDate_Test()
        {
            DateTime data = new DateTime(2004, 11, 2);
            string result = StringUtils.FormatFullDate(data);
            if(Environment.OSVersion.Platform == PlatformID.Unix) //TODO report bug to Mono
            {
                Assert.AreEqual("2 listopad 2004", result);
            }
            else
            {
                Assert.AreEqual("2 listopada 2004", result);
            }
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatPrice_DecimalVersionNullArgumentTest()
        {
            string result = StringUtils.FormatPrice(3.12m, null);
            Assert.AreEqual("3,12", result);
        }

        [Test]
        public void FormatPrice_DecimalVersionTest()
        {
            string result = StringUtils.FormatPrice(3.12m, "PLN");
            Assert.AreEqual("3,12 PLN", result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatPrice_FloatVersionNullArgumentTest()
        {
            string result = StringUtils.FormatPrice(3.12f, null);
            Assert.AreEqual("3,12", result);
        }

        [Test]
        public void FormatPrice_FloatVersionTest()
        {
            string result = StringUtils.FormatPrice(3.12f, "PLN");
            Assert.AreEqual("3,12 PLN", result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MD5_NullArgumentTest()
        {
            StringUtils.MD5(null);
        }

        [Test]
        public void MD5_Test()
        {
            string result = StringUtils.MD5("asasasdasds");
            Assert.AreEqual("e2abf6594c935a9d8372722eec416b52", result, "test string: \"asasasdasds\"");
        }

        string DummyResolver(string tag)
        {
            return tag;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FirstVersion_AllNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases(null as string, null, null, null as StringUtils.TagResolver);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FirstVersion_FirstNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases(null as string, "asda", "asada", DummyResolver);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FirstVersion_SecondNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases( "asda", null, "asada", DummyResolver);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FirstVersion_ThirdNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases("asda", "asada", null, DummyResolver);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FirstVersion_FourthNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases("null as string", "asda", "asada", null as StringUtils.TagResolver);
        }

        string SimpleResolver(string tag)
        {
            return string.Format("\nit was: \n{0}\n", tag);
        }

        [Test]
        [Timeout(150)]
        public void ReplaceEnclosedPhrases_FirstVersion_Test()
        {
            string test = "blablba asdas [t]saasasd<t>sdasdas[trrt] asas<t> [t]aasasasd<t>[t] as[t] r<t> [t] aa";
            string correct = "blablba asdas \nit was: \nsaasasd\nsdasdas[trrt] asas<t> \nit was: \naasasasd\n[t] as\nit was: \n r\n [t] aa";
            string opening = "\\[t\\]";
            string closing = "\\<t\\>";
            string result = StringUtils.ReplaceEnclosedPhrases(test, opening, closing, SimpleResolver);
            Assert.AreEqual(correct, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_SecondVersion_AllNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases(null as string, null, null, null as Dictionary<string,string>);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_SecondVersion_FirstNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases(null as string, "asda", "asada", new Dictionary<string, string>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_SecondVersion_SecondNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases("asda", null, "asada", new Dictionary<string, string>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_SecondVersion_ThirdNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases("asda", "asada", null, new Dictionary<string, string>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_SecondVersion_FourthNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases("null as string", "asda", "asada", null as Dictionary<string, string>);
        }


        [Test]
        public void ReplaceEnclosedPhrases_SecondVersion_Test()
        {
            string test = "asasas[t]3<t>sasas<t>[t]ゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚<t>[t][t]1<t>[t]aasa";
            Dictionary<string, string> mapping = new Dictionary<string, string>() 
            {
                {"1","jedynka"},
                {"ゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚","krzak"}
            };
            string correct = "asasassasas<t>krzak[t]jedynka[t]aasa";
            string opening = "\\[t\\]";
            string closing = "\\<t\\>";
            string result = StringUtils.ReplaceEnclosedPhrases(test, opening, closing, mapping);
            Assert.AreEqual(correct, result);
        }

        [Test]
        public void ReplaceEnclosedPhrases_SecondVersion_TabAndNewLineTest()
        {
            string test = "asd\nas\tasd\n3d\tsasasd\nゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚d\td\nd\n1d\td\naasa";
            Dictionary<string, string> mapping = new Dictionary<string, string>() 
            {
                {"1","jedynka"},
                {"ゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚","krzak"}
            };
            string correct = "asd\nas\tassasaskrzakd\njedynkad\naasa";
            string opening = "d\\n";
            string closing = "d\\t";
            string result = StringUtils.ReplaceEnclosedPhrases(test, opening, closing, mapping);
            Assert.AreEqual(correct, result, "Test use opening and closing phrases with newline and tab");
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FullVersion_AllNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases(null as string, null, null, null as Dictionary<string, string>, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FullVersion_FirstNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases(null as string, "asda", "asada", new Dictionary<string, string>(), "Asda");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FullVersion_SecondNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases("asda", null, "asada", new Dictionary<string, string>(), "Asda");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FullVersion_ThirdNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases("asda", "asada", null, new Dictionary<string, string>(), "Asda");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceEnclosedPhrases_FullVersion_FourthNullArgumentTest()
        {
            StringUtils.ReplaceEnclosedPhrases("null as string", "asda", "asada", null as Dictionary<string, string>, "Asda");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(150)]
        public void ReplaceEnclosedPhrases_FullVersion_FifthNullArgumentTest()
        {
            string test = "asasas[t]3<t>sasas<t>[t]ゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚<t>[t][t]1<t>[t]aasa";
            Dictionary<string, string> mapping = new Dictionary<string, string>() 
            {
                {"1","jedynka"},
                {"ゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚","krzak"}
            };
            string correct = "asasassasas<t>krzak[t]jedynka[t]aasa";
            string opening = "\\[t\\]";
            string closing = "\\<t\\>";
            string result = StringUtils.ReplaceEnclosedPhrases(test, opening, closing, mapping, null);
            Assert.AreEqual(correct, result);
        }
 
        [Test]
        [Timeout(150)]
        public void ReplaceEnclosedPhrases_FullVersion_Test()
        {
            string test = "asasas[t]3<t>sasas<t>[t]ゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚<t>[t][t]1<t>[t]aasa";
            Dictionary<string, string> mapping = new Dictionary<string, string>() 
            {
                {"1","jedynka"},
                {"ゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚","krzak"}
            };
            string correct = "asasasTUsasas<t>krzak[t]jedynka[t]aasa";
            string opening = "\\[t\\]";
            string closing = "\\<t\\>";
            string result = StringUtils.ReplaceEnclosedPhrases(test, opening, closing, mapping, "TU");
            Assert.AreEqual(correct, result);
        }

        [Test]
        public void ReplaceEnclosedPhrases_FullVersion_TabAndNewLineTest()
        {
            string test = "asd\nas\tasd\n3d\tsasasd\nゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚d\td\nd\n1d\td\naasa";
            Dictionary<string, string> mapping = new Dictionary<string, string>() 
            {
                {"1","jedynka"},
                {"ゅ黥ぢ蛎ぎ火鰲金月曜日せ鐘擬尚","krzak"}
            };
            string correct = "asd\nas\tasTUsasaskrzakd\njedynkad\naasa";
            string opening = "d\\n";
            string closing = "d\\t";
            string result = StringUtils.ReplaceEnclosedPhrases(test, opening, closing, mapping, "TU");
            Assert.AreEqual(correct, result, "Test use opening and closing phrases with newline and tab");
        }
		
        [Test]
        public void ReplaceMatchedEnclosedPhrasesTest()
        {
        	string input = "&*(*($..jeden976,,kl;jx*..adsasd,,lldas*&(*..dwa,,..";
   
            
            Dictionary<string, string> mapping = new Dictionary<string, string>() 
            {
                {"jeden","1"},
                {"dwa","2"}
            };
            string correct = "&*(*($..jeden976,,kl;jx*..adsasd,,lldas*&(*2..";
            string opening = "..";
            string closing = ",,";
            string result = StringUtils.ReplaceMatchedEnclosedPhrases(input,opening,closing,mapping);
            Assert.AreEqual(correct, result);
            
        }
        
     
        
        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void ContainsChar_nullText_nullCharsArray()
        {
            string text = null;
            char[] chars = null;
            StringUtils.ContainsChar(text, chars);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void ContainsChar_someText_nullCharsArray()
        {
            string text = "test";
            char[] chars = null;
            StringUtils.ContainsChar(text, chars);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void ContainsChar_nullText_someCharsArray()
        {
            string text = null;
            char[] chars = new char[]{'a', 'b', 'c'};
            StringUtils.ContainsChar(text, chars);
        }

        [Test]
        public void ContainsChar_emptyText_emptyCharsArray()
        {
            string text = string.Empty;
            char[] chars = new char[3];
            Assert.False(StringUtils.ContainsChar(text, chars));
        }

        [Test]
        public void ContainsChar_emptyText_someCharsArray()
        {
            string text = string.Empty;
            char[] chars = new char[]{'a','b','c'};
            Assert.False(StringUtils.ContainsChar(text, chars));
        }

        [Test]
        public void ContainsChar_someText_wrongCharsArray()
        {
            string text = "test";
            char[] chars = new char[] { 'a', 'b', 'c' };
            Assert.False(StringUtils.ContainsChar(text, chars));
        }

        [Test]
        public void ContainsChar_someText_someCorrectCharsArray()
        {
            string text = "test";
            char[] chars = new char[] { 'a', 'e', 'c' };
            Assert.True(StringUtils.ContainsChar(text, chars));
        }

        [Test]
        public void ContainsChar_someText_allCorrectCharsArray()
        {
            string text = "test";
            char[] chars = new char[] { 't', 'e', 's', 't' };
            Assert.True(StringUtils.ContainsChar(text, chars));
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void ContainsChars_nullText_nullCharsArray()
        {
            string text = null;
            char[] chars = null;
            StringUtils.ContainsChars(text, chars);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void ContainsChars_someText_nullCharsArray()
        {
            string text = "test";
            char[] chars = null;
            StringUtils.ContainsChars(text, chars);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void ContainsChars_nullText_someCharsArray()
        {
            string text = null;
            char[] chars = new char[] { 'a', 'b', 'c' };
            StringUtils.ContainsChars(text, chars);
        }

        [Test]
        public void ContainsChars_emptyText_emptyCharsArray()
        {
            string text = string.Empty;
            char[] chars = new char[3];
            Assert.False(StringUtils.ContainsChars(text, chars));
        }

        [Test]
        public void ContainsChars_emptyText_someCharsArray()
        {
            string text = string.Empty;
            char[] chars = new char[] { 'a', 'b', 'c' };
            Assert.False(StringUtils.ContainsChars(text, chars));
        }

        [Test]
        public void ContainsChars_someText_wrongCharsArray()
        {
            string text = "test";
            char[] chars = new char[] { 'a', 'b', 'c' };
            Assert.False(StringUtils.ContainsChars(text, chars));
        }

        [Test]
        public void ContainsChars_someText_someCorrectCharsArray()
        {
            string text = "test";
            char[] chars = new char[] { 't', 'e', 'c' };
            Assert.False(StringUtils.ContainsChars(text, chars));
        }

        [Test]
        public void ContainsChars_someText_allCorrectCharsArray()
        {
            string text = "test";
            char[] chars = new char[] { 't', 'e', 's', 't' };
            Assert.True(StringUtils.ContainsChars(text, chars));
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CountNonEmptyLinesInTextFile_NullArgument()
        {
        	StringUtils.CountNonEmptyLinesInTextFile(null);
        }
    }
}
