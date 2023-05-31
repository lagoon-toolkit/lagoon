using Microsoft.JSInterop.WebAssembly;
using System.Runtime.InteropServices;

namespace Lagoon.UI.Internal.BlazorInputFile;

// This is used on WebAssembly
internal class SharedMemoryFileListEntryStream : FileListEntryStream
{
    //private readonly static Type MonoWebAssemblyJSRuntimeType
    //    = Type.GetType("Mono.WebAssembly.Interop.MonoWebAssemblyJSRuntime, Mono.WebAssembly.Interop");
    //private static MethodInfo _cachedInvokeUnmarshalledMethodInfo;

    public SharedMemoryFileListEntryStream(IJSRuntime jsRuntime, ElementReference inputFileElement, FileListEntryImpl file)
        : base(jsRuntime, inputFileElement, file)
    {
    }

    public static bool IsSupported(IJSRuntime jsRuntime)
    {
        return jsRuntime is IJSInProcessRuntime;
    }

    protected override async Task<int> CopyFileDataIntoBuffer(long sourceOffset, byte[] destination, int destinationOffset, int maxBytes, CancellationToken cancellationToken)
    {
        await _jsRuntime.InvokeAsync<string>(
            "BlazorInputFile.ensureArrayBufferReadyForSharedMemoryInterop",
            cancellationToken,
            _inputFileElement,
            _file.Id);

        if (_jsRuntime is WebAssemblyJSRuntime webAssemblyJSRuntime)
        {
            return webAssemblyJSRuntime.InvokeUnmarshalled<ReadRequest, int>("BlazorInputFile.readFileDataSharedMemory", new ReadRequest
            {
                InputFileElementReferenceId = _inputFileElement.Id,
                FileId = _file.Id,
                SourceOffset = sourceOffset,
                Destination = destination,
                DestinationOffset = destinationOffset,
                MaxBytes = maxBytes,
            });
        }
        // Should not be possible
        throw new InvalidOperationException();
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ReadRequest
    {
        [FieldOffset(0)] public string InputFileElementReferenceId;
        [FieldOffset(4)] public int FileId;
        [FieldOffset(8)] public long SourceOffset;
        [FieldOffset(16)] public byte[] Destination;
        [FieldOffset(20)] public int DestinationOffset;
        [FieldOffset(24)] public int MaxBytes;
    }
}
