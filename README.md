# ASTSerialization
Serialization and deserialization implementations for Microsoft.ProgramSynthesis.AST.

This repository is related to the repository [Microsoft.prose](https://github.com/Microsoft/prose).

The method `string ProgramNode.PrintAST()` and `ProgramNode ProgramNode.Parse(string, Grammar)` 
in SDK provided by Microsoft cannot treat the C# non-basic type correctly. 
This repository provide two method `XElement ASTSerialization.Serialization.PrintXML()` and 
`ProgramNode ASTSerialization.Serialization.Parse(XElement)` which solved the problem.

A C# non-basic type wants to be serialized and deserialized by `ASTSerialization.Serialization`
must implement the interface `ASTSerialization.IObjSerializable` and provide a constructor
with `XElement` as the only one parameter.

Temporarily, it supports all basic types except `System.Enum` and `System.Struct`. 
`ProgramNode` supports `NonterminalNode`,`LiteralNode` and `VariableNode`.

The example show the usage in test method of ProseTutorial.Tests/substringTest.cs(method call) and class `Regex2` of
ProseTutorial/synthesis/WitnessFunctions.cs(interface and constructor implementation)
