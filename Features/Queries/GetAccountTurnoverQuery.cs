using CQRSMediatr.Interfaces;
using EventSourcing.Models;

namespace EventSourcing.Features.Queries;

//TODO: Need to implement this query
public class GetAccountTurnoverQuery: IQuery<IEnumerable<AccountEventDto>>
{
}
