// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma warning disable ASPIRECOMPUTE001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Aspire.Hosting.Utils;
using Microsoft.AspNetCore.InternalTesting;

namespace Aspire.Hosting.Tests;

public class ResourceExtensionsTests
{
    [Fact]
    public void TryGetAnnotationsOfTypeReturnsFalseWhenNoAnnotations()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"));

        Assert.False(parent.Resource.HasAnnotationOfType<DummyAnnotation>());
        Assert.False(parent.Resource.TryGetAnnotationsOfType<DummyAnnotation>(out var annotations));
        Assert.Null(annotations);
    }

    [Fact]
    public void TryGetAnnotationsOfTypeReturnsFalseWhenOnlyAnnotationsOfOtherTypes()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"))
                            .WithAnnotation(new AnotherDummyAnnotation());

        Assert.False(parent.Resource.HasAnnotationOfType<DummyAnnotation>());
        Assert.False(parent.Resource.TryGetAnnotationsOfType<DummyAnnotation>(out var annotations));
        Assert.Null(annotations);
    }

    [Fact]
    public void TryGetAnnotationsOfTypeReturnsTrueWhenNoAnnotations()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"))
                            .WithAnnotation(new DummyAnnotation());

        Assert.True(parent.Resource.HasAnnotationOfType<DummyAnnotation>());
        Assert.True(parent.Resource.TryGetAnnotationsOfType<DummyAnnotation>(out var annotations));
        Assert.Single(annotations);
    }

    [Fact]
    public void TryGetAnnotationsIncludingAncestorsOfTypeReturnsAnnotationFromParentDirectly()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"))
                            .WithAnnotation(new DummyAnnotation());

        Assert.True(parent.Resource.HasAnnotationIncludingAncestorsOfType<DummyAnnotation>());
        Assert.True(parent.Resource.TryGetAnnotationsIncludingAncestorsOfType<DummyAnnotation>(out var annotations));
        Assert.Single(annotations);
    }

    [Fact]
    public void TryGetAnnotationIncludingAncestorsOfTypeReturnsFalseWhenNoAnnotations()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"));

        Assert.False(parent.Resource.HasAnnotationIncludingAncestorsOfType<DummyAnnotation>());
        Assert.False(parent.Resource.TryGetAnnotationsIncludingAncestorsOfType<DummyAnnotation>(out var annotations));
        Assert.Null(annotations);
    }

    [Fact]
    public void TryGetAnnotationIncludingAncestorsOfTypeReturnsFalseWhenOnlyAnnotationsOfOtherTypes()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"))
                            .WithAnnotation(new AnotherDummyAnnotation());

        Assert.False(parent.Resource.HasAnnotationIncludingAncestorsOfType<DummyAnnotation>());
        Assert.False(parent.Resource.TryGetAnnotationsIncludingAncestorsOfType<DummyAnnotation>(out var annotations));
        Assert.Null(annotations);
    }

    [Fact]
    public void TryGetAnnotationIncludingAncestorsOfTypeReturnsFalseWhenOnlyAnnotationsOfOtherTypesIncludingParent()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"))
                            .WithAnnotation(new AnotherDummyAnnotation());

        var child = builder.AddResource(new ChildResource("child", parent.Resource))
                           .WithAnnotation(new AnotherDummyAnnotation());

        Assert.False(parent.Resource.HasAnnotationIncludingAncestorsOfType<DummyAnnotation>());
        Assert.False(child.Resource.TryGetAnnotationsIncludingAncestorsOfType<DummyAnnotation>(out var annotations));
        Assert.Null(annotations);
    }

    [Fact]
    public void TryGetAnnotationsIncludingAncestorsOfTypeReturnsAnnotationFromParent()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"))
                            .WithAnnotation(new DummyAnnotation());

        var child = builder.AddResource(new ChildResource("child", parent.Resource));

        Assert.True(parent.Resource.HasAnnotationIncludingAncestorsOfType<DummyAnnotation>());
        Assert.True(child.Resource.TryGetAnnotationsIncludingAncestorsOfType<DummyAnnotation>(out var annotations));
        Assert.Single(annotations);
    }

    [Fact]
    public void TryGetAnnotationsIncludingAncestorsOfTypeCombinesAnnotationsFromParentAndChild()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"))
                            .WithAnnotation(new DummyAnnotation());

        var child = builder.AddResource(new ChildResource("child", parent.Resource))
                           .WithAnnotation(new DummyAnnotation());

        Assert.True(parent.Resource.HasAnnotationIncludingAncestorsOfType<DummyAnnotation>());
        Assert.True(child.Resource.TryGetAnnotationsIncludingAncestorsOfType<DummyAnnotation>(out var annotations));
        Assert.Equal(2, annotations.Count());
    }

    [Fact]
    public void TryGetAnnotationsIncludingAncestorsOfTypeCombinesAnnotationsFromParentAndChildAndGrandchild()
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var parent = builder.AddResource(new ParentResource("parent"))
                            .WithAnnotation(new DummyAnnotation());

        var child = builder.AddResource(new ChildResource("child", parent: parent.Resource))
                           .WithAnnotation(new DummyAnnotation());

        var grandchild = builder.AddResource(new ChildResource("grandchild", parent: child.Resource))
                                .WithAnnotation(new DummyAnnotation());

        Assert.True(parent.Resource.HasAnnotationIncludingAncestorsOfType<DummyAnnotation>());
        Assert.True(grandchild.Resource.TryGetAnnotationsIncludingAncestorsOfType<DummyAnnotation>(out var annotations));
        Assert.Equal(3, annotations.Count());
    }

    [Fact]
    public void TryGetContainerImageNameReturnsCorrectFormatWhenShaSupplied()
    {
        var builder = DistributedApplication.CreateBuilder();
        var container = builder.AddContainer("grafana", "grafana/grafana", "latest").WithImageSHA256("1adbcc2df3866ff5ec1d836e9d2220c904c7f98901b918d3cc5e1118ab1af991");

        Assert.True(container.Resource.TryGetContainerImageName(out var imageName));
        Assert.Equal("grafana/grafana@sha256:1adbcc2df3866ff5ec1d836e9d2220c904c7f98901b918d3cc5e1118ab1af991", imageName);
    }

    [Fact]
    public void TryGetContainerImageNameReturnsCorrectFormatWhenShaNotSupplied()
    {
        var builder = DistributedApplication.CreateBuilder();
        var container = builder.AddContainer("grafana", "grafana/grafana", "10.3.1");

        Assert.True(container.Resource.TryGetContainerImageName(out var imageName));
        Assert.Equal("grafana/grafana:10.3.1", imageName);
    }

    [Fact]
    public async Task GetEnvironmentVariableValuesAsyncReturnCorrectVariablesInRunMode()
    {
        var builder = DistributedApplication.CreateBuilder();
        var container = builder.AddContainer("elasticsearch", "library/elasticsearch", "8.14.0")
         .WithEnvironment("discovery.type", "single-node")
         .WithEnvironment("xpack.security.enabled", "true")
         .WithEnvironment(context =>
         {
             Assert.NotNull(context.Resource);

             context.EnvironmentVariables["ELASTIC_PASSWORD"] = "p@ssw0rd1";
         });

        var env = await container.Resource.GetEnvironmentVariableValuesAsync().DefaultTimeout();

        Assert.Collection(env,
            env =>
            {
                Assert.Equal("discovery.type", env.Key);
                Assert.Equal("single-node", env.Value);
            },
            env =>
            {
                Assert.Equal("xpack.security.enabled", env.Key);
                Assert.Equal("true", env.Value);
            },
            env =>
            {
                Assert.Equal("ELASTIC_PASSWORD", env.Key);
                Assert.Equal("p@ssw0rd1", env.Value);
            });
    }

    [Fact]
    public async Task GetEnvironmentVariableValuesAsyncReturnCorrectVariablesUsingValueProviderInRunMode()
    {
        var builder = DistributedApplication.CreateBuilder();
        builder.Configuration["Parameters:ElasticPassword"] = "p@ssw0rd1";

        var passwordParameter = builder.AddParameter("ElasticPassword");

        var container = builder.AddContainer("elasticsearch", "library/elasticsearch", "8.14.0")
         .WithEnvironment("discovery.type", "single-node")
         .WithEnvironment("xpack.security.enabled", "true")
         .WithEnvironment("ELASTIC_PASSWORD", passwordParameter);

        var env = await container.Resource.GetEnvironmentVariableValuesAsync().DefaultTimeout();

        Assert.Collection(env,
            env =>
            {
                Assert.Equal("discovery.type", env.Key);
                Assert.Equal("single-node", env.Value);
            },
            env =>
            {
                Assert.Equal("xpack.security.enabled", env.Key);
                Assert.Equal("true", env.Value);
            },
            env =>
            {
                Assert.Equal("ELASTIC_PASSWORD", env.Key);
                Assert.Equal("p@ssw0rd1", env.Value);
            });
    }

    [Fact]
    public async Task GetEnvironmentVariableValuesAsyncReturnCorrectVariablesUsingManifestExpressionProviderInPublishMode()
    {
        var builder = DistributedApplication.CreateBuilder();
        builder.Configuration["Parameters:ElasticPassword"] = "p@ssw0rd1";

        var passwordParameter = builder.AddParameter("ElasticPassword");

        var container = builder.AddContainer("elasticsearch", "library/elasticsearch", "8.14.0")
         .WithEnvironment("discovery.type", "single-node")
         .WithEnvironment("xpack.security.enabled", "true")
         .WithEnvironment("ELASTIC_PASSWORD", passwordParameter);

        var env = await container.Resource.GetEnvironmentVariableValuesAsync(DistributedApplicationOperation.Publish).DefaultTimeout();

        Assert.Collection(env,
            env =>
            {
                Assert.Equal("discovery.type", env.Key);
                Assert.Equal("single-node", env.Value);
            },
            env =>
            {
                Assert.Equal("xpack.security.enabled", env.Key);
                Assert.Equal("true", env.Value);
            },
            env =>
            {
                Assert.Equal("{ElasticPassword.value}", env.Value);
                Assert.False(string.IsNullOrEmpty(env.Value));
            });
    }

    [Fact]
    public async Task GetArgumentValuesAsync_ReturnsCorrectValuesForSpecialCases()
    {
        var builder = DistributedApplication.CreateBuilder();
        var surrogate = builder.AddResource(new ConnectionStringParameterResource("ResourceWithConnectionStringSurrogate", _ => "ConnectionString", null));
        var secretParameter = builder.AddResource(new ParameterResource("SecretParameter", _ => "SecretParameter", true));
        var nonSecretParameter = builder.AddResource(new ParameterResource("NonSecretParameter", _ => "NonSecretParameter"));

        var containerArgs = await builder.AddContainer("elasticsearch", "library/elasticsearch", "8.14.0")
            .WithArgs(surrogate)
            .WithArgs(secretParameter)
            .WithArgs(nonSecretParameter)
            .Resource.GetArgumentValuesAsync().DefaultTimeout();

        Assert.Equal<IEnumerable<string>>(["ConnectionString", "SecretParameter", "NonSecretParameter"], containerArgs);

        // Executables can also have arguments passed in AddExecutable
        var executableArgs = await builder.AddExecutable(
                "ping",
                "ping",
                string.Empty,
                surrogate,
                secretParameter,
                nonSecretParameter)
            .Resource.GetArgumentValuesAsync().DefaultTimeout();

        Assert.Equal<IEnumerable<string>>(["ConnectionString", "SecretParameter", "NonSecretParameter"], executableArgs);
    }

    [Fact]
    public void GetDeploymentTargetAnnotationWorks()
    {
        var builder = DistributedApplication.CreateBuilder();

        var compute1 = builder.AddResource(new ComputeEnvironmentResource("compute1"));
        var compute2 = builder.AddResource(new ComputeEnvironmentResource("compute2"));

        void RunTest<T>(IResourceBuilder<T> resourceBuilder) where T : IComputeResource
        {
            resourceBuilder
                .WithAnnotation(new DeploymentTargetAnnotation(compute1.Resource) { ComputeEnvironment = compute1.Resource })
                .WithAnnotation(new DeploymentTargetAnnotation(compute2.Resource) { ComputeEnvironment = compute2.Resource });

            var ex = Assert.Throws<InvalidOperationException>(() => resourceBuilder.Resource.GetDeploymentTargetAnnotation());
            Assert.Contains("'compute1, compute2'", ex.Message);

            resourceBuilder.WithComputeEnvironment(compute2);

            Assert.Equal(compute2.Resource, resourceBuilder.Resource.GetDeploymentTargetAnnotation()!.ComputeEnvironment);
        }

        RunTest(builder.AddContainer("myContainer", "nginx"));
        RunTest(builder.AddProject<Projects.ServiceA>("ServiceA"));
        RunTest(builder.AddExecutable("myExecutable", "nginx", string.Empty));
    }

    private sealed class ComputeEnvironmentResource(string name) : Resource(name), IComputeEnvironmentResource
    {
    }

    private sealed class ParentResource(string name) : Resource(name)
    {

    }

    private sealed class ChildResource(string name, Resource parent) : Resource(name), IResourceWithParent<Resource>
    {
        public Resource Parent => parent;
    }

    private sealed class DummyAnnotation : IResourceAnnotation
    {

    }

    private sealed class AnotherDummyAnnotation : IResourceAnnotation
    {

    }
}
