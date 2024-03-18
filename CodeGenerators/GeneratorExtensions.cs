using Microsoft.CodeAnalysis;
using Stubble.Core.Builders;
using System.Text;

namespace CodeGenerators;

public static class GeneratorExtensions
{
	private static readonly Stubble.Core.StubbleVisitorRenderer Stubble = new StubbleBuilder().Build();

	private static readonly StringBuilder InitialSource = new StringBuilder()
		.AppendLine("using System;")
		.AppendLine("namespace Deserializer;");

	public static void AddInitialSourcePart(this GeneratorInitializationContext this_, string template, object data) {
		var codeToAdd = Stubble.Render(template, data);
		InitialSource.AppendLine(codeToAdd);
	}

	public static void FinishInitialSource(this GeneratorInitializationContext this_, string name) {
		this_.RegisterForPostInitialization((i) => i.AddSource(name, InitialSource.ToString()));
	}

	public static void AddSourceFromTemplate(this GeneratorExecutionContext this_, string name, string template, object data) {
		var codeToAdd = Stubble.Render(template, data);
		this_.AddSource(name, codeToAdd);
	}
}
