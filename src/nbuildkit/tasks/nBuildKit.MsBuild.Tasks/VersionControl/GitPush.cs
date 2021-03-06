﻿//-----------------------------------------------------------------------
// <copyright company="nBuildKit">
// Copyright (c) nBuildKit. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using Microsoft.Build.Framework;
using NBuildKit.MsBuild.Tasks.Core;

namespace NBuildKit.MsBuild.Tasks.VersionControl
{
    /// <summary>
    /// Defines a <see cref="ITask"/> that performs a Git push.
    /// </summary>
    public sealed class GitPush : GitCommandLineToolTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitPush"/> class.
        /// </summary>
        public GitPush()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitPush"/> class.
        /// </summary>
        /// <param name="invoker">The object which handles the invocation of the command line applications.</param>
        public GitPush(IApplicationInvoker invoker)
            : base(invoker)
        {
        }

        /// <summary>
        /// Gets or sets the branch that should be pushed to the remote repository.
        /// </summary>
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
                arguments.Add("push origin ");
                if (!string.IsNullOrWhiteSpace(Branch))
                {
                    arguments.Add(string.Format(CultureInfo.InvariantCulture, "\"{0}\" ", Branch.TrimEnd('\\')));
                }
                else
                {
                    arguments.Add("--all ");
                }

                if (PushTags && !string.IsNullOrWhiteSpace(Branch))
                {
                    arguments.Add("--tags ");
                }

                arguments.Add("--porcelain --atomic ");
            }

            InvokeGit(arguments);

            return !Log.HasLoggedErrors;
        }

        /// <summary>
        /// Gets or sets a value indicating whether tags should be pushed to the remote repository.
        /// </summary>
        public bool PushTags
        {
            get;
            set;
        }
    }
}
