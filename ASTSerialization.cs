using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Rules;

namespace ASTSerialization
{
    public interface IObjSerializable
    {
        Type getSerializedType();
        XElement serialize();// Deserialization needs a construction function with XElement
    }
    public class Serialization
    {
        private Grammar grammar;
        public Serialization(Grammar _grammar)
        {
            grammar = _grammar;
        }
        public XElement PrintXML(ProgramNode node)
        {
            switch(node.GetType().FullName.ToString())
            {
                case "Microsoft.ProgramSynthesis.AST.TerminalNode":
                    throw(new TypeAccessException("TerminalNode is abstract node!"));
                case "Microsoft.ProgramSynthesis.AST.NonterminalNode":
                {
                    XElement xe = new XElement("NonterminalNode");
                    var nonterminalNode = node as NonterminalNode;
                    xe.SetAttributeValue("rule",nonterminalNode.Rule.ToString());
                    foreach(var child in nonterminalNode.Children)
                    {
                        xe.Add(PrintXML(child));
                    }
                    return xe;
                }
                case "Microsoft.ProgramSynthesis.AST.Hole":
                    throw(new NotImplementedException("Hole is not supported yet!"));
                case "Microsoft.ProgramSynthesis.AST.LiteralNode":
                {
                    XElement xe = new XElement("LiteralNode");
                    var literalNode = node as LiteralNode;
                    xe.SetAttributeValue("symbol",literalNode.Symbol.ToString());
                    switch(literalNode.Value.GetType().FullName.ToString())
                    {
                        case "System.Int32":
                            xe.SetAttributeValue("type",literalNode.Value.GetType().FullName.ToString());
                            xe.Add(literalNode.Value.ToString());
                            break;
                        case "System.string":
                            xe.SetAttributeValue("type",literalNode.Value.GetType().FullName.ToString());
                            xe.Add(literalNode.Value);
                            break;
                        default:
                        {
                            var iobj = literalNode.Value as IObjSerializable;
                            if(iobj==null)
                                throw(new NotSupportedException("Type " + literalNode.Value.GetType().FullName.ToString() 
                                    + " wants to be serialized must finish the interface 'IObjSerializable' first."));
                            xe.SetAttributeValue("type",iobj.getSerializedType().FullName.ToString());
                            xe.Add(iobj.serialize());
                            break;
                        }
                    }
                    return xe;
                }
                case "Microsoft.ProgramSynthesis.AST.VariableNode":
                {
                    XElement xe = new XElement("VariableNode");
                    var variableNode = node as VariableNode;
                    xe.SetAttributeValue("symbol",variableNode.Symbol.ToString());
                    return xe;
                }
                case "Microsoft.ProgramSynthesis.AST.LambdaNode":
                    throw(new NotImplementedException("LambdaNode is not supported yet!"));
                case "Microsoft.ProgramSynthesis.AST.LetNode":
                    throw(new NotImplementedException("LetNode is not supported yet!"));
                default:
                    throw(new TypeAccessException("Type "+node.GetType().FullName.ToString()+" is invalid."));
            }
        }
        public ProgramNode Parse(XElement xe)
        {
            switch(xe.Name.ToString())
            {
                case "TerminalNode":
                    throw(new FormatException("TerminalNode is abstract node!"));
                case "NonterminalNode":
                {
                    // var symbolName = xe.Attribute("symbol").Value;
                    var ruleName = xe.Attribute("rule").Value;
                    var grammarRule = (NonterminalRule) grammar.Rule(ruleName);
                    var children = new ProgramNode[xe.Elements().Count()];
                    int index = 0;
                    foreach(var childXe in xe.Elements())
                    {
                        children[index] = Parse(childXe);
                        index++;
                    }
                    var node = new NonterminalNode(grammarRule,children);
                    return node;
                }
                case "Hole":
                    throw(new NotImplementedException("Hole is not supported yet!"));
                case "LiteralNode":
                {
                    var symbolName = xe.Attribute("symbol").Value;
                    var typeName = xe.Attribute("type").Value;
                    Type type = Type.GetType(typeName);
                    Object obj = null;
                    switch(type.FullName.ToString())
                    {
                        case "System.Int32":
                            obj = Int32.Parse(xe.Value);
                            break;
                        case "System.string":
                            obj = xe.Value;
                            break;
                        default:
                        {
                            if(type==null)
                                throw(new TypeAccessException("Type " + typeName + " was not found!"));
                            var serializedObjct = xe.FirstNode;
                            try
                            {
                                obj = Activator.CreateInstance(type,serializedObjct);
                            }
                            catch(MissingMethodException)
                            {
                                throw(new MissingMethodException(type.GetType().FullName.ToString()
                                    + " does not implement the construction fuction with XElement for deserialization."));
                            }
                            break;
                        }
                    }
                    var node = new LiteralNode(grammar.Symbol(symbolName),obj);
                    return node;
                }
                case "VariableNode":
                {
                    var symbolName = xe.Attribute("symbol").Value;
                    var node = new VariableNode(grammar.Symbol(symbolName));
                    return node;
                }
                case "LambdaNode":
                    throw(new NotImplementedException("LambdaNode is not supported yet!"));
                case "LetNode":
                    throw(new NotImplementedException("LetNode is not supported yet!"));
                default:
                    throw(new FormatException("Unknown XML node label!"));
            }
        }
    }
}

