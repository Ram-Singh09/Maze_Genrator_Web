using System.Runtime.InteropServices;

public unsafe sealed class Ref_List<T> :IDisposable where T : unmanaged
{
    // Vars -----------------------------

    private T* Data;
    private int Size; // actual max size alloc
    public int Length { get; private set; } // volatile, for usage of remove and add

    // Funcs ----------------------------

    public Ref_List(int Len = 1)
    {
        if (Len < 0)
        {
            Console.WriteLine("Cannot use negative size n ref list");
            Size = 32;
        }

        Size = Len;
        Data = (T*)NativeMemory.AllocZeroed((nuint)(Size * sizeof(T)));
        Length = 0;
    }

    public void Add(T Var)
    {
        if (Length >= Size)
        {
            // overflowed to negative side
            if ((Size << 1) < Size)
            {
                Console.WriteLine("Can't doubble the array");
                return;
            }

            Size <<= 1;
            Data = (T*)NativeMemory.Realloc(Data, (nuint)Size * (nuint)sizeof(T));
        }
        
        Data[Length++] = Var;
    }
    public void Add(T[] Arr, int Len)
    {
        Len = Len > Arr.Length || Len < 0 ? Arr.Length : Len;
        
        for (int i = 0; i < Arr.Length; i++)
        {
            if (Length >= Size)
            {
                if ((Size << 1) < Size)
                {
                    Console.WriteLine("Can't doubble the array");
                    return;
                }

                Size <<= 1;
                Data = (T*)NativeMemory.Realloc(Data, (nuint)Size * (nuint)sizeof(T));
            }

            Data[Length++] = Arr[i];
        }
    }

    public void Remove_Last()
    {
        // throw error in future if needs
        if (Length > 0) Length--;
    }
    public void Remove_At(uint index)
    {
        if (index >= Length)
            throw new IndexOutOfRangeException("Wrong idnex provided ");

        // skip if only 1 var to wrok on
        if (Length == 1 || index == Length - 1)
        {
            Length--;
            return;
        }

        // copy 1 section to another
        for (uint i = index; i < Length - 1; i++)
            Data[i] = Data[i + 1];

        // shrink
        Length--;
    }

    public void Save_To_File(string File_path)
    {
        File.WriteAllBytes(File_path, new ReadOnlySpan<byte>(Data, Length));
    }

    public void Clear() => Length = 0;

    public Span<T> As_Span() => new Span<T>(Data, Size);
    public ref T this[int index]
    {
        get
        {
            if ((uint)index >= Length)
                throw new IndexOutOfRangeException("Wrong index provided ");

            return ref Data[index];
        }
    }

    public void Dispose()
    {
        if (Data is null) return;

        NativeMemory.Free(Data);
        Data = null;
        GC.SuppressFinalize(this);
    }
    ~Ref_List() => Dispose();
}
