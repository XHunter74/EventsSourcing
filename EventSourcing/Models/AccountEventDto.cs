using EventSourcing.Data;

namespace EventSourcing.Models;

public record AccountEventDto(DateTime Date, EventType Type, decimal Amount);
