#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace JMTools
{
    public static class GameObjectExtensions
    {

        /// <summary>
        /// Given a <see cref="GameObject"/>'s <paramref name="transform"/> will calculate the <see cref="Bounds"/>
        /// of the union of all <see cref="Renderer"/> <see cref="Component"/> attached to the <param name="transform"/> and its children.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>The bounds of all child renderers in <paramref name="transform"/></returns>
        public static Bounds CalculateRendererBounds(this Transform transform)
        {
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(Vector3.zero);

            Bounds bounds = new Bounds(transform.position, Vector3.one);

            foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            Vector3 localCenter = bounds.center - transform.position;
            bounds.center = localCenter;

            transform.rotation = currentRotation;


            return bounds;
        }

        public static Bounds CalculateRendererBounds(this GameObject gameObject)
            => CalculateRendererBounds(gameObject.transform);

        public static Bounds CalculateRendererBounds(this Component component)
            => CalculateRendererBounds(component.transform);


        public static List<Component> GetComponentDependants<T>(this GameObject gameObject) where T : Component
            => GetComponentDependants<T>(gameObject.transform);

        /// <summary>
        /// Returns a list of <see cref="Component"/>s that have the <see cref="RequireComponent"/> attribute with the type <typeparam name="T"/>
        /// </summary>
        /// <param name="transform"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Component> GetComponentDependants<T>(this Transform transform) where T : Component
        {
            List<Component> dependants = new List<Component>();
            Type target = typeof(T);
            foreach (Component component in transform.GetComponents<Component>())
            {
                RequireComponent? attribute = component.GetType().GetCustomAttribute<RequireComponent>();

                if (attribute != null
                    && !DoesntRequire(attribute, target))
                    dependants.Add(component);
            }

            return dependants;

            static bool DoesntRequire(RequireComponent attribute, Type target)
            {
                return attribute.m_Type0 != target
                       && attribute.m_Type1 != target
                       && attribute.m_Type2 != target;
            }
        }

    }
}
