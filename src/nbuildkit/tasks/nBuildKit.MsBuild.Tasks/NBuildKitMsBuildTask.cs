﻿//-----------------------------------------------------------------------
// <copyright company="nBuildKit">
// Copyright (c) nBuildKit. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NBuildKit.MsBuild.Tasks
{
    /// <summary>
    /// Defines the base class for implementations of an MsBuild task.
    /// </summary>
    public abstract class NBuildKitMsBuildTask : Task
    {
        private static string AppendDirectorySeparatorChar(string path)
        {
            // Append a slash only if the path is a directory and does not have a slash.
            path = path.Trim();
            if (!Path.HasExtension(path) &&
                !path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }

        /// <summary>
        /// Creates a relative path from one directory to another.
        /// </summary>
        /// <remarks>
        /// Original code here: http://stackoverflow.com/a/275749/539846
        /// </remarks>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.
        /// </exception>
        protected static string GetRelativeDirectoryPath(string fromPath, string toPath)
        {
            if (string.IsNullOrWhiteSpace(fromPath))
            {
                throw new ArgumentNullException("fromPath");
            }

            if (string.IsNullOrWhiteSpace(toPath))
            {
                throw new ArgumentNullException("toPath");
            }

            // The Uri class treats paths that are directories but don't end in a directory separator as files.
            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath.Trim()));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath.Trim()));

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        /// <summary>
        /// Creates a relative path from one file to another.
        /// </summary>
        /// <remarks>
        /// Original code here: http://stackoverflow.com/a/275749/539846
        /// </remarks>
        /// <param name="fromPath">Contains the file path that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start file to the end path.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.
        /// </exception>
        protected static string GetRelativeFilePath(string fromPath, string toPath)
        {
            if (string.IsNullOrWhiteSpace(fromPath))
            {
                throw new ArgumentNullException("fromPath");
            }

            if (string.IsNullOrWhiteSpace(toPath))
            {
                throw new ArgumentNullException("toPath");
            }

            // The Uri class treats paths that are directories but don't end in a directory separator as files.
            Uri fromUri = new Uri(fromPath.Trim());
            Uri toUri = new Uri(toPath.Trim());

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        /// <summary>
        /// Creates a relative path for a file based on a given directory.
        /// </summary>
        /// <param name="fromPath">Contains the file path that defines the start of the relative path.</param>
        /// <param name="directoryPath">The path of the base directory.</param>
        /// <returns>The relative path from the start file to the directory.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="fromPath"/> or <paramref name="directoryPath"/> is <c>null</c>.
        /// </exception>
        protected static string GetFilePathRelativeToDirectory(string fromPath, string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(fromPath))
            {
                throw new ArgumentNullException("fromPath");
            }

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentNullException("directoryPath");
            }

            fromPath = fromPath.Trim();
            var relativeDirectoryPath = GetRelativeDirectoryPath(Path.GetDirectoryName(fromPath), directoryPath.Trim());
            return Path.Combine(
                relativeDirectoryPath,
                Path.GetFileName(fromPath));
        }

        /// <summary>
        /// Gets the verbosity that the current MsBuild instance is running at.
        /// </summary>
        /// <returns>The verbosity of the MsBuild logger.</returns>
        protected static string VerbosityForCurrentMsBuildInstance()
        {
            var regex = new Regex(
                @"(\/|:|;)(v|verbosity)(:|=)(\w*)",
                RegexOptions.IgnoreCase
                | RegexOptions.Multiline
                | RegexOptions.Compiled
                | RegexOptions.Singleline);

            var commandLineArguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < commandLineArguments.Length; i++)
            {
                var argument = commandLineArguments[i];

                var hasSwitch = false;
                var remainder = string.Empty;
                var verbosityMatch = regex.Match(argument);
                if (verbosityMatch.Success)
                {
                    hasSwitch = true;
                    remainder = verbosityMatch.Groups[4].Value;
                }

                if (hasSwitch)
                {
                    if (!string.IsNullOrEmpty(remainder))
                    {
                        return remainder;
                    }
                    else
                    {
                        if (commandLineArguments.Length > i + 1)
                        {
                            return commandLineArguments[i + 1].Trim();
                        }
                    }
                }
            }

            return "normal";
        }

        /// <summary>
        /// Returns the absolute path for the given path item.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The absolute path.</returns>
        protected string GetAbsolutePath(ITaskItem path)
        {
            return GetAbsolutePath(path.ToPath());
        }

        /// <summary>
        /// Returns the absolute path for the given path item.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The absolute path.</returns>
        protected string GetAbsolutePath(string path)
        {
            Log.LogMessage(
                MessageImportance.Low,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Searching for absolute path of {0}",
                    path));

            var result = path;
            if (string.IsNullOrWhiteSpace(result))
            {
                return string.Empty;
            }

            result = result.Trim();
            if (!Path.IsPathRooted(result))
            {
                result = Path.GetFullPath(result);
            }

            return result;
        }
    }
}
