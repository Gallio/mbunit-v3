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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace MbUnit.Core.ConsoleSupport.CommandLine
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
			arguments = new List<Argument>();
			argumentMap = new Hashtable();
            
			foreach (FieldInfo field in argumentSpecification.GetFields())
			{
				if (!field.IsStatic && !field.IsInitOnly && !field.IsLiteral)
				{
					CommandLineArgumentAttribute attribute = GetAttribute(field);
				    if (attribute is DefaultCommandLineArgumentAttribute)
				    {
				        if (defaultArgument != null)
				            ThrowError("More that one DefaultCommandLineArgument has been used");
				        defaultArgument = new Argument(attribute, field, reporter);
				    }
				    else
				        arguments.Add(new Argument(attribute, field, reporter));
				}
			}
            
			// add explicit names to map
			foreach (Argument argument in arguments)
			{
				if (argumentMap.ContainsKey(argument.LongName))
					ThrowError("Argument {0} is duplicated",argument.LongName);
				argumentMap[argument.LongName] = argument;
				if (argument.ExplicitShortName && argument.ShortName != null && argument.ShortName.Length > 0)
				{
					if(argumentMap.ContainsKey(argument.ShortName))
						ThrowError("Argument {0} is duplicated",argument.ShortName);
					argumentMap[argument.ShortName] = argument;
				}
			}
            
			// add implicit names which don't collide to map
			foreach (Argument argument in arguments)
			{
				if (!argument.ExplicitShortName && argument.ShortName != null && argument.ShortName.Length > 0)
				{
					if (!argumentMap.ContainsKey(argument.ShortName))
						argumentMap[argument.ShortName] = argument;
				}
			}
		}
        
		private static CommandLineArgumentAttribute GetAttribute(ICustomAttributeProvider field)
		{
			object[] attributes = field.GetCustomAttributes(typeof(CommandLineArgumentAttribute), false);
			if (attributes.Length==0)
				ThrowError("No fields tagged with CommandLineArgumentAttribute");
			return (CommandLineArgumentAttribute) attributes[0];
		}
        
		private void ReportUnrecognizedArgument(string argument)
		{
			reporter(string.Format("Unrecognized command line argument '{0}'", argument));
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
		private bool ParseArgumentList(IEnumerable<string> args, object destination)
		{
			bool hadError = false;
			if (args != null)
			{
				foreach (string argument in args)
				{
				    if (argument != null && argument.Length > 0)
				        {
				            switch (argument[0])
				            {
				                case '-':
				                case '/':
				                    int endIndex = argument.IndexOfAny(new char[] {':', '+'}, 1);
				                    string option = argument.Substring(1, endIndex == -1 ? argument.Length - 1 : endIndex - 1);
				                    string optionArgument;
				                    if (option.Length + 1 == argument.Length)
				                        optionArgument = null;
				                    else if (argument.Length > 1 + option.Length && argument[1 + option.Length] == ':')
				                        optionArgument = argument.Substring(option.Length + 2);
				                    else
				                        optionArgument = argument.Substring(option.Length + 1);


				                    Argument arg = (Argument) argumentMap[option];
				                    if (arg == null)
				                    {
				                        ReportUnrecognizedArgument(argument);
				                        hadError = true;
				                    }
				                    else
				                        hadError |= !arg.SetValue(optionArgument, destination);

				                    break;
				                case '@':
				                    string[] nestedArguments;
				                    hadError |= LexFileArguments(argument.Substring(1), out nestedArguments);
				                    hadError |= ParseArgumentList(nestedArguments, destination);
				                    break;
				                default:
				                    if (defaultArgument != null)
				                        hadError |= !defaultArgument.SetValue(argument, destination);
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
		    foreach (Argument arg in arguments)
		        hadError |= arg.Finish(destination);

		    if (defaultArgument != null)
		        hadError |= defaultArgument.Finish(destination);

		    return !hadError;
		}
        
        /// <summary>
        /// A user firendly usage string describing the command line argument syntax.
        /// </summary>
        /// <param name="output">The command line output</param>
        public void ShowUsage(CommandLineOutput output)
        {
            foreach (Argument arg in arguments)
            {
                output.PrintArgumentHelp(arg.LongName, arg.ShortName, arg.Description, arg.ArgumentValueType, arg.Type);
                output.NewLine();
            }
            output.PrintText("@<file>          Read response file for more options.", 2);
            output.NewLine();
            output.PrintText(string.Format("<{0}>", defaultArgument.LongName), 2);
            output.NewLine();
        }
        
		private bool LexFileArguments(string fileName, out string[] nestedArguments)
		{
			string args;
                    
			try
			{
				using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					args = (new StreamReader(file)).ReadToEnd();
				}
			}
			catch (Exception e)
			{
				reporter(string.Format("Error: Can't open command line argument file '{0}' : '{1}'", fileName, e.Message));
				nestedArguments = null;
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
			catch (IndexOutOfRangeException)
			{
				// got EOF 
				if (inQuotes)
				{
					reporter(string.Format("Error: Unbalanced '\"' in command line argument file '{0}'", fileName));
					hadError = true;
				}
				else if (currentArg.Length > 0)
				{
					// valid argument can be terminated by EOF
					argArray.Add(currentArg.ToString());
				}
			}
            
			nestedArguments = (string[]) argArray.ToArray(typeof (string));
			return hadError;
		}
        
		private static string LongName(CommandLineArgumentAttribute attribute, FieldInfo field)
		{
			return (attribute == null || attribute.IsDefaultLongName) ? field.Name : attribute.LongName;
		}
        
		private static string ShortName(CommandLineArgumentAttribute attribute, FieldInfo field)
		{
			return !ExplicitShortName(attribute) ? LongName(attribute, field).Substring(0,1) : attribute.ShortName;
		}
        
		private static bool ExplicitShortName(CommandLineArgumentAttribute attribute)
		{
			return (attribute != null && !attribute.IsDefaultShortName);
		}

		private static Type ElementType(FieldInfo field)
		{
			if (IsCollectionType(field.FieldType))
				return field.FieldType.GetElementType();
			else
				return null;
		}
        
		private static CommandLineArgumentFlags Flags(CommandLineArgumentAttribute attribute, FieldInfo field)
		{
			if (attribute != null)
				return attribute.Flags;
			else if (IsCollectionType(field.FieldType))
				return CommandLineArgumentFlags.MultipleUnique;
			else
				return CommandLineArgumentFlags.AtMostOnce;
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
				longName = CommandLineArgumentParser.LongName(attribute, field);
				explicitShortName = CommandLineArgumentParser.ExplicitShortName(attribute);
				shortName = CommandLineArgumentParser.ShortName(attribute, field);
				elementType = ElementType(field);
				flags = Flags(attribute, field);
				this.field = field;
				seenValue = false;
				this.reporter = reporter;
                if (attribute != null)
                {
                    isDefault = attribute is DefaultCommandLineArgumentAttribute;
                    description = attribute.Description;
                    argValueType = attribute.ArgumentValueType;
                }
			    if (IsCollection)
			        collectionValues = new ArrayList();


			    Debug.Assert(longName != null && longName.Length > 0);
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
			    if (IsCollection)
			        field.SetValue(destination, collectionValues.ToArray(elementType));


			    return ReportMissingRequiredArgument();
			}
            
			private bool ReportMissingRequiredArgument()
			{
				if (IsRequired && !SeenValue)
				{
					if (IsDefault)
						reporter(string.Format("Missing required argument '<{0}>'.", LongName));
					else
						reporter(string.Format("Missing required argument '/{0}'.", LongName));
					return true;
				}
				return false;
			}
            
			private void ReportDuplicateArgumentValue(string value)
			{
				reporter(string.Format("Duplicate '{0}' argument '{1}'", LongName, value));
			}
            
			public bool SetValue(string value, object destination)
			{
				if (SeenValue && !AllowMultiple)
				{
					reporter(string.Format("Duplicate '{0}' argument", LongName));
					return false;
				}
				seenValue = true;
                
				object newValue;
				if (!ParseValue(ValueType, value, out newValue))
					return false;
			    if (IsCollection)
			    {
			        if (Unique && collectionValues.Contains(newValue))
			        {
			            ReportDuplicateArgumentValue(value);
			            return false;
			        }
			        else
			            collectionValues.Add(newValue);
			    }
			    else
			        field.SetValue(destination, newValue);


			    return true;
			}
            
			public Type ValueType
			{
				get { return IsCollection ? elementType : Type; }
			}
            
			private void ReportBadArgumentValue(string value)
			{
				reporter(string.Format("'{0}' is not a valid value for the '{1}' command line option", value, LongName));
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
				get { return longName; }
			}

			public bool ExplicitShortName
			{
				get { return explicitShortName; }
			}
            
			public string ShortName
			{
				get { return shortName; }
			}

		    public string ArgumentValueType
		    {
                get { return argValueType;  }
		    }

			public bool IsRequired
			{
				get { return 0 != (flags & CommandLineArgumentFlags.Required); }
			}
            
			public bool SeenValue
			{
				get { return seenValue; }
			}
            
			public bool AllowMultiple
			{
				get { return 0 != (flags & CommandLineArgumentFlags.Multiple); }
			}
            
			public bool Unique
			{
				get { return 0 != (flags & CommandLineArgumentFlags.Unique); }
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
				get { return isDefault; }
			}

			public string Description
			{
				get { return description; }
			}
            
			private readonly string longName;
			private readonly string shortName;
			private readonly bool explicitShortName;
		    private readonly string argValueType;
			private bool seenValue;
			private readonly FieldInfo field;
			private readonly Type elementType;
			private readonly CommandLineArgumentFlags flags;
			private readonly ArrayList collectionValues;
			private readonly ErrorReporter reporter;
			private readonly bool isDefault;
			private readonly string description;
		}
        
		private readonly List<Argument> arguments;
		private readonly Hashtable argumentMap;
		private readonly Argument defaultArgument;
		private readonly ErrorReporter reporter;
	}
}
