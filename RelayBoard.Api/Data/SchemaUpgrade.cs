using Microsoft.EntityFrameworkCore;

namespace RelayBoard.Api.Data;

public static class SchemaUpgrade
{
    public static async Task ApplyAsync(RelayBoardContext db, CancellationToken cancellationToken = default)
    {
        await TryAlterAsync(db, "ALTER TABLE Orders ADD COLUMN PickupBy TEXT NOT NULL DEFAULT '2026-08-18 15:30:00'", cancellationToken);
        await TryAlterAsync(db, "ALTER TABLE Orders ADD COLUMN DeliverBy TEXT NOT NULL DEFAULT '2026-08-18 18:00:00'", cancellationToken);
        await TryAlterAsync(db, "ALTER TABLE Assignments ADD COLUMN StopSequence INTEGER NOT NULL DEFAULT 1", cancellationToken);

        await db.Database.ExecuteSqlRawAsync(
            "UPDATE Orders SET PickupBy = '2026-08-18 14:10:00', DeliverBy = '2026-08-18 14:45:00' WHERE OrderNumber = 'RB-1005'",
            cancellationToken);
        await db.Database.ExecuteSqlRawAsync(
            "UPDATE Orders SET PickupBy = '2026-08-18 13:40:00', DeliverBy = '2026-08-18 14:40:00' WHERE OrderNumber = 'RB-1007'",
            cancellationToken);
        await db.Database.ExecuteSqlRawAsync(
            "UPDATE Orders SET PickupBy = '2026-08-18 15:30:00', DeliverBy = '2026-08-18 16:30:00' WHERE OrderNumber = 'RB-1009'",
            cancellationToken);
        await db.Database.ExecuteSqlRawAsync(
            "UPDATE Assignments SET StopSequence = 1 WHERE OrderId = 7",
            cancellationToken);
        await db.Database.ExecuteSqlRawAsync(
            "UPDATE Assignments SET StopSequence = 2 WHERE OrderId = 9",
            cancellationToken);
    }

    private static async Task TryAlterAsync(
        RelayBoardContext db,
        string sql,
        CancellationToken cancellationToken)
    {
        try
        {
            await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        catch (Exception ex) when (ex.Message.Contains("duplicate column", StringComparison.OrdinalIgnoreCase))
        {
        }
    }
}
