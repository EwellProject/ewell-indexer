using System.Collections.Generic;

namespace EwellIndexer.GraphQL.Dto;

public class UserRecordResultDto
{
    public long TotalCount { get; set; }
    public List<UserRecordDto> Data { get; set; }
}