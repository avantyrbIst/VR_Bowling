﻿using Zinnia.Extension;

namespace Test.Zinnia.Extension
{
    using NUnit.Framework;
    using UnityEngine;

    public class ColliderExtensionsTest
    {
        [Test]
        public void GetContainingTransformWithRigidbody()
        {
            GameObject parent = new GameObject("ColliderExtensionsTest");
            GameObject child = new GameObject("ColliderExtensionsTest");
            child.transform.SetParent(parent.transform);

            parent.AddComponent<Rigidbody>();
            BoxCollider collider = child.AddComponent<BoxCollider>();

            Assert.AreEqual(parent.transform, collider.GetContainingTransform());

            Object.DestroyImmediate(child);
            Object.DestroyImmediate(parent);
        }

        [Test]
        public void GetContainingTransformWithoutRigidbody()
        {
            GameObject parent = new GameObject("ColliderExtensionsTest");
            GameObject child = new GameObject("ColliderExtensionsTest");
            child.transform.SetParent(parent.transform);

            BoxCollider collider = child.AddComponent<BoxCollider>();

            Assert.AreEqual(child.transform, collider.GetContainingTransform());

            Object.DestroyImmediate(child);
            Object.DestroyImmediate(parent);
        }

        [Test]
        public void GetContainingTransformNull()
        {
            GameObject parent = new GameObject("ColliderExtensionsTest");
            GameObject child = new GameObject("ColliderExtensionsTest");
            child.transform.SetParent(parent.transform);

            BoxCollider collider = null;

            Assert.IsNull(collider.GetContainingTransform());

            Object.DestroyImmediate(child);
            Object.DestroyImmediate(parent);
        }
    }
}