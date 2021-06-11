using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace JMTools.Tests
{
    [TestFixture, TestOf(typeof(GameObjectExtensions))]
    public class GameObjectExtensions_Tests
    {

        [Test]
        public void RequiresComponent_PositiveTest()
        {
            GameObject obj = new GameObject();
            C1 c1 = obj.AddComponent<C1>();
            C2 c2 = obj.AddComponent<C2>();
            
            List<Component> dependants = obj.GetComponentDependants<C1>();
            
            Assert.That(dependants, Has.Member(c2));
            Assert.That(dependants, Has.No.Member(c1));
            Assert.That(dependants, Has.No.Member(obj.transform));
        }
        
        [Test]
        public void RequiresComponent_NegativeTest()
        {
            GameObject obj = new GameObject();
            C1 c1 = obj.AddComponent<C1>();
            C2 c2 = obj.AddComponent<C2>();
            
            List<Component> dependants = obj.GetComponentDependants<C2>();
            Assert.IsEmpty(dependants);
        }
        
        public class C1 : MonoBehaviour
        { }
        
        [RequireComponent(typeof(C1))]
        public class C2 : MonoBehaviour
        { }
    }
}
