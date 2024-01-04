using Volo.Abp.Application.Dtos;

namespace Ewell.Indexer.Plugin.GraphQL;

public class GetDamageClaimedDto : PagedResultRequestDto
{
    public string ChainId { get; set; }
    public long StartBlockHeight { get; set; }
}