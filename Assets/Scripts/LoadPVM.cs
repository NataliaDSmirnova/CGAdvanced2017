using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
public class LoadPVM {
    //fields
    private int currentBit = 0;
    private byte currentByte;

    private const string DDS_ID = "DDS v3d\n";
    private const string DDS_ID2 = "DDS v3e\n";
    private const uint DDS_BLOCKSIZE = 1 << 20;
    private const uint DDS_INTERLEAVE = 1 << 24;
    private const uint DDS_RL = 7;
    // properties
    // methods

    // We need 2^n dimensions
    uint countNewDim(uint Dim)
    {
        uint newDim = 1;
        while (newDim < Dim)
        {
            newDim *= 2;
        }
        return newDim;
    }
    

    byte[] compress16to8(byte[] inputData)
    {
        int newLength = inputData.Length / 2;
        byte[] retArray = new byte[newLength];
        for (int i = 0; i < newLength; ++i)
        {
            retArray[i] = inputData[i * 2 + 1];
        }
        return retArray;
    }


    public Color32[] LoadPVMFile(string volumeName, ref uint width, ref uint height, ref uint depth, ref uint components)
    {
        currentBit = 0;
        byte[] raw = ReadPVMVolume(volumeName, ref width, ref height, ref depth, ref components);
        uint nwidth, nheight, ndepth;
        nwidth = countNewDim(width);
        nheight = countNewDim(height);
        ndepth = countNewDim(depth);
        
        uint nsize = nwidth * nheight * ndepth;
        Color32[] retArr = new Color32[nsize];
        for (uint i = 0; i < nsize; ++i)
        {
            retArr[i] = new Color32(0, 0, 0, 255);
        }
        if (components == 1)
        {
            for (uint z = 0; z < depth; ++z)
            {
                for (uint y = 0; y < height; ++y)
                {
                    for (uint x = 0; x < width; ++x)
                    {
                        uint nIndex = x + y * nwidth + z * nwidth * nheight;
                        uint oIndex = x + y * width + z * width * height;
                        retArr[nIndex].r = raw[oIndex];
                        retArr[nIndex].g = raw[oIndex];
                        retArr[nIndex].b = raw[oIndex];
                    }
                }
            }
        }
        else if (components == 3)
        {
            for (uint z = 0; z < depth; ++z)
            {
                for (uint y = 0; y < height; ++y)
                {
                    for (uint x = 0; x < width; ++x)
                    {
                        uint nIndex = x + y * nwidth + z * nwidth * nheight;
                        uint oIndex = (x + y * width + z * width * height) * 3;

                        retArr[nIndex].r = raw[oIndex];
                        retArr[nIndex].g = raw[oIndex + 1];
                        retArr[nIndex].b = raw[oIndex + 2];

                    }
                }
            }
        }
        else if (components == 2)
        {
            byte[] nRaw = compress16to8(raw);
            for (uint z = 0; z < depth; ++z)
            {
                for (uint y = 0; y < height; ++y)
                {
                    for (uint x = 0; x < width; ++x)
                    {
                        uint nIndex = x + y * nwidth + z * nwidth * nheight;
                        uint oIndex = x + y * width + z * width * height;

                        retArr[nIndex].r = nRaw[oIndex];
                        retArr[nIndex].g = nRaw[oIndex];
                        retArr[nIndex].b = nRaw[oIndex];

                    }
                }
            }
        }
        else
        {
            GameObject.Find("UICanvas").transform.FindChild("PanelLog").
                transform.FindChild("TextLog").GetComponent<Text>().text += "Component Error\n";
            Debug.Log("Component Error");
        }

        width = nwidth;
        height = nheight;
        depth = ndepth;
        return retArr;
    }


    private bool? ReadBit(ref BinaryReader binReader)
    {
        if (currentBit == 0 || currentBit == 8)
        {
            if (binReader.BaseStream.Length == binReader.BaseStream.Position)
            {
                return null;
            }
            currentByte = binReader.ReadByte();
            currentBit = 0;
        }
        bool outputBit;

        outputBit = (currentByte & (1 << (7 - currentBit))) > 0;
        
        ++currentBit;
        return outputBit;
    }



    private uint ReadNBits(ref BinaryReader binReader, int bits)
    {
        uint outputNBits = 0;
        for (int i = 0; i < bits; ++i)
        {
            outputNBits = outputNBits << 1;
            bool? curBit = ReadBit(ref binReader);
            if (curBit == null)
            {
                return 0;
            }
            outputNBits = outputNBits | Convert.ToUInt16(curBit);
        }
        return outputNBits;
    }
    
    
    private int CodeDDS(int bits)
    {
        return (bits > 1 ? bits - 1 : bits);
    }

    private int DecodeDDS(int bits)
    {
        return (bits >= 1 ? bits + 1 : bits);
    }
    
    private void CopyBytes(ref byte[] data, byte[] data2, uint shift, uint length)
    {
        for (uint i = 0; i < length; ++i)
        {
            data[i + shift] = data2[i];
        }
    }

    // deinterleave a byte stream
    private void DeinterleaveByteStream(ref byte[] dataArray, uint bytes, uint skip, uint block = 0, bool restore = false)
    {
        uint i, j, k;
        uint ind = 0;
        byte []tempByteArray;

        if (skip <= 1)
        {
            return;
        }

        if (block == 0)
        {
            tempByteArray = new byte[bytes];
            if (!restore)
            {
                ind = 0;
                for (i = 0; i < skip; ++i)
                {
                    for (j = i; j < bytes; j += skip)
                    {
                        tempByteArray[ind++] = dataArray[j];
                    }
                }
            }
            else
            {
                ind = 0;
                for (i = 0; i < skip; ++i)
                {
                    for (j = i; j < bytes; j += skip)
                    {
                        tempByteArray[j] = dataArray[ind++];
                    }
                }
            }
            CopyBytes(ref dataArray, tempByteArray, 0, bytes);
        }
        else
        {
            tempByteArray = new byte[(bytes < skip * block) ? bytes : skip * block];
            if (!restore)
            {
                for (k = 0; k<bytes / skip / block; ++k)
                {
                    ind = 0;
                    for (i = 0; i < skip; ++i)
                    {
                        for (j = i; j < skip * block; j += skip)
                        {
                            tempByteArray[ind++] = dataArray[k * skip * block + j];
                        }
                    }
                    CopyBytes(ref dataArray, tempByteArray, k * skip * block, skip * block);

                }
                ind = 0;
                for (i = 0; i < skip; ++i)
                {
                    for (j = i; j < bytes - k * skip * block; j += skip)
                    {
                        tempByteArray[ind++] = dataArray[k * skip * block + j];
                    }
                }
                CopyBytes(ref dataArray, tempByteArray, k * skip * block, bytes - k * skip * block);
            }
            else
            {
                for (k = 0; k<bytes / skip / block; ++k)
                {
                    ind = k * skip * block;
                    for (i = 0; i < skip; ++i)
                    {
                        for (j = i; j < skip * block; j += skip)
                        {
                            tempByteArray[j] = dataArray[ind++];
                        }
                    }
                    CopyBytes(ref dataArray, tempByteArray, k * skip * block, skip * block);
                }
                ind = k * skip * block;
                for (i = 0; i < skip; ++i)
                {
                    for (j = i; j < bytes - k * skip * block; j += skip)
                    {
                        tempByteArray[j] = dataArray[ind++];
                    }
                }
                CopyBytes(ref dataArray, tempByteArray, k * skip * block, bytes - k * skip * block);
            }
        }
        
    }
    // interleave a byte stream
    private void InterleaveByteStream(ref byte[] dataArray, uint bytes, uint skip, uint block = 0)
    {
        DeinterleaveByteStream(ref dataArray, bytes, skip, block, true);
    }
    
    // decode a Differential Data Stream
    
    private byte[] DecodeDataStream(ref BinaryReader binReader, ref uint bytes, uint block)
    {
        uint skip, strip;
        byte[] dataArray = new byte[0];

        uint cnt, cnt1, cnt2;
        int bits, act;
        
        skip = ReadNBits(ref binReader, 2) + 1;
        strip = ReadNBits(ref binReader, 16) + 1;
        
        cnt = 0;
        act = 0;

        while ((cnt1 = ReadNBits(ref binReader, (int)DDS_RL)) != 0)
        {
            bits = DecodeDDS((int)ReadNBits(ref binReader, 3));

            for (cnt2 = 0; cnt2 < cnt1; ++cnt2)
            {
                if (strip == 1 || cnt <= strip)
                {
                    act += (int)ReadNBits(ref binReader, bits) - (1 << bits) / 2;
                }
                else
                {
                    act += dataArray[cnt - strip] - dataArray[cnt - strip - 1] + (int)ReadNBits(ref binReader, bits) - (1 << bits) / 2;
                }

                while (act < 0)
                {
                    act += 256;
                }

                while (act > 255)
                {
                    act -= 256;
                }

                if (cnt % DDS_BLOCKSIZE == 0)
                {
                    byte[] data2 = new byte[dataArray.Length + DDS_BLOCKSIZE];
                    dataArray.CopyTo(data2, 0);
                    dataArray = data2;
                }
                dataArray[cnt] = (byte)act;
                ++cnt;
            }
        }
        InterleaveByteStream(ref dataArray, cnt, skip, block);
        bytes = cnt;
        return dataArray;
    }
    // read a RAW file
    private byte[] ReadRAWFile(string volumeName, ref uint bytes)
    {
        BinaryReader binReader = new BinaryReader(File.Open("Assets/Resources/" + volumeName, FileMode.Open));
        byte[] dataArray = binReader.ReadBytes((int)binReader.BaseStream.Length);
        bytes = (uint)binReader.BaseStream.Length;
        return dataArray;
    }
    
    // read a Differential Data Stream
    private byte[] ReadDDSFile(string volumeName, ref uint bytes, ref bool hasDDSHead)
    {
        int version = 1;
        Stream inputStream = File.Open("Assets/Resources/" + volumeName, FileMode.Open);
        BinaryReader binReader = new BinaryReader(inputStream);
        
        byte[] headLine = binReader.ReadBytes(8);
        hasDDSHead = true;
        
        byte[] dataArray;

        for (int i = 0; i < DDS_ID.Length; ++i)
        {
            if (DDS_ID[i] != headLine[i])
            {
                version = 0;
            }
        }
        if (version == 0)
        {
            for (int i = 0; i < DDS_ID2.Length; ++i)
            {
                if (DDS_ID2[i] != headLine[i])
                {
                    hasDDSHead = false;
                    version = 0;
                    return null;
                }
                version = 2;
            }
        }
        dataArray = DecodeDataStream(ref binReader, ref bytes, version == 1 ? 0 : DDS_INTERLEAVE);
        
        return dataArray;
    }
    

    bool CompareStrChunk(byte[] dataArray, string str, uint start, uint length)
    {
        if (start + length > dataArray.Length || str.Length < length)
        {
            return false;
        }
        for (int i = 0; i < length; ++i)
        {
            if (str[i] != dataArray[start + i])
            {
                return false;
            }
        }
        return true;
    }

    // read a compressed PVM volume
    private byte[] ReadPVMVolume(string volumeName, ref uint width, ref uint height, ref uint depth, ref uint components)
    {
        byte[] data;
        uint bytes = 0, numc;

        byte[] volume = null;
        
        bool hadDDSHead = false;
        data = ReadDDSFile(volumeName, ref bytes, ref hadDDSHead);
        if (hadDDSHead == false)
        {
            data = ReadRAWFile(volumeName, ref bytes);
        }

        if (bytes < 5)
        {
            return null;
        }
        
        if (CompareStrChunk(data, "PVM\n", 0, 4) == false)
        {
            if (CompareStrChunk(data, "PVM2\n", 0, 5) == true)
            {
                //version = 2;
            }
            else if (CompareStrChunk(data, "PVM3\n", 0, 5) == true)
            {
                //version = 3;
            }
            else
            {
                return null;
            }
            string result = System.Text.Encoding.ASCII.GetString(data);

            string[] lines = result.Split('\n');
            string[] dimInfo = lines[1].Split(' ');
            width = uint.Parse(dimInfo[0]);
            height = uint.Parse(dimInfo[1]);
            depth = uint.Parse(dimInfo[2]);
            string[] compon = lines[3].Split(' ');
            numc = uint.Parse(compon[0]);
            components = numc;
            int line4Length = lines[0].Length + lines[1].Length + lines[2].Length + lines[3].Length + 4;
            uint allbytes = width * height * depth * numc;
            volume = new byte[allbytes];
            
            for (uint i = 0; i < allbytes; ++i)
            {
                volume[i] = data[i + line4Length];
            }
        }
        else
        {
            int ind = 4;
            while (data[ind] == '#')
            {
                while (data[ind++] != '\n') ;
            }
            string result = System.Text.Encoding.ASCII.GetString(data, ind, data.Length - ind);
            string[] lines = result.Replace("\r\n", "\n").Split('\n');
            string[] dimInfo = lines[0].Split(' ');
            width = uint.Parse(dimInfo[0]);
            height = uint.Parse(dimInfo[1]);
            depth = uint.Parse(dimInfo[2]);
            string[] compon = lines[1].Split(' ');
            numc = uint.Parse(compon[0]);
            components = numc;
        }
        return volume;
    }
}
