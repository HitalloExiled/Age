using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Collections;
using Age.Core.Extensions;

namespace ThirdParty.Slang;

public unsafe class GlobalSession : Disposable
{
    internal readonly IGlobalSession* Handle;

    public GlobalSession(uint version)
    {
        fixed (IGlobalSession** pHandle = &this.Handle)
        {
            SlangException.ThrowIfInvalid(PInvoke.slang_createGlobalSession(version, pHandle));
        }

        this.Handle->Vtbl->SlangUnknown.AddRef(this.Handle);
    }

    protected override void OnDisposed(bool disposing)
    {
        PInvoke.slang_shutdown();

        this.Handle->Vtbl->SlangUnknown.Release(this.Handle);
    }

    public Session CreateSession(in SessionDesc sessionDesc)
    {
        using var compilerOptionEntries       = new NativeRefArray<Internal.CompilerOptionEntry>(sessionDesc.CompilerOptionEntries.Count);
        using var preprocessorMacros          = new NativeRefArray<Internal.PreprocessorMacroDesc>(sessionDesc.PreprocessorMacros.Count);
        using var searchPaths                 = new NativeStringRefArray(sessionDesc.SearchPaths.Count);
        using var targets                     = new NativeRefArray<Internal.TargetDesc>(sessionDesc.Targets.Count);
        using var targetCompilerOptionEntries = new NativeRefArray<UnsafeArrayBuffer<Internal.CompilerOptionEntry>>(sessionDesc.Targets.Count);

        var stringsCapacity = sessionDesc.CompilerOptionEntries.Count + (sessionDesc.PreprocessorMacros.Count * 2);

        using var strings = new NativeStringRefList(stringsCapacity);

        copyTo(sessionDesc.CompilerOptionEntries, compilerOptionEntries, strings);

        for (var i = 0; i < sessionDesc.PreprocessorMacros.Count; i++)
        {
            var preprocessorMacroDesc = sessionDesc.PreprocessorMacros[i];

            preprocessorMacros[i] = new()
            {
                Name  = getBuffer(strings, preprocessorMacroDesc.Name),
                Value = getBuffer(strings, preprocessorMacroDesc.Value),
            };
        }

        for (var i = 0; i < sessionDesc.SearchPaths.Count; i++)
        {
            searchPaths[i] = sessionDesc.SearchPaths[i];
        }

        var sessionDescTargets = sessionDesc.Targets.AsSpan();

        for (var i = 0; i < sessionDescTargets.Length; i++)
        {
            ref readonly var target = ref sessionDescTargets[i];

            var nativeCompilerOptionEntries = new UnsafeArrayBuffer<Internal.CompilerOptionEntry>(target.CompilerOptionEntries.Count);

            targetCompilerOptionEntries[i] = nativeCompilerOptionEntries;

            copyTo(target.CompilerOptionEntries, nativeCompilerOptionEntries.AsSpan(), strings);

            targets.Buffer[i] = new()
            {
                CompilerOptionEntryCount    = (uint)nativeCompilerOptionEntries.Length,
                CompilerOptionEntries       = nativeCompilerOptionEntries.Buffer,
                Flags                       = target.Flags,
                FloatingPointMode           = target.FloatingPointMode,
                ForceGlslScalarBufferLayout = target.ForceGlslScalarBufferLayout,
                Format                      = target.Format,
                LineDirectiveMode           = target.LineDirectiveMode,
                Profile                     = target.Profile,
            };
        }

        ISlangFileSystem* fileSystem = null;

        if (sessionDesc.FileSystem != null)
        {
            fileSystem = sessionDesc.FileSystem.Handle;
        }

        var sessionDescNative = new Internal.SessionDesc
        {
            AllowGLSLSyntax          = sessionDesc.AllowGLSLSyntax,
            CompilerOptionEntries    = compilerOptionEntries,
            CompilerOptionEntryCount = (uint)compilerOptionEntries.Length,
            DefaultMatrixLayoutMode  = sessionDesc.DefaultMatrixLayoutMode,
            EnableEffectAnnotations  = sessionDesc.EnableEffectAnnotations,
            FileSystem               = fileSystem,
            Flags                    = sessionDesc.Flags,
            PreprocessorMacroCount   = preprocessorMacros.Length,
            PreprocessorMacros       = preprocessorMacros,
            SearchPathCount          = searchPaths.Length,
            SearchPaths              = searchPaths,
            SkipSPIRVValidation      = sessionDesc.SkipSPIRVValidation,
            TargetCount              = targets.Length,
            Targets                  = targets,
        };

        var session = new Session(this, sessionDescNative);

        for (var i = 0; i < targetCompilerOptionEntries.Length; i++)
        {
            targetCompilerOptionEntries[i].Dispose();
        }

        return session;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void copyTo(ReadOnlySpan<CompilerOptionEntry> source, Span<Internal.CompilerOptionEntry> destination, NativeStringRefList strings)
        {
            for (var i = 0; i < source.Length; i++)
            {
                ref readonly var compilerOptionEntry = ref source[i];

                var compilerOptionEntryValue = compilerOptionEntry.Value;

                destination[i] = new()
                {
                    Name  = compilerOptionEntry.Name,
                    Value =
                    {
                        IntValue0    = compilerOptionEntryValue.IntValue0,
                        IntValue1    = compilerOptionEntryValue.IntValue1,
                        Kind         = compilerOptionEntryValue.Kind,
                        StringValue0 = getBuffer(strings, compilerOptionEntryValue.StringValue0),
                        StringValue1 = getBuffer(strings, compilerOptionEntryValue.StringValue1),
                    }
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte* getBuffer(NativeStringRefList strings, string? value)
        {
            strings.Add(value);

            return strings.Buffer[strings.Count - 1];
        }
    }

    public SlangProfileID FindProfile(string name)
    {
        var buffer = MemoryMarshal.CreateUTF8StringBuffer(name);

        var profileID = this.Handle->Vtbl->FindProfile(this.Handle, buffer);

        NativeMemory.Free(buffer);

        return profileID;
    }
}
