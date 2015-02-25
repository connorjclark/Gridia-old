using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gridia;

namespace GridiaTest
{
    [TestClass]
    public class RichTextTest
    {
        private RichText _richText;

        [TestInitialize]
        public void SetUp()
        {
            _richText = new RichText();
        }

        [TestMethod]
        public void BoldParsing()
        {
            _richText.Append("Shrek is *love*");
            Assert.AreEqual("Shrek is <b>love</b>", _richText.ToString());
        }

        [TestMethod]
        public void ItalicsParsing()
        {
            _richText.Append("Shrek is **life**");
            Assert.AreEqual("Shrek is <i>life</i>", _richText.ToString());
        }

        [TestMethod]
        public void ItalicsAndBoldParsing()
        {
            _richText.Append("Shrek is ***life***");
            Assert.AreEqual("Shrek is <i><b>life</b></i>", _richText.ToString());
        }

        [TestMethod]
        public void EscapeAsterisk()
        {
            _richText.Append(@"test\*");
            Assert.AreEqual("test*", _richText.ToString());
        }

        [TestMethod]
        public void EscapeAsteriskBold()
        {
            _richText.Append(@"*\*bold\**");
            Assert.AreEqual("<b>*bold*</b>", _richText.ToString());
        }

        [TestMethod]
        public void EscapeAsteriskItalics()
        {
            _richText.Append(@"**\*\*italics\*\***");
            Assert.AreEqual("<i>**italics**</i>", _richText.ToString());
        }

        [TestMethod]
        public void EscapeAsteriskComplex()
        {
            _richText.Append(@"\*bold\* \*\*italics\*\* \*\*\*both\*\*\*");
            Assert.AreEqual("*bold* **italics** ***both***", _richText.ToString());
        }

        [TestMethod]
        public void ValidHtml()
        {
            Assert.IsTrue(RichText.HtmlIsValid("<b>This is <i>a</i> test</b>"));
            Assert.IsTrue(RichText.HtmlIsValid("<b>This is \n<i>a\n</i> test</b>"));
        }

        [TestMethod]
        public void InvalidHtml()
        {
            Assert.IsFalse(RichText.HtmlIsValid("<b>This is <i>a test</b>"));
        }

        [TestMethod]
        public void TextDoesNotGetMalformedWhenPruned()
        {
            _richText.MaxLength = 100;
            for (var i = 0; i < 10; i++)
            {
                _richText.Append("*This long string should have a high chance of getting cut off somewhere in the middle*");
            }
            Assert.IsTrue(RichText.HtmlIsValid(_richText.ToString()));
        }
    }
}
