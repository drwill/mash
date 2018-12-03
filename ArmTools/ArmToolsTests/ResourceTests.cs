using FluentAssertions;
using System;
using Xunit;

namespace Mash.ArmTools.Tests
{
    public class ResourceTests
    {
        private static readonly string _subscriptionId = Guid.NewGuid().ToString();
        private const string _resourceGroup = "rgName";
        private const string _provider = "Microsoft.GenericProvider";
        private const string _resourceType = "customResourceType";
        private const string _resourceName = "customResourceName";
        private const string _ancestor1Type = "ancestor1Type";
        private const string _ancestor1Name = "ancestor1Name";
        private const string _ancestor2Type = "ancestor2Type";
        private const string _ancestor2Name = "ancestor2Name";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("/doesNotStartWithSubscriptions")]
        [InlineData("/subscriptions")]
        [InlineData("/subscriptions/subscriptionId/notResourceGroups")]
        [InlineData("/subscriptions/subscriptionId/resourceGroups")]
        [InlineData("/subscriptions/subscriptionId/resourceGroups/rgName")]
        [InlineData("/subscriptions/subscriptionId/resourceGroups/rgName/notProviders")]
        [InlineData("/subscriptions/subscriptionId/resourceGroups/rgName/providers")]
        [InlineData("/subscriptions/subscriptionId/resourceGroups/rgName/providers/providerNamespace")]
        [InlineData("/subscriptions/subscriptionId/resourceGroups/rgName/providers/providerNamespace/resourceType")]
        [InlineData("/subscriptions/subscriptionId/resourceGroups/rgName/providers/providerNamespace/ancestorType/ancestor/resourceType")]
        public void Resource_Parse_Failures(string resourceId)
        {
            // arrange and act
            Action parse = () => Resource.Parse(resourceId);

            // assert
            parse.Should().Throw<ArgumentException>($"[{resourceId}] should be invalid");
        }

        [Fact]
        public void Resource_Parse_TopLevelResource()
        {
            // arrange
            string[] parts = new[]
            {
                ResourceConstants.Subscriptions,
                _subscriptionId,
                ResourceConstants.ResourceGroups,
                _resourceGroup,
                ResourceConstants.Providers,
                _provider,
                _resourceType,
                _resourceName,
            };
            string input = String.Join(ResourceConstants.ResourceIdSplitter, parts);

            // act
            var resourceId = Resource.Parse(input);

            // assert
            resourceId.SubscriptionId.Should().Be(_subscriptionId);
            resourceId.ResourceGroupName.Should().Be(_resourceGroup);
            resourceId.ProviderNamespace.Should().Be(_provider);
            resourceId.ResourceType.Should().Be(_resourceType);
            resourceId.Name.Should().Be(_resourceName);
            resourceId.FullResourceType.Should().Be($"{_provider}/{_resourceType}");
            resourceId.Ancestors.Should().BeEmpty();
            resourceId.IsTopLevel.Should().BeTrue();
        }

        [Fact]
        public void Resource_Parse_NestedResource()
        {
            // arrange
            string[] parts = new[]
            {
                ResourceConstants.Subscriptions,
                _subscriptionId,
                ResourceConstants.ResourceGroups,
                _resourceGroup,
                ResourceConstants.Providers,
                _provider,
                _ancestor1Type,
                _ancestor1Name,
                _resourceType,
                _resourceName,
            };
            string input = String.Join(ResourceConstants.ResourceIdSplitter, parts);

            // act
            var resourceId = Resource.Parse(input);

            // assert
            resourceId.SubscriptionId.Should().Be(_subscriptionId);
            resourceId.ResourceGroupName.Should().Be(_resourceGroup);
            resourceId.ProviderNamespace.Should().Be(_provider);
            resourceId.ResourceType.Should().Be(_resourceType);
            resourceId.Name.Should().Be(_resourceName);
            resourceId.FullResourceType.Should().Be($"{_provider}/{_ancestor1Type}/{_resourceType}");
            resourceId.Ancestors.Should().BeEquivalentTo(new[] { _ancestor1Name });
            resourceId.IsTopLevel.Should().BeFalse();
        }

        [Fact]
        public void Resource_Parse_TopLevelParent()
        {
            // arrange
            var resourceId = new Resource(
                _subscriptionId,
                _resourceGroup,
                _provider,
                _resourceType,
                _resourceName,
                new[] { _ancestor1Type },
                new[] { _ancestor1Name });

            // act
            var parent = resourceId.Parent;

            // assert
            parent.SubscriptionId.Should().Be(_subscriptionId);
            parent.ResourceGroupName.Should().Be(_resourceGroup);
            parent.ProviderNamespace.Should().Be(_provider);
            parent.ResourceType.Should().Be(_ancestor1Type);
            parent.Name.Should().Be(_ancestor1Name);
            parent.FullResourceType.Should().Be($"{_provider}/{_ancestor1Type}");
            parent.Ancestors.Should().BeEquivalentTo(new string[0]);
            parent.IsTopLevel.Should().BeTrue();
        }

        [Fact]
        public void Resource_Parse_NestedParent()
        {
            // arrange
            var resourceId = new Resource(
                _subscriptionId,
                _resourceGroup,
                _provider,
                _resourceType,
                _resourceName,
                new[] { _ancestor1Type, _ancestor2Type },
                new[] { _ancestor1Name, _ancestor2Name });

            // act
            var parent = resourceId.Parent;

            // assert
            parent.SubscriptionId.Should().Be(_subscriptionId);
            parent.ResourceGroupName.Should().Be(_resourceGroup);
            parent.ProviderNamespace.Should().Be(_provider);
            parent.ResourceType.Should().Be(_ancestor2Type);
            parent.Name.Should().Be(_ancestor2Name);
            parent.FullResourceType.Should().Be($"{_provider}/{_ancestor1Type}/{_ancestor2Type}");
            parent.Ancestors.Should().BeEquivalentTo(new[] { _ancestor1Name });
            parent.IsTopLevel.Should().BeFalse();
        }

        [Fact]
        public void Resource_Parse_DeepResource()
        {
            // arrange
            string[] parts = new[]
            {
                ResourceConstants.Subscriptions,
                _subscriptionId,
                ResourceConstants.ResourceGroups,
                _resourceGroup,
                ResourceConstants.Providers,
                _provider,
                _ancestor1Type,
                _ancestor1Name,
                _ancestor2Type,
                _ancestor2Name,
                _resourceType,
                _resourceName,
            };
            string input = String.Join(ResourceConstants.ResourceIdSplitter, parts);

            // act
            var resourceId = Resource.Parse(input);

            // assert
            resourceId.SubscriptionId.Should().Be(_subscriptionId);
            resourceId.ResourceGroupName.Should().Be(_resourceGroup);
            resourceId.ProviderNamespace.Should().Be(_provider);
            resourceId.ResourceType.Should().Be(_resourceType);
            resourceId.Name.Should().Be(_resourceName);
            resourceId.FullResourceType.Should().Be($"{_provider}/{_ancestor1Type}/{_ancestor2Type}/{_resourceType}");
            resourceId.Ancestors.Should().BeEquivalentTo(new[] { _ancestor1Name, _ancestor2Name });
            resourceId.IsTopLevel.Should().BeFalse();
        }
    }
}
