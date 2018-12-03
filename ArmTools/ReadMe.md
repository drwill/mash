# ARM Tools

Working with Azure resources? Need to parse the resource Id to get important information from it?

This package helps parse and build resource Ids.

Take a resource Id like /subscriptions/{subscriptionId}/resourceGroups/{rgName}/providers/Microsoft.Compute/virtualMachines/vmName and process it like this:

```C#
var resource = Resource.Parse(resourceId);
Console.WriteLine($"Subscription is {resource.SubscriptionId}"); 
```

Other properties include the resource group name, provider namespace, resource type, resource name, full resource type (e.g. Microsoft.Compute/virtualMachines), ancestor names, Id, and parent resource.

## What's new?

### November 30, 2018

Initial support for top-level and nested resources. Does not yet support resource group Ids or tenant-level resources (resources without a subscription).