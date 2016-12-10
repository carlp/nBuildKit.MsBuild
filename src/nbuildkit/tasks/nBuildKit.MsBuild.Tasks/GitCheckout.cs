﻿//-----------------------------------------------------------------------
// <copyright company="nBuildKit">
// Copyright (c) nBuildKit. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace NBuildKit.MsBuild.Tasks
{
    /// <summary>
    /// Defines a <see cref="ITask"/> that performs a Git checkout.
    /// </summary>
    public sealed class GitCheckout : GitCommandLineToolTask
    {
        /// <summary>
        /// Gets or sets the branch that should be checked out.
        /// </summary>
        [Required]
        public string Branch
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public override bool Execute()
        {
            var arguments = new List<string>();
            {
                arguments.Add(string.Format("checkout \"{0}\" ", Branch.TrimEnd('\\')));
                arguments.Add("--quiet ");
            }

            InvokeGit(arguments);

            return !Log.HasLoggedErrors;
        }
    }
}
