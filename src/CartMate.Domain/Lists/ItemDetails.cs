namespace CartMate.Domain.Lists;

/// <summary>The user-editable fields of a grocery item, grouped to keep method signatures short.</summary>
public sealed record ItemDetails(string Name, int Quantity, string? Category, string? Note);
