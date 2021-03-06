//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
                
namespace NBuildKit.MsBuild.Tasks.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines the possible exit results for a guarded execution of a method.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Nuclei.ExceptionHandling", "")]
    internal enum GuardResult
    {
        /// <summary>
        /// There was no exit result. Not normally a valid value.
        /// </summary>
        None,

        /// <summary>
        /// The method executed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The method execution failed at some point with an unhandled exception.
        /// </summary>
        Failure,
    }
}

