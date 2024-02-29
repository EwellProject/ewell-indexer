// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ewell_contract.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using System.Collections.Generic;
using aelf = global::AElf.CSharp.Core;

namespace Ewell.Contracts.Ido {

  #region Events
  public partial class ProjectRegistered : aelf::IEvent<ProjectRegistered>
  {
    public global::System.Collections.Generic.IEnumerable<ProjectRegistered> GetIndexed()
    {
      return new List<ProjectRegistered>
      {
      };
    }

    public ProjectRegistered GetNonIndexed()
    {
      return new ProjectRegistered
      {
        ProjectId = ProjectId,
        AcceptedSymbol = AcceptedSymbol,
        ProjectSymbol = ProjectSymbol,
        CrowdFundingType = CrowdFundingType,
        CrowdFundingIssueAmount = CrowdFundingIssueAmount,
        PreSalePrice = PreSalePrice,
        StartTime = StartTime,
        EndTime = EndTime,
        MinSubscription = MinSubscription,
        MaxSubscription = MaxSubscription,
        PublicSalePrice = PublicSalePrice,
        ListMarketInfo = ListMarketInfo,
        LiquidityLockProportion = LiquidityLockProportion,
        UnlockTime = UnlockTime,
        IsEnableWhitelist = IsEnableWhitelist,
        WhitelistId = WhitelistId,
        IsBurnRestToken = IsBurnRestToken,
        TotalPeriod = TotalPeriod,
        AdditionalInfo = AdditionalInfo,
        TargetRaisedAmount = TargetRaisedAmount,
        Creator = Creator,
        FirstDistributeProportion = FirstDistributeProportion,
        RestPeriodDistributeProportion = RestPeriodDistributeProportion,
        PeriodDuration = PeriodDuration,
        VirtualAddress = VirtualAddress,
        TokenReleaseTime = TokenReleaseTime,
        LiquidatedDamageProportion = LiquidatedDamageProportion,
      };
    }
  }

  public partial class NewWhitelistIdSet : aelf::IEvent<NewWhitelistIdSet>
  {
    public global::System.Collections.Generic.IEnumerable<NewWhitelistIdSet> GetIndexed()
    {
      return new List<NewWhitelistIdSet>
      {
      };
    }

    public NewWhitelistIdSet GetNonIndexed()
    {
      return new NewWhitelistIdSet
      {
        ProjectId = ProjectId,
        WhitelistId = WhitelistId,
      };
    }
  }

  public partial class AdditionalInfoUpdated : aelf::IEvent<AdditionalInfoUpdated>
  {
    public global::System.Collections.Generic.IEnumerable<AdditionalInfoUpdated> GetIndexed()
    {
      return new List<AdditionalInfoUpdated>
      {
      };
    }

    public AdditionalInfoUpdated GetNonIndexed()
    {
      return new AdditionalInfoUpdated
      {
        ProjectId = ProjectId,
        AdditionalInfo = AdditionalInfo,
      };
    }
  }

  public partial class ProjectCanceled : aelf::IEvent<ProjectCanceled>
  {
    public global::System.Collections.Generic.IEnumerable<ProjectCanceled> GetIndexed()
    {
      return new List<ProjectCanceled>
      {
      };
    }

    public ProjectCanceled GetNonIndexed()
    {
      return new ProjectCanceled
      {
        ProjectId = ProjectId,
      };
    }
  }

  public partial class PeriodUpdated : aelf::IEvent<PeriodUpdated>
  {
    public global::System.Collections.Generic.IEnumerable<PeriodUpdated> GetIndexed()
    {
      return new List<PeriodUpdated>
      {
      };
    }

    public PeriodUpdated GetNonIndexed()
    {
      return new PeriodUpdated
      {
        ProjectId = ProjectId,
        NewPeriod = NewPeriod,
      };
    }
  }

  public partial class Invested : aelf::IEvent<Invested>
  {
    public global::System.Collections.Generic.IEnumerable<Invested> GetIndexed()
    {
      return new List<Invested>
      {
      };
    }

    public Invested GetNonIndexed()
    {
      return new Invested
      {
        ProjectId = ProjectId,
        User = User,
        InvestSymbol = InvestSymbol,
        Amount = Amount,
        TotalAmount = TotalAmount,
        ProjectSymbol = ProjectSymbol,
        ToClaimAmount = ToClaimAmount,
      };
    }
  }

  public partial class DisInvested : aelf::IEvent<DisInvested>
  {
    public global::System.Collections.Generic.IEnumerable<DisInvested> GetIndexed()
    {
      return new List<DisInvested>
      {
      };
    }

    public DisInvested GetNonIndexed()
    {
      return new DisInvested
      {
        ProjectId = ProjectId,
        User = User,
        InvestSymbol = InvestSymbol,
        DisinvestedAmount = DisinvestedAmount,
        TotalAmount = TotalAmount,
      };
    }
  }

  public partial class LiquidatedDamageRecord : aelf::IEvent<LiquidatedDamageRecord>
  {
    public global::System.Collections.Generic.IEnumerable<LiquidatedDamageRecord> GetIndexed()
    {
      return new List<LiquidatedDamageRecord>
      {
      };
    }

    public LiquidatedDamageRecord GetNonIndexed()
    {
      return new LiquidatedDamageRecord
      {
        ProjectId = ProjectId,
        User = User,
        InvestSymbol = InvestSymbol,
        Amount = Amount,
      };
    }
  }

  public partial class LiquidatedDamageClaimed : aelf::IEvent<LiquidatedDamageClaimed>
  {
    public global::System.Collections.Generic.IEnumerable<LiquidatedDamageClaimed> GetIndexed()
    {
      return new List<LiquidatedDamageClaimed>
      {
      };
    }

    public LiquidatedDamageClaimed GetNonIndexed()
    {
      return new LiquidatedDamageClaimed
      {
        ProjectId = ProjectId,
        User = User,
        InvestSymbol = InvestSymbol,
        Amount = Amount,
      };
    }
  }

  public partial class Claimed : aelf::IEvent<Claimed>
  {
    public global::System.Collections.Generic.IEnumerable<Claimed> GetIndexed()
    {
      return new List<Claimed>
      {
      };
    }

    public Claimed GetNonIndexed()
    {
      return new Claimed
      {
        ProjectId = ProjectId,
        User = User,
        ProjectSymbol = ProjectSymbol,
        Amount = Amount,
        LatestPeriod = LatestPeriod,
        TotalPeriod = TotalPeriod,
      };
    }
  }

  public partial class ReFunded : aelf::IEvent<ReFunded>
  {
    public global::System.Collections.Generic.IEnumerable<ReFunded> GetIndexed()
    {
      return new List<ReFunded>
      {
      };
    }

    public ReFunded GetNonIndexed()
    {
      return new ReFunded
      {
        ProjectId = ProjectId,
        User = User,
        InvestSymbol = InvestSymbol,
        Amount = Amount,
      };
    }
  }

  public partial class Withdrawn : aelf::IEvent<Withdrawn>
  {
    public global::System.Collections.Generic.IEnumerable<Withdrawn> GetIndexed()
    {
      return new List<Withdrawn>
      {
      };
    }

    public Withdrawn GetNonIndexed()
    {
      return new Withdrawn
      {
        ProjectId = ProjectId,
        AcceptedSymbol = AcceptedSymbol,
        WithdrawAmount = WithdrawAmount,
        ProjectSymbol = ProjectSymbol,
        IsBurnRestToken = IsBurnRestToken,
        BurnAmount = BurnAmount,
      };
    }
  }

  public partial class LiquidatedDamageProportionUpdated : aelf::IEvent<LiquidatedDamageProportionUpdated>
  {
    public global::System.Collections.Generic.IEnumerable<LiquidatedDamageProportionUpdated> GetIndexed()
    {
      return new List<LiquidatedDamageProportionUpdated>
      {
      };
    }

    public LiquidatedDamageProportionUpdated GetNonIndexed()
    {
      return new LiquidatedDamageProportionUpdated
      {
        ProjectId = ProjectId,
        LiquidatedDamageProportion = LiquidatedDamageProportion,
      };
    }
  }

  #endregion
  public static partial class EwellContractContainer
  {
    static readonly string __ServiceName = "EwellContract";

    #region Marshallers
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.InitializeInput> __Marshaller_InitializeInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.InitializeInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Google.Protobuf.WellKnownTypes.Empty> __Marshaller_google_protobuf_Empty = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Google.Protobuf.WellKnownTypes.Empty.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.RegisterInput> __Marshaller_RegisterInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.RegisterInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.UpdateAdditionalInfoInput> __Marshaller_UpdateAdditionalInfoInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.UpdateAdditionalInfoInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::AElf.Types.Hash> __Marshaller_aelf_Hash = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::AElf.Types.Hash.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.AddWhitelistsInput> __Marshaller_AddWhitelistsInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.AddWhitelistsInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.RemoveWhitelistsInput> __Marshaller_RemoveWhitelistsInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.RemoveWhitelistsInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.SetWhitelistIdInput> __Marshaller_SetWhitelistIdInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.SetWhitelistIdInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.InvestInput> __Marshaller_InvestInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.InvestInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.ReFundAllInput> __Marshaller_ReFundAllInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.ReFundAllInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.ClaimInput> __Marshaller_ClaimInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.ClaimInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.UpdateLiquidatedDamageProportionInput> __Marshaller_UpdateLiquidatedDamageProportionInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.UpdateLiquidatedDamageProportionInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::AElf.Types.Address> __Marshaller_aelf_Address = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::AElf.Types.Address.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.LiquidatedDamageConfig> __Marshaller_LiquidatedDamageConfig = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.LiquidatedDamageConfig.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.ProjectInfo> __Marshaller_ProjectInfo = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.ProjectInfo.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.ProjectListInfo> __Marshaller_ProjectListInfo = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.ProjectListInfo.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.ExtraInfoIdList> __Marshaller_ExtraInfoIdList = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.ExtraInfoIdList.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.GetInvestDetailInput> __Marshaller_GetInvestDetailInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.GetInvestDetailInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.InvestDetail> __Marshaller_InvestDetail = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.InvestDetail.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.GetProfitDetailInput> __Marshaller_GetProfitDetailInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.GetProfitDetailInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.ProfitDetail> __Marshaller_ProfitDetail = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.ProfitDetail.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Ewell.Contracts.Ido.LiquidatedDamageDetails> __Marshaller_LiquidatedDamageDetails = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Ewell.Contracts.Ido.LiquidatedDamageDetails.Parser.ParseFrom);
    #endregion

    #region Methods
    static readonly aelf::Method<global::Ewell.Contracts.Ido.InitializeInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Initialize = new aelf::Method<global::Ewell.Contracts.Ido.InitializeInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Initialize",
        __Marshaller_InitializeInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.RegisterInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Register = new aelf::Method<global::Ewell.Contracts.Ido.RegisterInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Register",
        __Marshaller_RegisterInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.UpdateAdditionalInfoInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_UpdateAdditionalInfo = new aelf::Method<global::Ewell.Contracts.Ido.UpdateAdditionalInfoInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "UpdateAdditionalInfo",
        __Marshaller_UpdateAdditionalInfoInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Cancel = new aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Cancel",
        __Marshaller_aelf_Hash,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty> __Method_LockLiquidity = new aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "LockLiquidity",
        __Marshaller_aelf_Hash,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Withdraw = new aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Withdraw",
        __Marshaller_aelf_Hash,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.AddWhitelistsInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_AddWhitelists = new aelf::Method<global::Ewell.Contracts.Ido.AddWhitelistsInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "AddWhitelists",
        __Marshaller_AddWhitelistsInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.RemoveWhitelistsInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_RemoveWhitelists = new aelf::Method<global::Ewell.Contracts.Ido.RemoveWhitelistsInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "RemoveWhitelists",
        __Marshaller_RemoveWhitelistsInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty> __Method_NextPeriod = new aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "NextPeriod",
        __Marshaller_aelf_Hash,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.SetWhitelistIdInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetWhitelistId = new aelf::Method<global::Ewell.Contracts.Ido.SetWhitelistIdInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetWhitelistId",
        __Marshaller_SetWhitelistIdInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.InvestInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Invest = new aelf::Method<global::Ewell.Contracts.Ido.InvestInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Invest",
        __Marshaller_InvestInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Disinvest = new aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Disinvest",
        __Marshaller_aelf_Hash,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty> __Method_ReFund = new aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "ReFund",
        __Marshaller_aelf_Hash,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.ReFundAllInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_ReFundAll = new aelf::Method<global::Ewell.Contracts.Ido.ReFundAllInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "ReFundAll",
        __Marshaller_ReFundAllInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.ClaimInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Claim = new aelf::Method<global::Ewell.Contracts.Ido.ClaimInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Claim",
        __Marshaller_ClaimInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty> __Method_ClaimLiquidatedDamage = new aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "ClaimLiquidatedDamage",
        __Marshaller_aelf_Hash,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty> __Method_ClaimLiquidatedDamageAll = new aelf::Method<global::AElf.Types.Hash, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "ClaimLiquidatedDamageAll",
        __Marshaller_aelf_Hash,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.UpdateLiquidatedDamageProportionInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_UpdateLiquidatedDamageProportion = new aelf::Method<global::Ewell.Contracts.Ido.UpdateLiquidatedDamageProportionInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "UpdateLiquidatedDamageProportion",
        __Marshaller_UpdateLiquidatedDamageProportionInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Address, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetProxyAccountContract = new aelf::Method<global::AElf.Types.Address, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetProxyAccountContract",
        __Marshaller_aelf_Address,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.LiquidatedDamageConfig, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetLiquidatedDamageConfig = new aelf::Method<global::Ewell.Contracts.Ido.LiquidatedDamageConfig, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetLiquidatedDamageConfig",
        __Marshaller_LiquidatedDamageConfig,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address> __Method_GetWhitelistContractAddress = new aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address>(
        aelf::MethodType.View,
        __ServiceName,
        "GetWhitelistContractAddress",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_aelf_Address);

    static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address> __Method_GetAdmin = new aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address>(
        aelf::MethodType.View,
        __ServiceName,
        "GetAdmin",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_aelf_Address);

    static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address> __Method_GetTokenContractAddress = new aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address>(
        aelf::MethodType.View,
        __ServiceName,
        "GetTokenContractAddress",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_aelf_Address);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Ewell.Contracts.Ido.ProjectInfo> __Method_GetProjectInfo = new aelf::Method<global::AElf.Types.Hash, global::Ewell.Contracts.Ido.ProjectInfo>(
        aelf::MethodType.View,
        __ServiceName,
        "GetProjectInfo",
        __Marshaller_aelf_Hash,
        __Marshaller_ProjectInfo);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Ewell.Contracts.Ido.ProjectListInfo> __Method_GetProjectListInfo = new aelf::Method<global::AElf.Types.Hash, global::Ewell.Contracts.Ido.ProjectListInfo>(
        aelf::MethodType.View,
        __ServiceName,
        "GetProjectListInfo",
        __Marshaller_aelf_Hash,
        __Marshaller_ProjectListInfo);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Ewell.Contracts.Ido.ExtraInfoIdList> __Method_GetWhitelist = new aelf::Method<global::AElf.Types.Hash, global::Ewell.Contracts.Ido.ExtraInfoIdList>(
        aelf::MethodType.View,
        __ServiceName,
        "GetWhitelist",
        __Marshaller_aelf_Hash,
        __Marshaller_ExtraInfoIdList);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.GetInvestDetailInput, global::Ewell.Contracts.Ido.InvestDetail> __Method_GetInvestDetail = new aelf::Method<global::Ewell.Contracts.Ido.GetInvestDetailInput, global::Ewell.Contracts.Ido.InvestDetail>(
        aelf::MethodType.View,
        __ServiceName,
        "GetInvestDetail",
        __Marshaller_GetInvestDetailInput,
        __Marshaller_InvestDetail);

    static readonly aelf::Method<global::Ewell.Contracts.Ido.GetProfitDetailInput, global::Ewell.Contracts.Ido.ProfitDetail> __Method_GetProfitDetail = new aelf::Method<global::Ewell.Contracts.Ido.GetProfitDetailInput, global::Ewell.Contracts.Ido.ProfitDetail>(
        aelf::MethodType.View,
        __ServiceName,
        "GetProfitDetail",
        __Marshaller_GetProfitDetailInput,
        __Marshaller_ProfitDetail);

    static readonly aelf::Method<global::AElf.Types.Hash, global::AElf.Types.Hash> __Method_GetWhitelistId = new aelf::Method<global::AElf.Types.Hash, global::AElf.Types.Hash>(
        aelf::MethodType.View,
        __ServiceName,
        "GetWhitelistId",
        __Marshaller_aelf_Hash,
        __Marshaller_aelf_Hash);

    static readonly aelf::Method<global::AElf.Types.Hash, global::Ewell.Contracts.Ido.LiquidatedDamageDetails> __Method_GetLiquidatedDamageDetails = new aelf::Method<global::AElf.Types.Hash, global::Ewell.Contracts.Ido.LiquidatedDamageDetails>(
        aelf::MethodType.View,
        __ServiceName,
        "GetLiquidatedDamageDetails",
        __Marshaller_aelf_Hash,
        __Marshaller_LiquidatedDamageDetails);

    static readonly aelf::Method<global::AElf.Types.Hash, global::AElf.Types.Address> __Method_GetProjectAddressByProjectHash = new aelf::Method<global::AElf.Types.Hash, global::AElf.Types.Address>(
        aelf::MethodType.View,
        __ServiceName,
        "GetProjectAddressByProjectHash",
        __Marshaller_aelf_Hash,
        __Marshaller_aelf_Address);

    static readonly aelf::Method<global::AElf.Types.Address, global::AElf.Types.Address> __Method_GetPendingProjectAddress = new aelf::Method<global::AElf.Types.Address, global::AElf.Types.Address>(
        aelf::MethodType.View,
        __ServiceName,
        "GetPendingProjectAddress",
        __Marshaller_aelf_Address,
        __Marshaller_aelf_Address);

    static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address> __Method_GetProxyAccountContract = new aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address>(
        aelf::MethodType.View,
        __ServiceName,
        "GetProxyAccountContract",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_aelf_Address);

    static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::Ewell.Contracts.Ido.LiquidatedDamageConfig> __Method_GetLiquidatedDamageConfig = new aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::Ewell.Contracts.Ido.LiquidatedDamageConfig>(
        aelf::MethodType.View,
        __ServiceName,
        "GetLiquidatedDamageConfig",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_LiquidatedDamageConfig);

    #endregion

    #region Descriptors
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Ewell.Contracts.Ido.EwellContractReflection.Descriptor.Services[0]; }
    }

    public static global::System.Collections.Generic.IReadOnlyList<global::Google.Protobuf.Reflection.ServiceDescriptor> Descriptors
    {
      get
      {
        return new global::System.Collections.Generic.List<global::Google.Protobuf.Reflection.ServiceDescriptor>()
        {
          global::AElf.Standards.ACS12.Acs12Reflection.Descriptor.Services[0],
          global::Ewell.Contracts.Ido.EwellContractReflection.Descriptor.Services[0],
        };
      }
    }
    #endregion
  }
}
#endregion

