// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Blobs of bytes, cross-compiling 
namespace FSharp.Compiler.AbstractIL.Internal

#if !FABLE_COMPILER
open System.IO
#endif
open Internal.Utilities
open FSharp.Compiler.AbstractIL 
open FSharp.Compiler.AbstractIL.Internal 

module Utils =
    val runningOnMono: bool

module internal Bytes = 
    /// returned int will be 0 <= x <= 255
    val get: byte[] -> int -> int    
    val zeroCreate: int -> byte[]
    /// each int must be 0 <= x <= 255 
    val ofInt32Array: int[] ->  byte[] 
    /// each int will be 0 <= x <= 255 

    val blit: byte[] -> int -> byte[] -> int -> int -> unit

    val stringAsUnicodeNullTerminated: string -> byte[]
    val stringAsUtf8NullTerminated: string -> byte[]

/// May be backed by managed or unmanaged memory, or memory mapped file.
[<AbstractClass>]
type internal ByteMemory =

    abstract Item: int -> byte with get

    abstract Length: int

    abstract ReadBytes: pos: int * count: int -> byte[]

    abstract ReadInt32: pos: int -> int

    abstract ReadUInt16: pos: int -> uint16

    abstract ReadUtf8String: pos: int * count: int -> string

    abstract Slice: pos: int * count: int -> ByteMemory

#if !FABLE_COMPILER
    abstract CopyTo: Stream -> unit
#endif

    abstract Copy: srcOffset: int * dest: byte[] * destOffset: int * count: int -> unit

    abstract ToArray: unit -> byte[]

#if !FABLE_COMPILER
    /// Get a stream representation of the backing memory.
    /// Disposing this will not free up any of the backing memory.
    abstract AsStream: unit -> Stream

    /// Get a stream representation of the backing memory.
    /// Disposing this will not free up any of the backing memory.
    /// Stream cannot be written to.
    abstract AsReadOnlyStream: unit -> Stream
#endif

[<Struct;NoEquality;NoComparison>]
type internal ReadOnlyByteMemory =

    new: ByteMemory -> ReadOnlyByteMemory

    member Item: int -> byte with get

    member Length: int

    member ReadBytes: pos: int * count: int -> byte[]

    member ReadInt32: pos: int -> int

    member ReadUInt16: pos: int -> uint16

    member ReadUtf8String: pos: int * count: int -> string

    member Slice: pos: int * count: int -> ReadOnlyByteMemory

#if !FABLE_COMPILER
    member CopyTo: Stream -> unit
#endif

    member Copy: srcOffset: int * dest: byte[] * destOffset: int * count: int -> unit

    member ToArray: unit -> byte[]

#if !FABLE_COMPILER
    member AsStream: unit -> Stream
#endif

type ByteMemory with

    member AsReadOnly: unit -> ReadOnlyByteMemory

#if !FABLE_COMPILER
    /// Create another ByteMemory object that has a backing memory mapped file based on another ByteMemory's contents.
    static member CreateMemoryMappedFile: ReadOnlyByteMemory -> ByteMemory

    /// Creates a ByteMemory object that has a backing memory mapped file from a file on-disk.
    static member FromFile: path: string * FileAccess * ?canShadowCopy: bool -> ByteMemory

    /// Creates a ByteMemory object that is backed by a raw pointer.
    /// Use with care.
    static member FromUnsafePointer: addr: nativeint * length: int * holder: obj -> ByteMemory
#endif //!FABLE_COMPILER

    /// Creates a ByteMemory object that is backed by a byte array with the specified offset and length.
    static member FromArray: bytes: byte[] * offset: int * length: int -> ByteMemory

    /// Creates a ByteMemory object that is backed by a byte array.
    static member FromArray: bytes: byte[] -> ByteMemory

/// Imperative buffers and streams of byte[]
[<Sealed>]
type internal ByteBuffer = 
    member Close : unit -> byte[] 
    member EmitIntAsByte : int -> unit
    member EmitIntsAsBytes : int[] -> unit
    member EmitByte : byte -> unit
    member EmitBytes : byte[] -> unit
    member EmitByteMemory : ReadOnlyByteMemory -> unit
    member EmitInt32 : int32 -> unit
    member EmitInt64 : int64 -> unit
    member FixupInt32 : pos: int -> value: int32 -> unit
    member EmitInt32AsUInt16 : int32 -> unit
    member EmitBoolAsByte : bool -> unit
    member EmitUInt16 : uint16 -> unit
    member Position : int
    static member Create : int -> ByteBuffer


[<Sealed>]
type internal ByteStream =
    member ReadByte : unit -> byte
    member ReadBytes : int -> ReadOnlyByteMemory
    member ReadUtf8String : int -> string
    member Position : int 
    static member FromBytes : ReadOnlyByteMemory * start:int * length:int -> ByteStream
    
#if LAZY_UNPICKLE
    member CloneAndSeek : int -> ByteStream
    member Skip : int -> unit
#endif
