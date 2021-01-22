using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albelli.Lambda.Templates.Sns
{
    public class SnsMessageRouter
    {
        private readonly Dictionary<Type, string> _entityToPathCustomMap = new Dictionary<Type, string>();
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly Lazy<Dictionary<Type, string>> _lazyAutoMapping;

        public SnsMessageRouter(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _lazyAutoMapping = new Lazy<Dictionary<Type, string>>(GenerateAutoMapping, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public SnsMessageRouter AddMapping<TEntity>(string path)
        {
            _entityToPathCustomMap.TryAdd(typeof(TEntity), path);
            return this;
        }

        public string GetPath<TEntity>()
        {
            var type = typeof(TEntity);
            if (_entityToPathCustomMap.ContainsKey(type))
            {
                return _entityToPathCustomMap[type];
            }

            if (_lazyAutoMapping.Value.ContainsKey(type))
            {
                return _lazyAutoMapping.Value[type];
            }

            throw new InvalidOperationException($"Handler for entity {type} not found. Please ensure that you have POST method with body of type {type} to handle it or set custom path via {nameof(AddMapping)} method");
        }

        private Dictionary<Type, string> GenerateAutoMapping()
        {
            var result = new Dictionary<Type, string>();
            var actions = _actionDescriptorCollectionProvider.ActionDescriptors.Items;

            foreach (var action in actions)
            {
                var httpMethods = GetHttpMethods(action);

                if (!httpMethods.Any(x => string.Equals(x, "POST", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var bodyParameter = GetBodyParameter(action);
                if (bodyParameter == null)
                {
                    continue;
                }

                var path = GetPath(action);
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                if (result.ContainsKey(bodyParameter.ParameterType))
                {
                    throw new InvalidOperationException($"Mapping for type {bodyParameter.ParameterType} already set to path '{result[bodyParameter.ParameterType]}'. Therefore it can't be set to {path}");
                }

                result.Add(bodyParameter.ParameterType, path);
            }

            return result;
        }

        [CanBeNull]
        private static ParameterDescriptor GetBodyParameter(ActionDescriptor action)
        {
            var bodyParameters = action.Parameters
                .Where(x => x.BindingInfo.BindingSource == BindingSource.Body)
                .ToArray();

            if (bodyParameters.Length == 0)
            {
                return null;
            }

            if (bodyParameters.Length > 1)
            {
                throw new InvalidOperationException($"Action {action.DisplayName} has two or more parameters with [FromBody] attribute");
            }

            return bodyParameters[0];
        }

        private static string[] GetHttpMethods(ActionDescriptor action)
        {
            return action.ActionConstraints?
                       .OfType<HttpMethodActionConstraint>()
                       .SelectMany(c => c.HttpMethods)
                       .ToArray()
                   ?? new string[0];
        }

        [CanBeNull]
        private static string GetPath(ActionDescriptor action)
        {
            if (action is PageActionDescriptor pageActionDescriptor)
            {
                return pageActionDescriptor.ViewEnginePath;
            }

            if (action.AttributeRouteInfo != null)
            {
                return $"/{action.AttributeRouteInfo.Template}";
            }

            if (action is ControllerActionDescriptor controllerActionDescriptor)
            {
                return $"/{controllerActionDescriptor.ControllerName}/{controllerActionDescriptor.ActionName}";
            }

            return null;
        }
    }
}