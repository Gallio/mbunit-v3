// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;

using MbUnit.Framework;

[TestFixture]
public class TestFixtureSetUpAndTearDown_Test
{
    private int testFixtureSetUpCallCount = 0;
    private int setUpCallCount = 0;
    private int tearDownCallCount = 0;
    private int testCount = 0;
    private int testFixtureTearDownCallCount = 0;

    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
        this.testFixtureSetUpCallCount++;

        Assert.AreEqual(1, this.testFixtureSetUpCallCount);
        Assert.AreEqual(0, this.testFixtureTearDownCallCount);
        Assert.AreEqual(0, this.setUpCallCount);
        Assert.AreEqual(0, this.tearDownCallCount);
        Assert.AreEqual(0, this.testCount);

    }

    [SetUp]
    public void SetUp()
    {
        this.setUpCallCount++;

        Assert.AreEqual(1, this.testFixtureSetUpCallCount);
        Assert.AreEqual(0, this.testFixtureTearDownCallCount);
        Assert.AreEqual(this.setUpCallCount - 1, this.tearDownCallCount);
        Console.WriteLine(this);
    }

    [Test]
    public void Test1()
    {
        this.testCount++;

        Assert.AreEqual(1, this.testFixtureSetUpCallCount);
        Assert.AreEqual(0, this.testFixtureTearDownCallCount);

        Assert.AreEqual(this.testCount, this.setUpCallCount);
        Assert.AreEqual(this.setUpCallCount - 1, this.tearDownCallCount);
        Console.WriteLine(this);
    }

    [Test]
    public void Test2()
    {
        this.testCount++;

        Assert.AreEqual(1, this.testFixtureSetUpCallCount);
        Assert.AreEqual(0, this.testFixtureTearDownCallCount);

        Assert.AreEqual(this.testCount, this.setUpCallCount);
        Assert.AreEqual(this.setUpCallCount - 1, this.tearDownCallCount);
        Console.WriteLine(this);
    }

    [TearDown]
    public void TearDown()
    {
        this.tearDownCallCount++;

        Assert.AreEqual(1, this.testFixtureSetUpCallCount);
        Assert.AreEqual(0, this.testFixtureTearDownCallCount);
        Assert.AreEqual(this.testCount, this.setUpCallCount);
        Assert.AreEqual(this.setUpCallCount, this.tearDownCallCount);
        Console.WriteLine(this);
    }

    [TestFixtureTearDown]
    public void TestFixtureDown()
    {
        testFixtureTearDownCallCount++;

        Assert.AreEqual(1, this.testFixtureSetUpCallCount);
        Assert.AreEqual(1, this.testFixtureTearDownCallCount);
        Assert.AreEqual(2, this.setUpCallCount);
        Assert.AreEqual(2, this.testCount);
        Assert.AreEqual(2, this.tearDownCallCount);
    }

    public override string ToString()
    {
        System.IO.StringWriter sw = new System.IO.StringWriter();
        sw.WriteLine("TestFixtureSetUp: {0}", this.testFixtureSetUpCallCount);
        sw.WriteLine("SetUp: {0}", this.setUpCallCount);
        sw.WriteLine("Tests: {0}", this.testCount);
        sw.WriteLine("TearDown: {0}", this.tearDownCallCount);
        sw.WriteLine("TestFixtureTearDown: {0}", this.testFixtureTearDownCallCount);

        return sw.ToString();
    }
}
