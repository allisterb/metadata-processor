// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace nanoFramework.Tools.MetadataProcessor.Console
{
    /// <summary>
    /// qflat: pins the culture before <c>Main</c> runs. MetadataProcessor.Shared orders its tables with
    /// culture-sensitive comparisons, so without this the emitted .pe depends on the host's culture.
    /// Output still depends on the collation engine: ICU (the .NET default everywhere) and Windows NLS
    /// (upstream's net472 tool) order names differing only in punctuation (/ . _ - ') differently.
    /// </summary>
    internal static class CultureInitializer
    {
#pragma warning disable CA2255 // Module initializers in an app: intended, Program.cs is upstream's and stays unmodified.
        [ModuleInitializer]
        internal static void Initialize()
        {
            // .NET's invariant-globalization mode (DOTNET_SYSTEM_GLOBALIZATION_INVARIANT, which overrides
            // the runtimeconfig setting) collates ordinally: 'B' (0x42) sorts before 'a' (0x61), where
            // linguistic collation puts 'a' first. Tables would then be ordered differently, so refuse.
            if (string.Compare("a", "B", StringComparison.InvariantCulture) > 0)
            {
                System.Console.Error.WriteLine(
                    "error: .NET invariant-globalization mode is enabled; it changes the order of the emitted " +
                    "tables. Unset DOTNET_SYSTEM_GLOBALIZATION_INVARIANT (or set it to false) and run again.");
                Environment.Exit(2);
            }

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        }
#pragma warning restore CA2255
    }
}
