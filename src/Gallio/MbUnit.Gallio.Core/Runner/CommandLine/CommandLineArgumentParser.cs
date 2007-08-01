// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.org
//		Author: Jonathan de Halleux

using System;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace MbUnit.Core.Runner.CommandLine
{
	/// <summary>
	/// Parser for command line arguments.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The parser specification is infered from the instance fields of the object
	/// specified as the destination of the parse.
	/// Valid argument types are: int, uint, string, bool, enums
	/// Also argument types of Array of the above types are also valid.
	/// </para>
	/// <para>
	/// Error checking options can be controlled by adding a CommandLineArgumentAttribute
	/// to the instance fields of the destination object.
	/// </para>
	/// <para>
	/// At most one field may be marked with the DefaultCommandLineArgumentAttribute
	/// indicating that arguments without a '-' or '/' prefix will be parsed as that argument.
	/// </para>
	/// <para>
	/// If not specified then the parser will infer default options for parsing each
	/// instance field. The default long name of the argument is the field name. The
	/// default short name is the first character of the long name. Long names and explicitly
	/// specified short names must be unique. Default short names will be used provided that
	/// the default short name does not conflict with a long name or an explicitly
	/// specified short name.
	/// </para>
	/// <para>
	/// Arguments which are array types are collection arguments. Collection
	/// arguments can be specified multiple times.
	/// </para>
	/// <para>
	/// Command line parsing code from Peter Halam, 
	/// http://www.gotdotnet.com/community/usersamples/details.aspx?sampleguid=62a0f27e-274e-4228-ba7f-bc0118ecc41e
	/// </para>
	/// </remarks>
	public class CommandLineArgumentParser
	{
		/// <summary>
		/// Creates a new command line argument parser.
		/// </summary>
		/// <param name="argumentSpecification"> The type of object to  parse. </param>
		/// <param name="reporter"> The destination for parse errors. </param>
		public CommandLineArgumentParser(Type argumentSpecification, ErrorReporter reporter)
		{
			this.reporter = reporter;
			this.arguments = new ArrayList();
			this.argumentMap = new Hashtable();
            
			foreach (FieldInfo field in argumentSpecification.GetFields())
			{
				if (!field.IsStatic && !field.IsInitOnly && !field.IsLiteral)
				{
					CommandLineArgumentAttribute attribute = GetAttribute(field);
					if (attribute is DefaultCommandLineArgumentAttribute)
					{
						if (this.defaultArgument!=null)
							ThrowError("More that one DefaultCommandLineArgument has been used");
						this.defaultArgument = new Argument(attribute, field, reporter);
					}
					else
					{
						this.arguments.Add(new Argument(attribute, field, reporter));
					}
				}
			}
            
			// add explicit names to map
			foreach (Argument argument in this.arguments)
			{
				if (argumentMap.ContainsKey(argument.LongName))
					ThrowError("Argument {0} is duplicated",argument.LongName);
				this.argumentMap[argument.LongName] = argument;
				if (argument.ExplicitShortName && argument.ShortName != null && argument.ShortName.Length > 0)
				{
					if(this.argumentMap.ContainsKey(argument.ShortName))
						ThrowError("Argument {0} is duplicated",argument.ShortName);
					this.argumentMap[argument.ShortName] = argument;
				}
			}
            
			// add implicit names which don't collide to map
			foreach (Argument argument in this.arguments)
			{
				if (!argument.ExplicitShortName && argument.ShortName != null && argument.ShortName.Length > 0)
				{
					if (!argumentMap.ContainsKey(argument.ShortName))
						this.argumentMap[argument.ShortName] = argument;
				}
			}
		}
        
		private static CommandLineArgumentAttribute GetAttribute(FieldInfo field)
		{
			object[] attributes = field.GetCustomAttributes(typeof(CommandLineArgumentAttribute), false);
			if (attributes.Length==0)
				ThrowError("No fields tagged with CommandLineArgumentAttribute");
			return (CommandLineArgumentAttribute) attributes[0];
		}
        
		private void ReportUnrecognizedArgument(string argument)
		{
			this.reporter(string.Format("Unrecognized command line argument '{0}'", argument));
		}

		private static void ThrowError(string message, params Object[] args)
		{
			throw new InvalidOperationException(String.Format(message,args));
		}
        
		/// <summary>
		/// Parses an argument list into an object
		/// </summary>
		/// <param name="args"></param>
		/// <param name="destination"></param>
		/// <returns> true if an error occurred </returns>
		private bool ParseArgumentList(string[] args, object destination)
		{
			bool hadError = false;
			if (args != null)
			{
				foreach (string argument in args)
				{
					if (argument.Length > 0)
					{
						switch (argument[0])
						{
							case '-':
							case '/':
								int endIndex = argument.IndexOfAny(new char[] {':', '+'}, 1);
								string option = argument.Substring(1, endIndex == -1 ? argument.Length - 1 : endIndex - 1);
								string optionArgument;
								if (option.Length + 1 == argument.Length)
								{
									optionArgument = null;
								}
								else if (argument.Length > 1 + option.Length && argument[1 + option.Length] == ':')
								{
									optionArgument = argument.Substring(option.Length + 2);
								}
								else
								{
									optionArgument = argument.Substring(option.Length + 1);
								}
                                
								Argument arg = (Argument) this.argumentMap[option];
								if (arg == null)
								{
									ReportUnrecognizedArgument(argument);
									hadError = true;
								}
								else
								{
									hadError |= !arg.SetValue(optionArgument, destination);
								}
								break;
							case '@':
								string[] nestedArguments;
								hadError |= LexFileArguments(argument.Substring(1), out nestedArguments);
								hadError |= ParseArgumentList(nestedArguments, destination);
								break;
							default:
								if (this.defaultArgument != null)
								{
									hadError |= !this.defaultArgument.SetValue(argument, destination);
								}
								else
								{
									ReportUnrecognizedArgument(argument);
									hadError = true;
								}
								break;
						}
					}
				}
			}
            
			return hadError;
		}
        
		/// <summary>
		/// Parses an argument list.
		/// </summary>
		/// <param name="args"> The arguments to parse. </param>
		/// <param name="destination"> The destination of the parsed arguments. </param>
		/// <returns> true if no parse errors were encountered. </returns>
		public bool Parse(string[] args, object destination)
		{
			bool hadError = ParseArgumentList(args, destination);

			// check for missing required arguments
			foreach (Argument arg in this.arguments)
			{
				hadError |= arg.Finish(destination);
			}
			if (this.defaultArgument != null)
			{
				hadError |= this.defaultArgument.Finish(destination);
			}
            
			return !hadError;
		}
        
        
		/// <summary>
		/// A user firendly usage string describing the command line argument syntax.
		/// </summary>
		public string Usage
		{
			get
			{
				StringBuilder builder = new StringBuilder();
                
				int oldLength;
				foreach (Argument arg in this.arguments)
				{
					oldLength = builder.Length;
                    
					builder.Append("    /");
					builder.Append(arg.LongName);
					Type valueType = arg.ValueType;
					if (valueType == typeof(int))
					{
						builder.Append(":<int>");
					}
					else if (valueType == typeof(uint))
					{
						builder.Append(":<uint>");
					}
					else if (valueType == typeof(bool))
					{
						builder.Append("[+|-]");
					}
					else if (valueType == typeof(string))
					{
						builder.Append(":<string>");
					}
					else
					{
						Debug.Assert(valueType.IsEnum);
                        
						builder.Append(":{");
						bool first = true;
						foreach (FieldInfo field in valueType.GetFields())
						{
							if (field.IsStatic)
							{
								if (first)
									first = false;
								else
									builder.Append('|');
								builder.Append(field.Name);
							}
						}
						builder.Append('}');
					}
                    
					if (arg.ShortName != arg.LongName && this.argumentMap[arg.ShortName] == arg)
					{
						builder.Append(' ', IndentLength(builder.Length - oldLength));
						builder.Append("short form /");
						builder.Append(arg.ShortName);
					}

					if (arg.Description.Length>0)
						builder.Append("\t"+arg.Description);
					builder.Append(CommandLineUtility.NewLine);
				}
                
				oldLength = builder.Length;
				builder.Append("    @<file>");
				builder.Append(' ', IndentLength(builder.Length - oldLength));
				builder.Append("Read response file for more options");
				builder.Append(CommandLineUtility.NewLine);
                
				if (this.defaultArgument != null)
				{
					oldLength = builder.Length;
					builder.Append("    <");
					builder.Append(this.defaultArgument.LongName);
					builder.Append(">");
					builder.Append(CommandLineUtility.NewLine);
				}
                
				return builder.ToString();
			}
		}
            
		private static int IndentLength(int lineLength)
		{
			return Math.Max(4, 40 - lineLength);
		}
        
		private bool LexFileArguments(string fileName, out string[] arguments)
		{
			string args  = null;
                    
			try
			{
				using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					args = (new StreamReader(file)).ReadToEnd();
				}
			}
			catch (Exception e)
			{
				this.reporter(string.Format("Error: Can't open command line argument file '{0}' : '{1}'", fileName, e.Message));
				arguments = null;
				return false;
			}

			bool hadError = false;                    
			ArrayList argArray = new ArrayList();
			StringBuilder currentArg = new StringBuilder();
			bool inQuotes = false;
			int index = 0;
            
			// while (index < args.Length)
			try
			{
				while (true)
				{
					// skip whitespace
					while (char.IsWhiteSpace(args[index]))
					{
						index += 1;
					}
                    
					// # - comment to end of line
					if (args[index] == '#')
					{
						index += 1;
						while (args[index] != '\n')
						{
							index += 1;
						}
						continue;
					}
                    
					// do one argument
					do
					{
						if (args[index] == '\\')
						{
							int cSlashes = 1;
							index += 1;
							while (index == args.Length && args[index] == '\\')
							{
								cSlashes += 1;
							}

							if (index == args.Length || args[index] != '"')
							{
								currentArg.Append('\\', cSlashes);
							}
							else
							{
								currentArg.Append('\\', (cSlashes >> 1));
								if (0 != (cSlashes & 1))
								{
									currentArg.Append('"');
								}
								else
								{
									inQuotes = !inQuotes;
								}
							}
						}
						else if (args[index] == '"')
						{
							inQuotes = !inQuotes;
							index += 1;
						}
						else
						{
							currentArg.Append(args[index]);
							index += 1;
						}
					} while (!char.IsWhiteSpace(args[index]) || inQuotes);
					argArray.Add(currentArg.ToString());
					currentArg.Length = 0;
				}
			}
			catch (System.IndexOutOfRangeException)
			{
				// got EOF 
				if (inQuotes)
				{
					this.reporter(string.Format("Error: Unbalanced '\"' in command line argument file '{0}'", fileName));
					hadError = true;
				}
				else if (currentArg.Length > 0)
				{
					// valid argument can be terminated by EOF
					argArray.Add(currentArg.ToString());
				}
			}
            
			arguments = (string[]) argArray.ToArray(typeof (string));
			return hadError;
		}
        
		private static string LongName(CommandLineArgumentAttribute attribute, FieldInfo field)
		{
			return (attribute == null || attribute.DefaultLongName) ? field.Name : attribute.LongName;
		}
        
		private static string ShortName(CommandLineArgumentAttribute attribute, FieldInfo field)
		{
			return !ExplicitShortName(attribute) ? LongName(attribute, field).Substring(0,1) : attribute.ShortName;
		}
        
		private static bool ExplicitShortName(CommandLineArgumentAttribute attribute)
		{
			return (attribute != null && !attribute.DefaultShortName);
		}
        
		private static Type ElementType(FieldInfo field)
		{
			if (IsCollectionType(field.FieldType))
				return field.FieldType.GetElementType();
			else
				return null;
		}
        
		private static CommandLineArgumentType Flags(CommandLineArgumentAttribute attribute, FieldInfo field)
		{
			if (attribute != null)
				return attribute.Type;
			else if (IsCollectionType(field.FieldType))
				return CommandLineArgumentType.MultipleUnique;
			else
				return CommandLineArgumentType.AtMostOnce;
		}
        
		private static bool IsCollectionType(Type type)
		{
			return type.IsArray;
		}
            
		private static bool IsValidElementType(Type type)
		{
			return type != null && (
				type == typeof(int) ||
				type == typeof(uint) ||
				type == typeof(string) ||
				type == typeof(bool) ||
				type.IsEnum);
		}
        
		private class Argument
		{
			public Argument(CommandLineArgumentAttribute attribute, FieldInfo field, ErrorReporter reporter)
			{
				this.longName = CommandLineArgumentParser.LongName(attribute, field);
				this.explicitShortName = CommandLineArgumentParser.ExplicitShortName(attribute);
				this.shortName = CommandLineArgumentParser.ShortName(attribute, field);
				this.elementType = ElementType(field);
				this.flags = Flags(attribute, field);
				this.field = field;
				this.seenValue = false;
				this.reporter = reporter;
				this.isDefault = attribute != null && attribute is DefaultCommandLineArgumentAttribute;
				this.description=attribute.Description;
                
				if (IsCollection)
				{
					this.collectionValues = new ArrayList();
				}
                
				Debug.Assert(this.longName != null && this.longName.Length > 0);
				if (IsCollection && !AllowMultiple)
					ThrowError("Collection arguments must have allow multiple");
				Debug.Assert(!Unique || IsCollection, "Unique only applicable to collection arguments");
				Debug.Assert(IsValidElementType(Type) ||
					IsCollectionType(Type));
				Debug.Assert((IsCollection && IsValidElementType(elementType)) ||
					(!IsCollection && elementType == null));
			}
            
			public bool Finish(object destination)
			{
				if (this.IsCollection)
				{
					this.field.SetValue(destination, this.collectionValues.ToArray(this.elementType));
				}
                
				return ReportMissingRequiredArgument();
			}
            
			private bool ReportMissingRequiredArgument()
			{
				if (this.IsRequired && !this.SeenValue)
				{
					if (this.IsDefault)
						reporter(string.Format("Missing required argument '<{0}>'.", this.LongName));
					else
						reporter(string.Format("Missing required argument '/{0}'.", this.LongName));
					return true;
				}
				return false;
			}
            
			private void ReportDuplicateArgumentValue(string value)
			{
				this.reporter(string.Format("Duplicate '{0}' argument '{1}'", this.LongName, value));
			}
            
			public bool SetValue(string value, object destination)
			{
				if (SeenValue && !AllowMultiple)
				{
					this.reporter(string.Format("Duplicate '{0}' argument", this.LongName));
					return false;
				}
				this.seenValue = true;
                
				object newValue;
				if (!ParseValue(this.ValueType, value, out newValue))
					return false;
				if (this.IsCollection)
				{
					if (this.Unique && this.collectionValues.Contains(newValue))
					{
						ReportDuplicateArgumentValue(value);
						return false;
					}
					else
					{
						this.collectionValues.Add(newValue);
					}
				}
				else
				{
					this.field.SetValue(destination, newValue);
				}
                
				return true;
			}
            
			public Type ValueType
			{
				get { return this.IsCollection ? this.elementType : this.Type; }
			}
            
			private void ReportBadArgumentValue(string value)
			{
				this.reporter(string.Format("'{0}' is not a valid value for the '{1}' command line option", value, this.LongName));
			}
            
			private bool ParseValue(Type type, string stringData, out object value)
			{
				// null is only valid for bool variables
				// empty string is never valid
				if ((stringData != null || type == typeof(bool)) && (stringData == null || stringData.Length > 0))
				{
					try
					{
						if (type == typeof(string))
						{
							value = stringData;
							return true;
						}
						else if (type == typeof(bool))
						{
							if (stringData == null || stringData == "+")
							{
								value = true;
								return true;
							}
							else if (stringData == "-")
							{
								value = false;
								return true;
							}
						}
						else if (type == typeof(int))
						{
							value = int.Parse(stringData);
							return true;
						}
						else if (type == typeof(uint))
						{
							value = int.Parse(stringData);
							return true;
						}
						else
						{
							Debug.Assert(type.IsEnum);
							value = Enum.Parse(type, stringData, true);
							return true;
						}
					}
					catch
					{
						// catch parse errors
					}
				}
                                
				ReportBadArgumentValue(stringData);
				value = null;
				return false;
			}
            
			public string LongName
			{
				get { return this.longName; }
			}

			public bool ExplicitShortName
			{
				get { return this.explicitShortName; }
			}
            
			public string ShortName
			{
				get { return this.shortName; }
			}

			public bool IsRequired
			{
				get { return 0 != (this.flags & CommandLineArgumentType.Required); }
			}
            
			public bool SeenValue
			{
				get { return this.seenValue; }
			}
            
			public bool AllowMultiple
			{
				get { return 0 != (this.flags & CommandLineArgumentType.Multiple); }
			}
            
			public bool Unique
			{
				get { return 0 != (this.flags & CommandLineArgumentType.Unique); }
			}
            
			public Type Type
			{
				get { return field.FieldType; }
			}
            
			public bool IsCollection
			{
				get { return IsCollectionType(Type); }
			}
            
			public bool IsDefault
			{
				get { return this.isDefault; }
			}

			public string Description
			{
				get
				{
					return this.description;
				}
			}
            
			private string longName;
			private string shortName;
			private bool explicitShortName;
			private bool seenValue;
			private FieldInfo field;
			private Type elementType;
			private CommandLineArgumentType flags;
			private ArrayList collectionValues;
			private ErrorReporter reporter;
			private bool isDefault;
			private string description;
		}
        
		private ArrayList arguments;
		private Hashtable argumentMap;
		private Argument defaultArgument;
		private ErrorReporter reporter;
	}
}
