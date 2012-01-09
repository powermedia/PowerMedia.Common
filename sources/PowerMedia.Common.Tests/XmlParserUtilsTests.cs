using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml;
using PowerMedia.Common.XML;

namespace PowerMedia.Common.Tests.XML
{
    [TestFixture]
    [Timeout(50)]
    public class XmlParserUtilsTests
    {
        [Test]
        public void GetIntSafely_Test()
        {
            Assert.IsNull(XmlParsingUtils.GetIntSafely(null));

            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<intTest>-1</intTest>");
                var testNode = testDocument["intTest"];
                var result = XmlParsingUtils.GetIntSafely(testNode);
                Assert.IsNotNull(result);
                Assert.AreEqual(-1, result.Value);
            }
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<intTest>-1.32</intTest>");
                var testNode = testDocument["intTest"];
                var result = XmlParsingUtils.GetIntSafely(testNode);
                Assert.IsNull(result);
            }

        }

        [Test]
        public void NodeValidate_Test()
        {
            Assert.IsFalse(XmlParsingUtils.NodeValidate(null));
            var testDocument = new XmlDocument();
            testDocument.LoadXml("<dateTest>2008/01/23</dateTest>");
            var testNode = testDocument["dateTest"];
            Assert.IsTrue(XmlParsingUtils.NodeValidate(testNode));
        }

        [Test]
        [Timeout(150)]
        public void GetDateSafely_Test()
        {
            Assert.IsNull(XmlParsingUtils.GetDateSafely(null));

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<dateTest>2008/01/23</dateTest>");
                var testNode = testDocument["dateTest"];
                var result = XmlParsingUtils.GetDateSafely(testNode);
                Assert.IsNull(result);
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<dateTest>23-01-2008</dateTest>");
                var testNode = testDocument["dateTest"];
                var result = XmlParsingUtils.GetDateSafely(testNode);
                Assert.IsNull(result);
            }

            //right format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<dateTest>2008-01-23</dateTest>");
                var testNode = testDocument["dateTest"];
                DateTime result = (DateTime)XmlParsingUtils.GetDateSafely(testNode);
                Assert.AreEqual(new DateTime(2008, 01, 23), result);
            }


        }


        [Test]
        [Timeout(150)]
        public void GetDateTimeSafely_Test()
        {
            Assert.IsNull(XmlParsingUtils.GetDateTimeSafely(null,null));

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<whole><dateTest></dateTest><timeTest></timeTest></whole>");
                var testDateNode = testDocument["whole"]["dateTest"];
                var testTimeNode = testDocument["whole"]["timeTest"];
                var result = XmlParsingUtils.GetDateTimeSafely(testDateNode, testTimeNode);
                Assert.IsNull(result);
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<whole><dateTest>123</dateTest><timeTest></timeTest></whole>");
                var testDateNode = testDocument["whole"]["dateTest"];
                var testTimeNode = testDocument["whole"]["timeTest"];
                var result = XmlParsingUtils.GetDateTimeSafely(testDateNode, testTimeNode);
                Assert.IsNull(result);
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<whole><dateTest>123</dateTest><timeTest>23</timeTest></whole>");
                var testDateNode = testDocument["whole"]["dateTest"];
                var testTimeNode = testDocument["whole"]["timeTest"];
                var result = XmlParsingUtils.GetDateTimeSafely(testDateNode, testTimeNode);
                Assert.IsNull(result);
            }

            //right format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml(
                    "<whole><dateTest>2008-01-23</dateTest><timeTest>23:23:23</timeTest></whole>"
                    );
                var testDateNode = testDocument["whole"]["dateTest"];
                var testTimeNode = testDocument["whole"]["timeTest"];
                var result = XmlParsingUtils.GetDateTimeSafely(testDateNode, testTimeNode);

                Assert.AreEqual(new DateTime(2008,01,23, 23,23,23), result);
            }

        }


        [Test]
        public void GetDoubleSafely_Test()
        {
            Assert.IsNull(XmlParsingUtils.GetDoubleSafely(null));

            //empty node
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest></doubleTest>");
                var testNode = testDocument["doubleTest"];
                Assert.IsNull((XmlParsingUtils.GetDoubleSafely(testNode)));
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>abcd</doubleTest>");
                var testNode = testDocument["doubleTest"];
                Assert.IsNull((XmlParsingUtils.GetDoubleSafely(testNode)));
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1.-2</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetDoubleSafely(testNode);
                Assert.IsNull(result);
            }

            //right format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetDoubleSafely(testNode);
                Assert.IsNotNull(result);
                Assert.AreEqual((int)-1, (int)result);
            }

            //right format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1.2</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetDoubleSafely(testNode);
                Assert.IsNotNull(result);
                Assert.AreEqual((decimal)-1.2, (decimal)result);
            }

        }

        [Test]
        public void GetDecimalSafely_Test()
        {
            Assert.IsNull(XmlParsingUtils.GetDecimalSafely(null));

            //empty node
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest></doubleTest>");
                var testNode = testDocument["doubleTest"];
                Assert.IsNull((XmlParsingUtils.GetDecimalSafely(testNode)));
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>abcd</doubleTest>");
                var testNode = testDocument["doubleTest"];
                Assert.IsNull((XmlParsingUtils.GetDecimalSafely(testNode)));
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1.-2</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetDecimalSafely(testNode);
                Assert.IsNull(result);
            }

            //right format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetDecimalSafely(testNode);
                Assert.IsNotNull(result);
                Assert.AreEqual((int)-1, (int)result);
            }

            //right format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1.2</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetDecimalSafely(testNode);
                Assert.IsNotNull(result);
                Assert.AreEqual((decimal)-1.2, (decimal)result);
            }

        }

        [Test]
        public void GetMoneySafely_Test()
        {
            Assert.IsNull(XmlParsingUtils.GetMoneySafely(null, null));

            //empty node
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest></doubleTest>");
                var testNode = testDocument["doubleTest"];
                Assert.IsNull((XmlParsingUtils.GetMoneySafely(testNode, ".")));
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>abcd</doubleTest>");
                var testNode = testDocument["doubleTest"];
                Assert.IsNull((XmlParsingUtils.GetMoneySafely(testNode, ".")));
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1.-2</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetMoneySafely(testNode, ".");
                Assert.IsNull(result);
            }

            //wrong format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1.2 PLN</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetMoneySafely(testNode, ".");
                Assert.IsNull(result);
            }

            //right format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetMoneySafely(testNode, ".");
                Assert.IsNotNull(result);
                Assert.AreEqual((int)-1, (int)result);
            }

            //right format
            {
                var testDocument = new XmlDocument();
                testDocument.LoadXml("<doubleTest>-1.2</doubleTest>");
                var testNode = testDocument["doubleTest"];
                var result = XmlParsingUtils.GetMoneySafely(testNode, ".");
                Assert.IsNotNull(result);
                Assert.AreEqual((decimal)-1.2, (decimal)result);
            }

        }


    }
}
