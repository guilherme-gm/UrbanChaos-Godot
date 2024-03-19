# Code Generators project

This project contains Source Generators used by the other projects in
UC-Godot.

https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview

Below you can see a general description of what it can do.

## Deserializer
Generates a "Deserialize" into classes that performs the process of reading a binary file into a C# class.

Example:
```CSharp
// MyClass.cs
[Deserializer.DeserializeGenerator]
public partial class MyClass
{
	public int MyIntVal { get; set; }

	[Deserializer.FixedArray(Dimensions = [3])]
	public int[] SomeNumbers { get; set; }
}
```

When the code is compiled, a new method is added to it, which looks more or less like:
```CSharp
// MyClass_deserialize.g.cs
public partial class MyClass
{
	public static MyClass Deserialize(BinaryReader br) {
		var value = new MyClass();

		value.MyIntVal = br.ReadInt32();
		value.SomeNumbers = [
			br.ReadInt32(), br.ReadInt32(), br.ReadInt32()
		];

		return value;
	}
}
```

This powers the AssetTool project which needs to deserialize the original game's assets.


### Deserializer context variables
When `Deserialize` code is running, there are a few variables that are a given.
You may need them to interact with some of the attributes below.

- `value`: Is the instance of the class being built.
  Following the order the fields were declared, during the evaluation of a field `X`,
  every field declared before `X` will have been evaluated already,
  while fields declared after `X` will not.

- `br`: The binary reader instance


### Class Attributes

#### `DeserializeGenerator`
Determines that the source generator should apply to this class in order to generate the `Deserialize` method.


### Property/Field Deserialization Attributes
Those attributes directly affect how a Field is deserialized, usually they don't work with each other (unless otherwise mentioned).

#### `DeserializeFn(FnName="<function name>")`
Determines that this field should be deserialized by `<function name>`, that is a static method part of the same class.

For improved code guarantees, use `nameof` for `FnName` (Example: `FnName = nameof(MyMethod)`).

The function must follow this pattern:
```CSharp
private static <FieldType> <FunctionName>(<ClassName> value, BinaryReader br) {
	// ...

	return <Deserialized Value>;
}
```

#### `FixedArray(Dimensions = int[])`
Determines that this field is an array with a fixed size and the given dimensions. You may have as many dimensions as needed.

Example:
```CSharp
[FixedArray(Dimensions = [5, 3, 2])]
public int[][][] MyVeryComplexArray { get; set; }
```

This will read 5 * 3 * 2 int values, forming `MyVeryComplexArray[5][3][2]`.


#### `FixedLenString(Length = int)`
Reads `Length` bytes as a string, ignoring everything after the first `\0` (0x00).

Opposed to what `BinaryReader::ReadString()` does, this always consumes `Length` bytes.

Example:
```CSharp
[FixedLenString(Length = 20)]
public string MyString { get; set; }
```

**Note:** This attri

#### `Nested()`
This field must be deserialized by their own `Deserialize` method.

**This attribute _may_ be used together with Array-related attributes.**

Example:
```CSharp
[DeserializeGenerator]
public class MyClass2 {
	//
}

[DeserializeGenerator]
public class MyClass1 {
	[FixedArray(Dimensions = [2])]
	[Nested]
	public MyClass2[] ArrayOfClass2 { get; set; }

	[Nested]
	public MyClass2 SingleClass2 { get; set; }

	/* Generated code
	public static MyClass1 Deserialize(BinaryReader br) {
		// ...
		value.ArrayOfClass2 = [MyClass2.Deserialize(br), MyClass2.Deserialize(br)];
		value.SingleClass2 = MyClass2.Deserialize(br);
	}
	*/
}
```


#### `VariableSizedArray(SizePropertyName = "<field name>")`
This field is an array which its size defined by `value.[FieldName]`. `[FieldName]` must be
defined before the field with this attribute for it to work properly.

It is recomended to use `nameof` to set the field name so field renames can be better type-checked.

Currently, it **DOES NOT** support multi-dimensional arrays.


### Property/Field Flow Attributes

These attributes afects the flow of deserialization and may be combined with the deserialization ones.

#### `Condition(When="<condition>")`
Determines that this field should only get deserialized when `When` condition is true.

`<condtion>` is simply written as is into a `if` statement. So any C# code may go there.

During deserialization, if a field doesn't match the given condition, it will not have any value assigned to it,
and thus use the C# default value for the data type.


#### `Skip()`
Simply ignores this field during deserialization.

You may use it when there are fields meant for other purposes and not to describe a file structre.


### Hooks
During deserialization, the following methods are called and may be used to perform additional operations.

#### `void PostDeserialize()` (Instance method)
Called after `Deserialize` finishes and before it returns. May be used for post processing.

Example:
```CSharp
[DeserializeGenerator]
public partial class MyClass
{
	public int Value { get; set; }

	partial void PostDeserialize() {
		this.Value *= 10;
	}
}

// Code somewhere else
var c = MyClass.Deserialize(br); // Let's image the file had Value = 5
c.Value; // 50
```
