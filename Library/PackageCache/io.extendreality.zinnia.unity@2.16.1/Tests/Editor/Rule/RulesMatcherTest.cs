﻿using Zinnia.Data.Collection.List;
using Zinnia.Rule;
using Zinnia.Rule.Collection;

namespace Test.Zinnia.Rule
{
    using NUnit.Framework;
    using System.Collections;
    using Test.Zinnia.Utility.Mock;
    using UnityEngine;
    using UnityEngine.TestTools;

    public class RulesMatcherTest
    {
        private GameObject containingObject;
        private RulesMatcher subject;

        [SetUp]
        public void SetUp()
        {
            containingObject = new GameObject("RulesMatcherTest");
            subject = containingObject.AddComponent<RulesMatcher>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(containingObject);
        }

        [UnityTest]
        public IEnumerator Match()
        {
            GameObject objectOne = new GameObject("RulesMatcherTest");
            GameObject objectTwo = new GameObject("RulesMatcherTest");
            UnityEventListenerMock ruleOneMatched = new UnityEventListenerMock();
            UnityEventListenerMock ruleTwoMatched = new UnityEventListenerMock();

            RulesMatcher.Element elementOne = new RulesMatcher.Element() { Rule = CreateRule(objectOne) };
            RulesMatcher.Element elementTwo = new RulesMatcher.Element() { Rule = CreateRule(objectTwo) };

            elementOne.Matched.AddListener(ruleOneMatched.Listen);
            elementTwo.Matched.AddListener(ruleTwoMatched.Listen);

            RulesMatcherElementObservableList elements = containingObject.AddComponent<RulesMatcherElementObservableList>();
            yield return null;
            subject.Elements = elements;

            elements.Add(elementOne);
            elements.Add(elementTwo);

            Assert.IsFalse(ruleOneMatched.Received);
            Assert.IsFalse(ruleTwoMatched.Received);

            subject.Match(objectOne);

            Assert.IsTrue(ruleOneMatched.Received);
            Assert.IsFalse(ruleTwoMatched.Received);

            ruleOneMatched.Reset();
            ruleTwoMatched.Reset();

            subject.Match(objectTwo);

            Assert.IsFalse(ruleOneMatched.Received);
            Assert.IsTrue(ruleTwoMatched.Received);

            Object.DestroyImmediate(objectOne);
            Object.DestroyImmediate(objectTwo);
        }

        [UnityTest]
        public IEnumerator MatchMultiple()
        {
            GameObject objectOne = new GameObject("RulesMatcherTest");
            GameObject objectTwo = new GameObject("RulesMatcherTest");
            UnityEventListenerMock ruleOneMatched = new UnityEventListenerMock();
            UnityEventListenerMock ruleTwoMatched = new UnityEventListenerMock();
            UnityEventListenerMock ruleThreeMatched = new UnityEventListenerMock();

            RulesMatcher.Element elementOne = new RulesMatcher.Element() { Rule = CreateRule(objectOne) };
            RulesMatcher.Element elementTwo = new RulesMatcher.Element() { Rule = CreateRule(objectTwo) };
            RulesMatcher.Element elementThree = new RulesMatcher.Element() { Rule = CreateRule(objectOne) };

            elementOne.Matched.AddListener(ruleOneMatched.Listen);
            elementTwo.Matched.AddListener(ruleTwoMatched.Listen);
            elementThree.Matched.AddListener(ruleThreeMatched.Listen);

            RulesMatcherElementObservableList elements = containingObject.AddComponent<RulesMatcherElementObservableList>();
            yield return null;
            subject.Elements = elements;

            elements.Add(elementOne);
            elements.Add(elementTwo);
            elements.Add(elementThree);

            Assert.IsFalse(ruleOneMatched.Received);
            Assert.IsFalse(ruleTwoMatched.Received);
            Assert.IsFalse(ruleThreeMatched.Received);

            subject.Match(objectOne);

            Assert.IsTrue(ruleOneMatched.Received);
            Assert.IsFalse(ruleTwoMatched.Received);
            Assert.IsTrue(ruleThreeMatched.Received);

            Object.DestroyImmediate(objectOne);
            Object.DestroyImmediate(objectTwo);
        }

        [UnityTest]
        public IEnumerator MatchInactiveGameObject()
        {
            GameObject objectOne = new GameObject("RulesMatcherTest");
            GameObject objectTwo = new GameObject("RulesMatcherTest");
            UnityEventListenerMock ruleOneMatched = new UnityEventListenerMock();
            UnityEventListenerMock ruleTwoMatched = new UnityEventListenerMock();

            RulesMatcher.Element elementOne = new RulesMatcher.Element() { Rule = CreateRule(objectOne) };
            RulesMatcher.Element elementTwo = new RulesMatcher.Element() { Rule = CreateRule(objectTwo) };

            elementOne.Matched.AddListener(ruleOneMatched.Listen);
            elementTwo.Matched.AddListener(ruleTwoMatched.Listen);

            RulesMatcherElementObservableList elements = containingObject.AddComponent<RulesMatcherElementObservableList>();
            yield return null;
            subject.Elements = elements;

            elements.Add(elementOne);
            elements.Add(elementTwo);

            subject.gameObject.SetActive(false);

            Assert.IsFalse(ruleOneMatched.Received);
            Assert.IsFalse(ruleTwoMatched.Received);

            subject.Match(objectOne);

            Assert.IsFalse(ruleOneMatched.Received);
            Assert.IsFalse(ruleTwoMatched.Received);

            Object.DestroyImmediate(objectOne);
            Object.DestroyImmediate(objectTwo);
        }

        [UnityTest]
        public IEnumerator MatchInactiveComponent()
        {
            GameObject objectOne = new GameObject("RulesMatcherTest");
            GameObject objectTwo = new GameObject("RulesMatcherTest");
            UnityEventListenerMock ruleOneMatched = new UnityEventListenerMock();
            UnityEventListenerMock ruleTwoMatched = new UnityEventListenerMock();

            RulesMatcher.Element elementOne = new RulesMatcher.Element() { Rule = CreateRule(objectOne) };
            RulesMatcher.Element elementTwo = new RulesMatcher.Element() { Rule = CreateRule(objectTwo) };

            elementOne.Matched.AddListener(ruleOneMatched.Listen);
            elementTwo.Matched.AddListener(ruleTwoMatched.Listen);

            RulesMatcherElementObservableList elements = containingObject.AddComponent<RulesMatcherElementObservableList>();
            yield return null;
            subject.Elements = elements;

            elements.Add(elementOne);
            elements.Add(elementTwo);

            subject.enabled = false;

            Assert.IsFalse(ruleOneMatched.Received);
            Assert.IsFalse(ruleTwoMatched.Received);

            subject.Match(objectOne);

            Assert.IsFalse(ruleOneMatched.Received);
            Assert.IsFalse(ruleTwoMatched.Received);

            Object.DestroyImmediate(objectOne);
            Object.DestroyImmediate(objectTwo);
        }

        protected virtual RuleContainer CreateRule(GameObject element)
        {
            RuleContainer container = new RuleContainer();
            ListContainsRule rule = containingObject.AddComponent<ListContainsRule>();
            UnityObjectObservableList objects = containingObject.AddComponent<UnityObjectObservableList>();
            rule.Objects = objects;
            objects.Add(element);
            container.Interface = rule;
            return container;
        }
    }
}