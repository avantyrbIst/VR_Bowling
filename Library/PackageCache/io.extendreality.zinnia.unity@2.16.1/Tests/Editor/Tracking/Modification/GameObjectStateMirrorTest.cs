﻿using Zinnia.Data.Collection.List;
using Zinnia.Tracking.Modification;

namespace Test.Zinnia.Tracking.Modification
{
    using NUnit.Framework;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.TestTools;

    public class GameObjectStateMirrorTest
    {
        private GameObject containingObject;
        private GameObjectStateMirror subject;

        [SetUp]
        public void SetUp()
        {
            containingObject = new GameObject("GameObjectStateMirrorTest");
            subject = containingObject.AddComponent<GameObjectStateMirror>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(containingObject);
        }

        [UnityTest]
        public IEnumerator ActivateTargets()
        {
            GameObject source = new GameObject("GameObjectStateMirrorTest");
            GameObject target1 = new GameObject("GameObjectStateMirrorTest");
            GameObject target2 = new GameObject("GameObjectStateMirrorTest");
            GameObject target3 = new GameObject("GameObjectStateMirrorTest");

            subject.Sources = containingObject.AddComponent<GameObjectObservableList>();
            subject.Targets = containingObject.AddComponent<GameObjectObservableList>();
            yield return null;

            subject.Sources.Add(source);
            subject.Targets.Add(target1);
            subject.Targets.Add(target2);
            subject.Targets.Add(target3);

            source.gameObject.SetActive(true);

            target1.gameObject.SetActive(true);
            target2.gameObject.SetActive(false);
            target3.gameObject.SetActive(false);

            Assert.IsTrue(source.gameObject.activeInHierarchy);
            Assert.IsTrue(target1.gameObject.activeInHierarchy);
            Assert.IsFalse(target2.gameObject.activeInHierarchy);
            Assert.IsFalse(target3.gameObject.activeInHierarchy);

            subject.Process();

            Assert.IsTrue(source.gameObject.activeInHierarchy);
            Assert.IsTrue(target1.gameObject.activeInHierarchy);
            Assert.IsTrue(target2.gameObject.activeInHierarchy);
            Assert.IsTrue(target3.gameObject.activeInHierarchy);

            Object.DestroyImmediate(source);
            Object.DestroyImmediate(target1);
            Object.DestroyImmediate(target2);
            Object.DestroyImmediate(target3);
        }

        [UnityTest]
        public IEnumerator DeactivateTargets()
        {
            GameObject source = new GameObject("GameObjectStateMirrorTest");
            GameObject target1 = new GameObject("GameObjectStateMirrorTest");
            GameObject target2 = new GameObject("GameObjectStateMirrorTest");
            GameObject target3 = new GameObject("GameObjectStateMirrorTest");

            subject.Sources = containingObject.AddComponent<GameObjectObservableList>();
            subject.Targets = containingObject.AddComponent<GameObjectObservableList>();
            yield return null;

            subject.Sources.Add(source);
            subject.Targets.Add(target1);
            subject.Targets.Add(target2);
            subject.Targets.Add(target3);

            source.gameObject.SetActive(false);

            target1.gameObject.SetActive(true);
            target2.gameObject.SetActive(false);
            target3.gameObject.SetActive(true);

            Assert.IsFalse(source.gameObject.activeInHierarchy);
            Assert.IsTrue(target1.gameObject.activeInHierarchy);
            Assert.IsFalse(target2.gameObject.activeInHierarchy);
            Assert.IsTrue(target3.gameObject.activeInHierarchy);

            subject.Process();

            Assert.IsFalse(source.gameObject.activeInHierarchy);
            Assert.IsFalse(target1.gameObject.activeInHierarchy);
            Assert.IsFalse(target2.gameObject.activeInHierarchy);
            Assert.IsFalse(target3.gameObject.activeInHierarchy);

            Object.DestroyImmediate(source);
            Object.DestroyImmediate(target1);
            Object.DestroyImmediate(target2);
            Object.DestroyImmediate(target3);
        }

        [UnityTest]
        public IEnumerator ActivateThenDeactivateTargets()
        {
            GameObject source = new GameObject("GameObjectStateMirrorTest");
            GameObject target1 = new GameObject("GameObjectStateMirrorTest");
            GameObject target2 = new GameObject("GameObjectStateMirrorTest");
            GameObject target3 = new GameObject("GameObjectStateMirrorTest");

            subject.Sources = containingObject.AddComponent<GameObjectObservableList>();
            subject.Targets = containingObject.AddComponent<GameObjectObservableList>();
            yield return null;

            subject.Sources.Add(source);
            subject.Targets.Add(target1);
            subject.Targets.Add(target2);
            subject.Targets.Add(target3);

            source.gameObject.SetActive(true);

            target1.gameObject.SetActive(true);
            target2.gameObject.SetActive(false);
            target3.gameObject.SetActive(false);

            Assert.IsTrue(source.gameObject.activeInHierarchy);
            Assert.IsTrue(target1.gameObject.activeInHierarchy);
            Assert.IsFalse(target2.gameObject.activeInHierarchy);
            Assert.IsFalse(target3.gameObject.activeInHierarchy);

            subject.Process();

            Assert.IsTrue(source.gameObject.activeInHierarchy);
            Assert.IsTrue(target1.gameObject.activeInHierarchy);
            Assert.IsTrue(target2.gameObject.activeInHierarchy);
            Assert.IsTrue(target3.gameObject.activeInHierarchy);

            source.gameObject.SetActive(false);

            subject.Process();

            Assert.IsFalse(source.gameObject.activeInHierarchy);
            Assert.IsFalse(target1.gameObject.activeInHierarchy);
            Assert.IsFalse(target2.gameObject.activeInHierarchy);
            Assert.IsFalse(target3.gameObject.activeInHierarchy);

            Object.DestroyImmediate(source);
            Object.DestroyImmediate(target1);
            Object.DestroyImmediate(target2);
            Object.DestroyImmediate(target3);
        }
    }
}