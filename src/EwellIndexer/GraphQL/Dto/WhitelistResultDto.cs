using System.Collections.Generic;

namespace EwellIndexer.GraphQL.Dto;

public class WhitelistResultDto
{
    public long TotalCount { get; set; }
    public List<WhitelistDto> Data { get; set; }
}