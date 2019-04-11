# ASTSerialization
Serialization and deserialization implement for Microsoft.ProgramSynthesis.AST.

This repository is related to the repository [Microsoft.prose](https://github.com/Microsoft/prose).

The method `string ProgramNode.PrintAST()` and `ProgramNode ProgramNode.Parse(string, Grammar)` 
in SDK provided by Microsoft cannot treat the C# non-basic type correctly. 
This repository provide two method `XElement ASTSerialization.Serialization.PrintXML()` and 
`ProgramNode ASTSerialization.Serialization.Parse(XElement)` which solved the problem.

A C# non-basic type wants to be serialized and deserialized by `ASTSerialization.Serialization`
must implement the interface `ASTSerialization.IObjSerializable` and provide a construction
with `XElement` as the only one parameter.

Temporarily, it supports the basic type `System.Int32` and `System.string`
(Other basic type may be supported if met) and the `ProgramNode`: `NonterminalNode`,`LiteralNode`
and `VariableNode`.
