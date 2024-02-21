namespace cleancontrol_backend.Schemas;

public record RoomRefill(InventoryItem item, Room room, int quantity, DateTime refillTime);
