using System.Collections.Generic;
using CASC.CodeParser.Utilities;
using NUnit.Framework;

namespace CASC_Test.Tests
{
    public class MadarinTest
    {
        private decimal tmp;
        private static readonly Dictionary<string, decimal> zh_numerics = new Dictionary<string, decimal>
        {
            ["一"] = 1,
            ["壹"] = 1,
            ["二"] = 2,
            ["貳"] = 2,
            ["三"] = 3,
            ["參"] = 3,
            ["四"] = 4,
            ["肆"] = 4,
            ["五"] = 5,
            ["伍"] = 5,
            ["六"] = 6,
            ["陆"] = 6,
            ["七"] = 7,
            ["柒"] = 7,
            ["八"] = 8,
            ["捌"] = 8,
            ["九"] = 9,
            ["玖"] = 9
        };
        private static readonly Dictionary<string, decimal> zh_test_number_templates = new Dictionary<string, decimal>
        {
            ["一"] = 1,
            ["一十"] = 10,
            ["一百"] = 100,
            ["一千"] = 1000,
            ["一萬"] = 10000,
            ["一十萬"] = 100000,
            ["一百萬"] = 1000000,
            ["一千萬"] = 10000000,
            ["一億"] = 100000000
        };

        [SetUp]
        public void setUp()
        {
            tmp = 0;
        }

        [Test]
        public void NumericTest()
        {
            var numbers = GetTemplates();

            foreach (var entry in numbers)
            {
                ChineseParser.tryParseDigits(entry.Key, out tmp);
                Assert.AreEqual(tmp, entry.Value);
            }
        }

        private IEnumerable<KeyValuePair<string, decimal>> GetTemplates()
        {
            foreach (var numEntry in zh_numerics)
                foreach (var template in zh_test_number_templates)
                    yield return new KeyValuePair<string, decimal>(template.Key.Replace('一', numEntry.Key.ToCharArray()[0]), template.Value * numEntry.Value);
        }
    }
}