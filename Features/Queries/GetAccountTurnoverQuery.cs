using CQRSMediatr.Interfaces;
using EventSourcing.Models;

namespace EventSourcing.Features.Queries;

public class GetAccountTurnoverQuery: IQuery<IEnumerable<AccountEventDto>>
{
}
