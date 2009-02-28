#pragma warning disable 618

namespace MbUnit.Tests.Compatibility.Framework.Xml {
    using System;
    using MbUnit.Framework;
    using MbUnit.Framework.Xml;
    
    [TestFixture]
    public class DifferenceTests {
        private Difference minorDifference;
        
        [SetUp] public void CreateMinorDifference() {
            DifferenceType id = DifferenceType.AttributeSequence;
            OldAssert.AreEqual(false, Differences.IsMajorDifference(id));
            minorDifference = new Difference(id);
        }
        
        [Test] public void ToStringContainsId() {
            string commentDifference = minorDifference.ToString();
            string idValue = "type: " + (int)DifferenceType.AttributeSequence;
            OldAssert.AreEqual( 
                            true, 
                            commentDifference.IndexOfAny(idValue.ToCharArray()) > 0,
							"contains " + idValue);
        }
    }
}
