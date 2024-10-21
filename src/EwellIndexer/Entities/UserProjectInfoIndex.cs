using AeFinder.Sdk.Entities;
using Nest;

namespace EwellIndexer.Entities;

public class UserProjectInfoIndex : UserProjectInfoBase, IAeFinderEntity
{
    [Keyword] public string CrowdfundingProjectId  { get; set; }
    public CrowdfundingProjectBase CrowdfundingProject { get; set; }
}