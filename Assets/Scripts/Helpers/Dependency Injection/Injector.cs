using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Helpers.Dependency_Injection
{
    [DefaultExecutionOrder(-1000)]
    public class Injector : Singleton<Injector>
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;

        private readonly Dictionary<Type, object> _registry = new();

        protected override void Awake()
        {
            base.Awake();
            
            var monoBehaviours = FindMonoBehaviours();
            
            // Find all modules implementing IDependencyProvider and register the dependencies they provide

            var providers = monoBehaviours.OfType<IDependencyProvider>();
            
            foreach (var provider in providers)
            {
                Register(provider);
            }
            
            // Find all injectable objects and inject their dependencies
            var injectables = monoBehaviours.Where(IsInjectable);
            
            foreach (var injectable in injectables)
            {
                Inject(injectable);
            }
        }

        /// <summary>
        /// Registers all dependencies provided by the given provider.
        /// </summary>
        /// <param name="provider">The provider to register dependencies from.</param>
        /// <exception cref="Exception">Thrown when a dependency is null.</exception>
        private void Register(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(BindingFlags);

            foreach (var method in methods)
            {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;
                
                var returnType = method.ReturnType;
                var providedInstance = method.Invoke(provider, null);
                if (providedInstance != null)
                {
                    _registry.Add(returnType, providedInstance);
                }
                else
                {
                    throw new Exception($"Provider method {method.Name} in class {provider.GetType().Name} returned null when providing type '{returnType.Name}.");
                }

            }
        }
        
        /// <summary>
        /// Finds all mono behaviours in the scene.
        /// </summary>
        /// <returns>An array of all mono behaviours in the scene.</returns>
        private static MonoBehaviour[] FindMonoBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }

        /// <summary>
        /// Checks if the given object is injectable.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is injectable, false otherwise.</returns>
        private static bool IsInjectable(MonoBehaviour obj)
        {
            var members = obj.GetType().GetMembers(BindingFlags);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }

        /// <summary>
        /// Resolves the given type from the registry.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The resolved instance.</returns>
        public object Resolve(Type type)
        {
            _registry.TryGetValue(type, out var resolvedInstance);
            return resolvedInstance;
        }

        /// <summary>
        /// Injects all dependencies into the given instance.
        /// </summary>
        /// <param name="instance">The instance to inject dependencies into.</param>
        /// <exception cref="Exception">Thrown when a dependency is null.</exception>
        private void Inject(object instance)
        {
            var type = instance.GetType();
            
            // Inject fields
            var injectableFields = type.GetFields(BindingFlags).Where(field => Attribute.IsDefined(field, typeof(InjectAttribute)));

            foreach (var injectableField in injectableFields)
            {
                var fieldType = injectableField.FieldType;
                var resolvedInstance = Resolve(fieldType);

                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to resolve dependency of type '{fieldType.Name}' for field '{injectableField.Name}' in class '{type.Name}'.");
                }
                
                injectableField.SetValue(instance, resolvedInstance);
            }
            
            // Inject methods
            var injectableMethods = type.GetMethods(BindingFlags).Where(method => Attribute.IsDefined(method, typeof(InjectAttribute)));

            foreach (var injectableMethod in injectableMethods)
            {
                var requiredParameters = injectableMethod.GetParameters().Select(parameter => parameter.ParameterType)
                    .ToArray();
                
                var resolvedInstances = requiredParameters.Select(Resolve).ToArray();
                if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
                {
                    throw new Exception($"Failed to inject dependencies for method '{injectableMethod.Name}' in class '{type.Name}'.");
                }
                
                injectableMethod.Invoke(instance, resolvedInstances);
            }
            
            // Inject properties
            var injectableProperties = type.GetProperties(BindingFlags).Where(property => Attribute.IsDefined(property, typeof(InjectAttribute)));
            
            foreach (var injectableProperty in injectableProperties)
            {
                var propertyType = injectableProperty.PropertyType;
                var resolvedInstance = Resolve(propertyType);
                
                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to inject dependency into property '{injectableProperty.Name}' in class '{type.Name}'.");
                }
                
                injectableProperty.SetValue(instance, resolvedInstance);
            }
        }
    }
}
