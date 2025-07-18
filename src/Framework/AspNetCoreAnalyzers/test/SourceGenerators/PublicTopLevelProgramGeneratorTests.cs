// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using VerifyCS = Microsoft.AspNetCore.Analyzers.Verifiers.CSharpSourceGeneratorVerifier<Microsoft.AspNetCore.SourceGenerators.PublicProgramSourceGenerator>;

namespace Microsoft.AspNetCore.SourceGenerators.Tests;

public class PublicTopLevelProgramGeneratorTests
{
    private const string ExpectedGeneratedSource = """
// <auto-generated />
/// <summary>
/// Auto-generated public partial Program class for top-level statement apps.
/// </summary>
public partial class Program { }
""";

    [Fact]
    public async Task GeneratesSource_ProgramWithTopLevelStatements()
    {
        var source = """
using Microsoft.AspNetCore.Builder;

var app = WebApplication.Create();

app.MapGet("/", () => "Hello, World!");

app.Run();
""";

        await VerifyCS.VerifyAsync(source, "PublicTopLevelProgram.Generated.g.cs", ExpectedGeneratedSource);
    }

    // The compiler synthesizes a Program class in the global namespace due to top-level statements
    // The Foo.Program class is completely unrelated to the entry point and is just as any regular type
    // Hence, we will expect to see the source generated in these scenarios
    [Theory]
    [InlineData("public partial class Program { }")]
    [InlineData("internal partial class Program { }")]
    public async Task GeneratesSource_IfProgramDefinedInANamespace (string declaration)
    {
        var source = $$"""
using Microsoft.AspNetCore.Builder;

var app = WebApplication.Create();

app.MapGet("/", () => "Hello, World!");

app.Run();

namespace Foo
{
    {{declaration}}
}
""";

        await VerifyCS.VerifyAsync(source, "PublicTopLevelProgram.Generated.g.cs", ExpectedGeneratedSource);
    }

    [Fact]
    public async Task DoesNotGeneratesSource_IfProgramIsAlreadyPublic()
    {
        var source = """
using Microsoft.AspNetCore.Builder;

var app = WebApplication.Create();

app.MapGet("/", () => "Hello, World!");

app.Run();

public partial class Program { }
""";

        await VerifyCS.VerifyAsync(source);
    }

    [Fact]
    public async Task DoesNotGeneratesSource_IfProgramDeclaresExplicitInternalAccess()
    {
        var source = """
using Microsoft.AspNetCore.Builder;

var app = WebApplication.Create();

app.MapGet("/", () => "Hello, World!");

app.Run();

internal partial class Program { }
""";

        await VerifyCS.VerifyAsync(source);
    }

    [Fact]
    public async Task DoesNotGeneratorSource_ExplicitPublicProgramClass()
    {
        var source = """
using Microsoft.AspNetCore.Builder;

public class Program
{
    public static void Main()
    {
        var app = WebApplication.Create();

        app.MapGet("/", () => "Hello, World!");

        app.Run();
    }
}
""";

        await VerifyCS.VerifyAsync(source);
    }

    [Fact]
    public async Task DoesNotGeneratorSource_ExplicitPublicProgramClassInNamespace()
    {
        var source = """
using Microsoft.AspNetCore.Builder;

namespace Foo
{
    public class Program
    {
        public static void Main()
        {
            var app = WebApplication.Create();

            app.MapGet("/", () => "Hello, World!");

            app.Run();
        }
    }
}
""";

        await VerifyCS.VerifyAsync(source);
    }

    [Fact]
    public async Task DoesNotGeneratorSource_ExplicitInternalProgramClass()
    {
        var source = """
using Microsoft.AspNetCore.Builder;

internal class Program
{
    public static void Main()
    {
        var app = WebApplication.Create();

        app.MapGet("/", () => "Hello, World!");

        app.Run();
    }
}
""";

        await VerifyCS.VerifyAsync(source);
    }

    [Fact]
    public async Task DoesNotGeneratorSource_ExplicitInternalProgramClassInNamespace()
    {
        var source = """
using Microsoft.AspNetCore.Builder;

namespace Foo
{
    internal class Program
    {
        public static void Main()
        {
            var app = WebApplication.Create();

            app.MapGet("/", () => "Hello, World!");

            app.Run();
        }
    }
}
""";

        await VerifyCS.VerifyAsync(source);
    }

    [Theory]
    [InlineData("interface")]
    [InlineData("struct")]
    public async Task DoesNotGeneratorSource_ExplicitInternalProgramType(string type)
    {
        var source = $$"""
using Microsoft.AspNetCore.Builder;

internal {{type}} Program
{
    public static void Main(string[] args)
    {
        var app = WebApplication.Create();

        app.MapGet("/", () => "Hello, World!");

        app.Run();
    }
}
""";

        await VerifyCS.VerifyAsync(source);
    }

    [Theory]
    [InlineData("interface")]
    [InlineData("struct")]
    public async Task DoesNotGeneratorSource_ExplicitInternalProgramTypeInNamespace(string type)
    {
        var source = $$"""
using Microsoft.AspNetCore.Builder;

namespace Foo
{
    internal {{type}} Program
    {
        public static void Main(string[] args)
        {
            var app = WebApplication.Create();

            app.MapGet("/", () => "Hello, World!");

            app.Run();
        }
    }
}
""";

        await VerifyCS.VerifyAsync(source);
    }
}
