// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

var builder = DistributedApplication.CreateBuilder(args);
builder.AddAzureContainerAppEnvironment("env");

var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini");

builder.AddProject<Projects.GitHubModelsEndToEnd_WebStory>("webstory")
       .WithExternalHttpEndpoints()
       .WithReference(chat).WaitFor(chat);

#if !SKIP_DASHBOARD_REFERENCE
// This project is only added in playground projects to support development/debugging
// of the dashboard. It is not required in end developer code. Comment out this code
// or build with `/p:SkipDashboardReference=true`, to test end developer
// dashboard launch experience, Refer to Directory.Build.props for the path to
// the dashboard binary (defaults to the Aspire.Dashboard bin output in the
// artifacts dir).
builder.AddProject<Projects.Aspire_Dashboard>(KnownResourceNames.AspireDashboard);
#endif

builder.Build().Run();
