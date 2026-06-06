namespace CodeCafe.Host;

/// <summary>
/// Marker type used by integration tests to host the composition root
/// without referencing the implicit <c>Program</c> class (which collides
/// with the same implicit class in <c>CodeCafe.Mcp</c>). The test
/// factory uses this type as the <c>WebApplicationFactory&lt;T&gt;</c>
/// entry point, so the type itself does not have to do anything.
/// </summary>
public sealed class HostEntryPointMarker
{
}
