using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Runtime settings shared by the console sample runner.
/// </summary>
/// <param name="Environment">Resolved API environment.</param>
/// <param name="AllowProductionWrites">When <see langword="true"/>, interactive mutating examples may run on non-sandbox environments.</param>
internal sealed record SampleRuntimeContext(FreeAgentEnvironment Environment, bool AllowProductionWrites);
