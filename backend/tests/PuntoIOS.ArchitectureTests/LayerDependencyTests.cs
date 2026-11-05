using System.Reflection;
using NetArchTest.Rules;

namespace PuntoIOS.ArchitectureTests;

/// <summary>Protege las reglas de la arquitectura hexagonal: las dependencias apuntan hacia adentro.</summary>
public class LayerDependencyTests
{
    private static readonly Assembly Domain = typeof(PuntoIOS.Domain.AssemblyMarker).Assembly;
    private static readonly Assembly Application = typeof(PuntoIOS.Application.AssemblyMarker).Assembly;
    private static readonly Assembly Infrastructure = typeof(PuntoIOS.Infrastructure.AssemblyMarker).Assembly;

    private static readonly string[] Frameworks =
    [
        "Microsoft.EntityFrameworkCore",
        "Npgsql",
        "FirebaseAdmin",
        "Microsoft.AspNetCore",
    ];

    [Fact]
    public void Dominio_no_depende_de_otras_capas_ni_de_frameworks()
    {
        AssertNoDependencies(Domain, ["PuntoIOS.Application", "PuntoIOS.Infrastructure", "PuntoIOS.Api", .. Frameworks]);
    }

    [Fact]
    public void Aplicacion_no_depende_de_infraestructura_api_ni_de_frameworks()
    {
        AssertNoDependencies(Application, ["PuntoIOS.Infrastructure", "PuntoIOS.Api", .. Frameworks]);
    }

    [Fact]
    public void Infraestructura_no_depende_de_la_api()
    {
        AssertNoDependencies(Infrastructure, ["PuntoIOS.Api"]);
    }

    private static void AssertNoDependencies(Assembly assembly, string[] prohibidas)
    {
        var types = Types.InAssembly(assembly);
        Assert.NotEmpty(types.GetTypes());

        var result = types.ShouldNot().HaveDependencyOnAny(prohibidas).GetResult();

        Assert.True(
            result.IsSuccessful,
            "Dependencias prohibidas en: " + string.Join(", ", result.FailingTypeNames ?? []));
    }
}
