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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using MbUnit.Core.Properties;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Kernel.Collections;

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
        private readonly Type argumentSpecification;
        private List<Argument> arguments;
        private Dictionary<string, Argument> argumentMap;
        private Argument defaultArgument;
	    private readonly IFileManager resourceFileMgr;
       
        /// <summary>
		/// Creates a new command line argument parser.
		/// </summary>
		/// <param name="argumentSpecification">The argument type containing fields decorated
        /// with <see cref="CommandLineArgumentAttribute" /></param>
		public CommandLineArgumentParser(Type argumentSpecification) : this(argumentSpecification, new FileManager())
		{
		}

        /// <summary>
        /// Creates a new command line argument parser.
        /// </summary>
        /// <param name="argumentSpecification">The argument type containing fields decorated
        /// with <see cref="CommandLineArgumentAttribute" /></param>
        /// <param name="fileManager">Object to process resource file.</param>
        public CommandLineArgumentParser(Type argumentSpecification, IFileManager fileManager)
        {
            if (argumentSpecification == null)
                throw new ArgumentNullException(@"argumentSpecification");

            this.argumentSpecification = argumentSpecification;
            resourceFileMgr = fileManager;

            PopulateArgumentMap();
        }

        /// <summary>
		/// Parses an argument list.
		/// </summary>
		/// <param name="args">The arguments to parse.</param>
		/// <param name="destination">The destination of the parsed arguments.</param>
        /// <param name="reporter">The error reporter</param>
		/// <returns>True if no parse errors were encountered.</returns>
		public bool Parse(string[] args, object destination, CommandLineErrorReporter reporter)
		{
            if (args == null || Array.IndexOf(args, null) >= 0)
                throw new ArgumentNullException(@"args");
            if (destination == null)
                throw new ArgumentNullException(@"destination");
            if (!argumentSpecification.IsInstanceOfType(destination))
                throw new ArgumentException(Resources.CommandLineArgumentParser_ArgumentObjectIsOfIncorrectType, @"destination");
            if (reporter == null)
                throw new ArgumentNullException(@"reporter");

            MultiMap<Argument, object> argumentValues = new MultiMap<Argument, object>();
            try
            {
                bool hadError = ParseArgumentList(args, argumentValues, reporter);

                // Finished assigning values.
                foreach (Argument arg in arguments)
                    hadError |= arg.AssignValue(destination, argumentValues, reporter);

                if (defaultArgument != null)
                    hadError |= defaultArgument.AssignValue(destination, argumentValues, reporter);

                return !hadError;
            }
            catch (Exception ex)
            {
                reporter(string.Format(Resources.CommandLineArgumentParser_ExceptionWhileParsing, ex.Message));
                return false;
            }
		}
        
        /// <summary>
        /// Prints a user friendly usage string describing the command line argument syntax.
        /// </summary>
        /// <param name="output">The command line output</param>
        public void ShowUsage(CommandLineOutput output)
        {
            if (output == null)
                throw new ArgumentNullException(@"output");

            foreach (Argument arg in arguments)
            {
                output.PrintArgumentHelp(@"/", arg.LongName, arg.ShortName,
                    arg.Description, arg.ValueLabel, arg.ValueType);
                output.NewLine();
            }

            output.PrintArgumentHelp(@"@", null, null, Resources.CommandLineArgumentParser_ResponseFileDescription,
                Resources.CommandLineArgumentParser_ResponseFileValueLabel, typeof(string));
            output.NewLine();

            output.PrintArgumentHelp(null, null, null, defaultArgument.Description, defaultArgument.ValueLabel, defaultArgument.ValueType);
            output.NewLine();
        }

        private void PopulateArgumentMap()
        {
            arguments = new List<Argument>();
            argumentMap = new Dictionary<string, Argument>();

            foreach (FieldInfo field in argumentSpecification.GetFields())
            {
                if (!field.IsStatic && !field.IsInitOnly && !field.IsLiteral)
                {
                    CommandLineArgumentAttribute attribute = GetAttribute(field);
                    if (attribute is DefaultCommandLineArgumentAttribute)
                    {
                        if (defaultArgument != null)
                            ThrowError(Resources.CommandLineArgumentParser_MoreThanOneDefaultCommandLineArgumentDefined);

                        defaultArgument = new Argument(attribute, field);
                    }
                    else
                    {
                        arguments.Add(new Argument(attribute, field));
                    }
                }
            }

            // add explicit names to map
            foreach (Argument argument in arguments)
            {
                AddArgumentToMap(argument.LongName, argument);

                if (!string.IsNullOrEmpty(argument.ShortName))
                    AddArgumentToMap(argument.ShortName, argument);

                foreach (string synonym in argument.Synonyms)
                    AddArgumentToMap(synonym, argument);
            }
        }

        private void AddArgumentToMap(string argumentName, Argument argument)
        {
            if (argumentMap.ContainsKey(argumentName))
                ThrowError(Resources.CommandLineArgumentParser_DuplicateArgumentName, argumentName);

            argumentMap.Add(argumentName, argument);
        }

        private bool ParseArgumentList(IEnumerable<string> args, MultiMap<Argument, object> argumentValues,
            CommandLineErrorReporter reporter)
        {
            bool hadError = false;

            foreach (string argument in args)
            {
                if (argument.Length == 0)
                    continue;

                switch (argument[0])
                {
                    case '-':
                    case '/':
                        int endIndex = argument.IndexOfAny(new char[] { ':', '+' }, 1);
                        string option = argument.Substring(1, endIndex == -1 ? argument.Length - 1 : endIndex - 1);
                        string optionArgument;
                        if (option.Length + 1 == argument.Length)
                            optionArgument = null;
                        else if (argument.Length > 1 + option.Length && argument[1 + option.Length] == ':')
                            optionArgument = argument.Substring(option.Length + 2);
                        else
                            optionArgument = argument.Substring(option.Length + 1);

                        Argument arg;
                        if (argumentMap.TryGetValue(option, out arg))
                        {
                            hadError |= !arg.AddValue(optionArgument, argumentValues, reporter);
                        }
                        else
                        {
                            ReportUnrecognizedArgument(reporter, argument);
                            hadError = true;
                        }

                        break;

                    case '@':
                        string[] nestedArguments;
                        hadError |= LexFileArguments(argument.Substring(1), reporter, out nestedArguments);

                        if (nestedArguments != null)
                            hadError |= ParseArgumentList(nestedArguments, argumentValues, reporter);
                        break;

                    default:
                        if (defaultArgument != null)
                        {
                            hadError |= !defaultArgument.AddValue(argument, argumentValues, reporter);
                        }
                        else
                        {
                            ReportUnrecognizedArgument(reporter, argument);
                            hadError = true;
                        }
                        break;
                }
            }

            return hadError;
        }

        private static void ReportUnrecognizedArgument(CommandLineErrorReporter reporter, string argument)
        {
            reporter(string.Format(Resources.CommandLineArgumentParser_UnrecognizedArgument, argument));
        }

		private bool LexFileArguments(string fileName, CommandLineErrorReporter reporter, out string[] nestedArguments)
		{
            nestedArguments = null;
            string args = GetResponseFileContext(fileName, reporter);
            if (args == null)
                return true;

            bool hadError = false;

            List<string> argArray = new List<string>();
            StringBuilder currentArg = new StringBuilder();
            bool inQuotes = false;
            int index = 0;
            try
	        {

                for (; ; )
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
	                reporter(string.Format(Resources.CommandLineArgumentParser_MismatchedQuotedInResponseFile, fileName));
	                hadError = true;
	            }
	            else if (currentArg.Length > 0)
	            {
	                // valid argument can be terminated by EOF
	                argArray.Add(currentArg.ToString());
	            }
	        }
        
	        nestedArguments = argArray.ToArray();
		    return hadError;
		}

	    private string GetResponseFileContext(string fileName, CommandLineErrorReporter reporter)
	    {
	        string args = null;
	        try
	        {
	            args = resourceFileMgr.GetFileContent(fileName);
	        }
	        catch (FileNotFoundException)
	        {
	            reporter(string.Format(Resources.CommandLineArgumentParser_ResponseFileDoesNotExist, fileName));
	        }
	        catch (Exception e)
	        {
	            reporter(string.Format(Resources.CommandLineArgumentParser_ErrorOpeningResponseFile, fileName, e.Message));
	        }

	        return args;
	    }

        private static CommandLineArgumentAttribute GetAttribute(ICustomAttributeProvider field)
        {
            object[] attributes = field.GetCustomAttributes(typeof(CommandLineArgumentAttribute), false);
            if (attributes.Length == 0)
                ThrowError(Resources.CommandLineArgumentParser_NoArgumentFields);

            return (CommandLineArgumentAttribute)attributes[0];
        }

        private static void ThrowError(string message, params Object[] args)
        {
            throw new InvalidOperationException(String.Format(message, args));
        }

		private class Argument
		{
            private readonly FieldInfo field;

            private readonly string longName;
            private readonly string shortName;
            private readonly string valueLabel;
            private readonly CommandLineArgumentFlags flags;
            private readonly Type valueType;
            private readonly string description;
            private readonly string[] synonyms;

            private readonly bool isDefault;

            public Argument(CommandLineArgumentAttribute attribute, FieldInfo field)
			{
                this.field = field;

                longName = GetLongName(attribute, field);
				shortName = GetShortName(attribute, field);
				valueType = GetValueType(field);
				flags = GetFlags(attribute, field);

                if (attribute != null)
                {
                    isDefault = attribute is DefaultCommandLineArgumentAttribute;
                    description = attribute.Description;
                    valueLabel = attribute.ValueLabel;
                    synonyms = attribute.Synonyms;
                }
                else
                {
                    synonyms = EmptyArray<string>.Instance;
                }

                if (IsCollection && !AllowMultiple)
                    ThrowError(Resources.CommandLineArgumentParser_Argument_CollectionArgumentsMustAllowMultipleValues);
                if (string.IsNullOrEmpty(longName))
                    ThrowError(Resources.CommandLineArgumentParser_Argument_MissingLongName);
                if (Unique && ! IsCollection)
                    ThrowError(Resources.CommandLineArgumentParser_Argument_InvalidUsageOfUniqueFlag);
                if (! IsValidElementType(valueType))
                    ThrowError(Resources.CommandLineArgumentParser_Argument_UnsupportedValueType);
			}

			public bool AssignValue(object destination, MultiMap<Argument, object> argumentValues,
                CommandLineErrorReporter reporter)
			{
                IList<object> values = argumentValues[this];

                if (IsCollection)
                {
                    Array array = Array.CreateInstance(valueType, values.Count);
                    for (int i = 0; i < values.Count; i++)
                        array.SetValue(values[i], i);

                    field.SetValue(destination, array);
                }
                else if (values.Count != 0)
                {
                    field.SetValue(destination, values[0]);
                }
                else if (IsRequired)
                {
					if (IsDefault)
						reporter(Resources.CommandLineArgumentParser_Argument_MissingRequiredDefaultArgument);
					else
						reporter(string.Format(Resources.CommandLineArgumentParser_Argument_MissingRequiredArgument, LongName));

					return true;
                }

			    return false;
			}

            public bool AddValue(string value, MultiMap<Argument, object> argumentValues,
                CommandLineErrorReporter reporter)
			{
                if (!AllowMultiple && argumentValues.ContainsKey(this))
                {
                    reporter(string.Format(Resources.CommandLineArgumentParser_Argument_DuplicateArgument, LongName));
                    return false;
                }

				object newValue;
                if (!ParseValue(ValueType, value, out newValue))
                {
                    reporter(string.Format(Resources.CommandLineArgumentParser_Argument_InvalidArgumentValue, LongName, value));
                    return false;
                }

                if (Unique && argumentValues.Contains(this, newValue))
                {
                    reporter(string.Format(Resources.CommandLineArgumentParser_Argument_DuplicateArgumentValueExpectedUnique, LongName, value));
                    return false;
                }

                argumentValues.Add(this, newValue);
			    return true;
			}
            
			public Type ValueType
			{
				get { return IsCollection ? valueType : Type; }
			}
            
			private static bool ParseValue(Type type, string stringData, out object value)
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
							if (stringData == null || stringData == @"+")
							{
								value = true;
								return true;
							}
							else if (stringData == @"-")
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
                                
				value = null;
				return false;
			}
            
			public string LongName
			{
				get { return longName; }
			}

			public string ShortName
			{
				get { return shortName; }
			}

            public string[] Synonyms
            {
                get { return synonyms; }
            }

		    public string ValueLabel
		    {
                get { return valueLabel;  }
		    }

			public bool IsRequired
			{
				get { return 0 != (flags & CommandLineArgumentFlags.Required); }
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

            private static string GetLongName(CommandLineArgumentAttribute attribute, FieldInfo field)
            {
                return attribute == null || attribute.IsDefaultLongName ? field.Name : attribute.LongName;
            }

            private static string GetShortName(CommandLineArgumentAttribute attribute, FieldInfo field)
            {
                return attribute == null || attribute.IsDefaultShortName ? GetLongName(attribute, field).Substring(0, 1) : attribute.ShortName;
            }

            private static Type GetValueType(FieldInfo field)
            {
                if (IsCollectionType(field.FieldType))
                    return field.FieldType.GetElementType();
                else
                    return field.FieldType;
            }

            private static CommandLineArgumentFlags GetFlags(CommandLineArgumentAttribute attribute, FieldInfo field)
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
		}
	}
}
