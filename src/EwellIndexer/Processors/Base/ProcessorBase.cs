using AeFinder.Sdk.Processor;
using AElf.CSharp.Core;
using Volo.Abp.ObjectMapping;

namespace EwellIndexer.Processors.Base;

public abstract class ProcessorBase<TEvent> : LogEventProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();
}