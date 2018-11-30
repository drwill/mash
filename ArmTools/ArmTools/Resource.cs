using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mash.ArmTools
{
    /// <summary>
    /// Represents an Azure resource
    /// </summary>
    /// <remarks>
    /// Parses a string-representation of an Azure resource and presents the composite properties
    /// </remarks>
    public class Resource
    {
        private Resource _parent = null;

        /// <summary>
        /// The resource Id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The name of the resource
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type of the resource
        /// </summary>
        public string ResourceType { get; private set; }

        /// <summary>
        /// The fully-qualified type of the resource, including provider namespace and all parent resource types
        /// </summary>
        public string FullResourceType { get; private set; }

        /// <summary>
        /// The names of the ancestor resources
        /// </summary>
        public IReadOnlyList<string> Ancestors { get; private set; } = new string[0];

        /// <summary>
        /// Indicates if this resource is a top-level resource, or whether it has a parent resource
        /// </summary>
        public bool IsTopLevel => !Ancestors.Any();

        /// <summary>
        /// The name of the resource provider that supports this type of resource
        /// </summary>
        public string ProviderNamespace { get; private set; }

        /// <summary>
        /// The name of the resource group where the resource resides
        /// </summary>
        public string ResourceGroupName { get; private set; }

        /// <summary>
        /// The Azure subscription Id where the resource resides
        /// </summary>
        public string SubscriptionId { get; private set; }

        /// <summary>
        /// The parent resource
        /// </summary>
        public Resource Parent
        {
            get
            {
                if (IsTopLevel)
                {
                    return null;
                }

                if (_parent == null)
                {
                    string[] ancestorTypes = null;
                    string[] ancestors = null;

                    var frtParts = FullResourceType.GetParts();

                    if (Ancestors.Count > 1)
                    {
                        ancestors = Ancestors.Take(Ancestors.Count - 1).ToArray();
                        ancestorTypes = frtParts.Skip(1).Take(frtParts.Length - 3).ToArray();
                    }

                    _parent = new Resource(
                        SubscriptionId,
                        ResourceGroupName,
                        ProviderNamespace,
                        frtParts[frtParts.Length - 2], // second to last
                        Ancestors.Last(),
                        ancestorTypes,
                        ancestors);
                }

                return _parent;
            }
        }

        /// <summary>
        /// Parses the specified resource Id into a Resource object
        /// </summary>
        public static Resource Parse(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                InvalidResourceId(resourceId);
            }

            string[] parts = resourceId.GetParts();
            var resource = new Resource();

            int i = 0;

            // Handle /subscriptions/{guid}
            if (!StringComparer.OrdinalIgnoreCase.Equals(parts[i], ResourceConstants.Subscriptions)
                || ++i >= parts.Length)
            {
                InvalidResourceId(resourceId);
            }
            resource.SubscriptionId = parts[i++];

            // Handle .../resourceGroups/{rgName}
            if (i >= parts.Length
                || !StringComparer.OrdinalIgnoreCase.Equals(parts[i], ResourceConstants.ResourceGroups)
                || ++i >= parts.Length)
            {
                InvalidResourceId(resourceId);
            }
            resource.ResourceGroupName = parts[i++];

            // Handle .../providers/{providerNamespace}
            if (i >= parts.Length
                || !StringComparer.OrdinalIgnoreCase.Equals(parts[i], ResourceConstants.Providers)
                || ++i >= parts.Length)
            {
                InvalidResourceId(resourceId);
            }
            resource.ProviderNamespace = parts[i++];

            // Handle ancestor types and names
            var fullResourceType = new StringBuilder(resource.ProviderNamespace);
            var ancestors = new List<string>((parts.Length - i) / 2);
            while (i < parts.Length - 2)
            {
                fullResourceType.Append($"{ResourceConstants.ResourceIdSplitter}{parts[i++]}");
                ancestors.Add(parts[i++]);
            }
            resource.Ancestors = ancestors;

            // Handle resource type and name
            if (i + 1 >= parts.Length)
            {
                InvalidResourceId(resourceId);
            }
            resource.ResourceType = parts[i++];
            fullResourceType.Append($"{ResourceConstants.ResourceIdSplitter}{resource.ResourceType}");
            resource.Name = parts[i];

            resource.FullResourceType = fullResourceType.ToString();

            resource.Id = resource.BuildId();

            return resource;
        }

        private Resource() { }

        public Resource(string subscriptionId, string resourceGroupName, string providerNamespace, string resourceType, string resourceName, string[] ancestorTypes = null, string[] ancestorNames = null)
        {
            SubscriptionId = subscriptionId ?? throw new ArgumentException(nameof(subscriptionId));
            ResourceGroupName = resourceGroupName ?? throw new ArgumentException(nameof(subscriptionId));
            ProviderNamespace = providerNamespace ?? throw new ArgumentException(nameof(subscriptionId));
            ResourceType = resourceType ?? throw new ArgumentException(nameof(subscriptionId));
            Name = resourceName ?? throw new ArgumentException(nameof(subscriptionId));

            if (ancestorTypes?.Length != ancestorTypes?.Length)
            {
                throw new ArgumentException($"{nameof(ancestorTypes)} and {nameof(ancestorNames)} must be the same length");
            }

            if (ancestorNames != null)
            {
                Ancestors = ancestorNames;
            }

            var fullResourceType = new StringBuilder(providerNamespace);
            if (ancestorTypes != null)
            {
                foreach (string ancestorType in ancestorTypes)
                {
                    fullResourceType.Append($"{ResourceConstants.ResourceIdSplitter}{ancestorType}");
                }
            }
            fullResourceType.Append($"{ResourceConstants.ResourceIdSplitter}{resourceType}");
            FullResourceType = fullResourceType.ToString();

            Id = BuildId();
        }

        private string BuildId()
        {
            var parts = new List<string>(50)
            {
                ResourceConstants.Subscriptions,
                SubscriptionId,
                ResourceConstants.ResourceGroups,
                ResourceGroupName,
                ResourceConstants.Providers,
                ProviderNamespace,
            };

            var ancestorTypes = FullResourceType.GetParts();
            for (int i = 1; i < ancestorTypes.Length - 1; ++i)
            {
                parts.Add(ancestorTypes[i]);
                parts.Add(Ancestors[i - 1]);
            }

            parts.Add(ResourceType);
            parts.Add(Name);

            return String.Join(ResourceConstants.ResourceIdSplitterString, parts);
        }

        private static void InvalidResourceId(string resourceId)
        {
            throw new ArgumentException($"Invalid resource Id '{resourceId}'", nameof(resourceId));
        }
    }
}
