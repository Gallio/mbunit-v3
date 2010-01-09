// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Common.Collections;
using Gallio.Tests;
using MbUnit.Framework;

using Gallio.Model.Filters;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(FilterSet<object>))]
    public class FilterSetTest : BaseTestWithMocks
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SimpleConstructor_FilterIsNull_Throws()
        {
            new FilterSet<object>((Filter<object>)null);
        }

        [Test]
        public void SimpleConstructor_WhenSuccessful_InitializesProperties()
        {
            var filter = new AnyFilter<object>();
            var filterSet = new FilterSet<object>(filter);

            Assert.AreEqual(1, filterSet.Rules.Count);
            Assert.AreEqual(FilterRuleType.Inclusion, filterSet.Rules[0].RuleType);
            Assert.AreEqual(filter, filterSet.Rules[0].Filter);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RulesConstructor_WhenRulesCollectionIsNull_Throws()
        {
            new FilterSet<object>((FilterRule<object>[])null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RulesConstructor_WhenRulesCollectionContainsNull_Throws()
        {
            new FilterSet<object>(new FilterRule<object>[] { null });
        }

        [Test]
        public void RulesConstructor_WhenSuccessful_InitializesProperties()
        {
            var filterRules = new[] { new FilterRule<object>(FilterRuleType.Exclusion, new AnyFilter<object>()), new FilterRule<object>(FilterRuleType.Inclusion, new NoneFilter<object>()) };
            var filterSet = new FilterSet<object>(filterRules);

            Assert.AreElementsEqual(filterRules, filterSet.Rules);
        }

        [Test]
        [Row(FilterSetResult.Unmatched, new FilterRuleType[] { }, new bool[] { })]
        [Row(FilterSetResult.Include, new FilterRuleType[] { FilterRuleType.Inclusion }, new bool[] { true })]
        [Row(FilterSetResult.Exclude, new FilterRuleType[] { FilterRuleType.Exclusion }, new bool[] { true })]
        [Row(FilterSetResult.Include, new FilterRuleType[] { FilterRuleType.Inclusion, FilterRuleType.Exclusion }, new bool[] { true, true })]
        [Row(FilterSetResult.Include, new FilterRuleType[] { FilterRuleType.Inclusion, FilterRuleType.Exclusion }, new bool[] { true, false })]
        [Row(FilterSetResult.Exclude, new FilterRuleType[] { FilterRuleType.Inclusion, FilterRuleType.Exclusion }, new bool[] { false, true })]
        [Row(FilterSetResult.Unmatched, new FilterRuleType[] { FilterRuleType.Inclusion, FilterRuleType.Exclusion }, new bool[] { false, false })]
        [Row(FilterSetResult.Exclude, new FilterRuleType[] { FilterRuleType.Exclusion, FilterRuleType.Inclusion }, new bool[] { true, true })]
        [Row(FilterSetResult.Exclude, new FilterRuleType[] { FilterRuleType.Exclusion, FilterRuleType.Inclusion }, new bool[] { true, false })]
        [Row(FilterSetResult.Include, new FilterRuleType[] { FilterRuleType.Exclusion, FilterRuleType.Inclusion }, new bool[] { false, true })]
        [Row(FilterSetResult.Unmatched, new FilterRuleType[] { FilterRuleType.Exclusion, FilterRuleType.Inclusion }, new bool[] { false, false })]
        public void Evaluate_Always_ConsidersRulesInOrder(FilterSetResult expectedResult, FilterRuleType[] ruleTypes, bool[] states)
        {
            FilterRule<object>[] filterRules = new FilterRule<object>[ruleTypes.Length];
            for (int i = 0; i < ruleTypes.Length; i++)
                filterRules[i] = new FilterRule<object>(ruleTypes[i],
                    states[i] ? (Filter<object>)new AnyFilter<object>() : new NoneFilter<object>());

            FilterSet<object> filterSet = new FilterSet<object>(filterRules);
            Assert.AreEqual(expectedResult, filterSet.Evaluate(null));
        }

        [Test]
        [Row("", new FilterRuleType[] { }, new bool[] { })]
        [Row("*", new FilterRuleType[] { FilterRuleType.Inclusion }, new bool[] { true })]
        [Row("exclude *", new FilterRuleType[] { FilterRuleType.Exclusion }, new bool[] { true })]
        [Row("* exclude *", new FilterRuleType[] { FilterRuleType.Inclusion, FilterRuleType.Exclusion }, new bool[] { true, true })]
        [Row("exclude not * include *", new FilterRuleType[] { FilterRuleType.Exclusion, FilterRuleType.Inclusion }, new bool[] { false, true })]
        public void ToFilterSetExpr_Always_FormatsRulesInOrder(string expectedResult, FilterRuleType[] ruleTypes, bool[] states)
        {
            FilterRule<object>[] filterRules = new FilterRule<object>[ruleTypes.Length];
            for (int i = 0; i < ruleTypes.Length; i++)
                filterRules[i] = new FilterRule<object>(ruleTypes[i],
                    states[i] ? (Filter<object>)new AnyFilter<object>() : new NoneFilter<object>());

            FilterSet<object> filterSet = new FilterSet<object>(filterRules);
            Assert.AreEqual(expectedResult, filterSet.ToFilterSetExpr());
        }

        [Test]
        public void Empty_Always_ReturnsAnEmptySet()
        {
            var empty = FilterSet<object>.Empty;

            Assert.AreEqual(0, empty.Rules.Count);
        }

        [Test]
        public void IsEmpty_WhenNoRules_ReturnsTrue()
        {
            var empty = FilterSet<object>.Empty;

            Assert.IsTrue(empty.IsEmpty);
        }

        [Test]
        public void IsEmpty_WhenAtLeastOneRule_ReturnsFalse()
        {
            var filterSet = new FilterSet<object>(new AnyFilter<object>());

            Assert.IsFalse(filterSet.IsEmpty);
        }

        [Test]
        public void HasInclusionRules_WhenNoRules_ReturnsFalse()
        {
            var empty = FilterSet<object>.Empty;

            Assert.IsFalse(empty.HasInclusionRules);
        }

        [Test]
        public void HasInclusionRules_WhenAtLeastOneInclusionRule_ReturnsTrue()
        {
            var filterSet = new FilterSet<object>(new AnyFilter<object>());

            Assert.IsTrue(filterSet.HasInclusionRules);
        }

        [Test]
        public void HasInclusionRules_WhenOnlyExclusionRules_ReturnsFalse()
        {
            var filterSet = new FilterSet<object>(new[] { new FilterRule<object>(FilterRuleType.Exclusion, new AnyFilter<object>()) });

            Assert.IsFalse(filterSet.HasInclusionRules);
        }
    }
}