using Microsoft.CodeAnalysis;
using Stubble.Core.Builders;

namespace CodeGenerators;

public static class GeneratorExtensions
{
	private static readonly Stubble.Core.StubbleVisitorRenderer Stubble = new StubbleBuilder().Build();

	public static void AddSourceFromTemplate(this GeneratorInitializationContext this_, string name, string template, object data) {
		var codeToAdd = Stubble.Render(template, data);
		this_.RegisterForPostInitialization((i) => i.AddSource(name, codeToAdd));
	}

	public static void AddSourceFromTemplate(this GeneratorExecutionContext this_, string name, string template, object data) {
		var codeToAdd = Stubble.Render(template, data);
		this_.AddSource(name, codeToAdd);
	}
}
