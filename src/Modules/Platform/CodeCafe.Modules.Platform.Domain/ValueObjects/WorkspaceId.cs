namespace CodeCafe.Modules.Platform.Domain.ValueObjects;

/// <summary>
/// Strong identifier for Platform workspaces. Keeps workspace ids distinct
/// from user ids while still storing as a Guid in persistence and contracts.
/// </summary>
public readonly record struct WorkspaceId(Guid Value)
{
    public static WorkspaceId New() => new(Guid.NewGuid());

    public static WorkspaceId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Workspace id is required.", nameof(value));
        }

        return new WorkspaceId(value);
    }

    public override string ToString() => Value.ToString();
}
