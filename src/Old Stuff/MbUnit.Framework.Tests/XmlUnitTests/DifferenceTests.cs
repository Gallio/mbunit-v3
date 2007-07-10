namespace MbUnit.Tests.XmlUnit {
    using System;
    using MbUnit.Framework;
    using MbUnit.Framework.Xml;
    
    [TestFixture]
    public class DifferenceTests {
        private Difference minorDifference;
        
        [SetUp] public void CreateMinorDifference() {
            DifferenceType id = DifferenceType.AttributeSequence;
            Assert.AreEqual(false, Differences.isMajorDifference(id));
            minorDifference = new Difference(id);
        }
        
        [Test] public void ToStringContainsId() {
            string commentDifference = minorDifference.ToString();
            string idValue = "type: " + (int)DifferenceType.AttributeSequence;
            Assert.AreEqual( 
                            true, 
                            commentDifference.IndexOfAny(idValue.ToCharArray()) > 0,
							"contains " + idValue);
        }
    }
}
