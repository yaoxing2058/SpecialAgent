﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NeoSaveGames.Serialization
{
    public interface INeoDeserializer
    {
        bool ReadFromStream(Stream stream);
        void BeginDeserialization();
        void EndDeserialization();

        bool isDeserializing { get; }

        bool PushContext(SerializationContext context, int id);
        void PopContext(SerializationContext context, int id);// Use type for error checking

        bool TryReadValue(string key, out bool output, bool defaultValue);
        bool TryReadValue(string key, out byte output, byte defaultValue);
        bool TryReadValue(string key, out sbyte output, sbyte defaultValue);
        bool TryReadValue(string key, out char output, char defaultValue);
        bool TryReadValue(string key, out short output, short defaultValue);
        bool TryReadValue(string key, out ushort output, ushort defaultValue);
        bool TryReadValue(string key, out int output, int defaultValue);
        bool TryReadValue(string key, out uint output, uint defaultValue);
        bool TryReadValue(string key, out long output, long defaultValue);
        bool TryReadValue(string key, out ulong output, ulong defaultValue);
        bool TryReadValue(string key, out float output, float defaultValue);
        bool TryReadValue(string key, out double output, double defaultValue);
        bool TryReadValue(string key, out Vector2 output, Vector2 defaultValue);
        bool TryReadValue(string key, out Vector3 output, Vector3 defaultValue);
        bool TryReadValue(string key, out Vector4 output, Vector4 defaultValue);
        bool TryReadValue(string key, out Vector2Int output, Vector2Int defaultValue);
        bool TryReadValue(string key, out Vector3Int output, Vector3Int defaultValue);
        bool TryReadValue(string key, out Quaternion output, Quaternion defaultValue);
        bool TryReadValue(string key, out Color output, Color defaultValue);
        bool TryReadValue(string key, out Color32 output, Color32 defaultValue);
        bool TryReadValue(string key, out Guid output);
        bool TryReadValue(string key, out DateTime output, DateTime defaultValue);
        bool TryReadValue(string key, out string output, string defaultValue);
        bool TryReadValues(string key, out bool[] output, bool[] defaultValues);
        bool TryReadValues(string key, out byte[] output, byte[] defaultValues);
        bool TryReadValues(string key, out sbyte[] output, sbyte[] defaultValues);
        bool TryReadValues(string key, out char[] output, char[] defaultValues);
        bool TryReadValues(string key, out short[] output, short[] defaultValues);
        bool TryReadValues(string key, out ushort[] output, ushort[] defaultValues);
        bool TryReadValues(string key, out int[] output, int[] defaultValues);
        bool TryReadValues(string key, out uint[] output, uint[] defaultValues);
        bool TryReadValues(string key, out long[] output, long[] defaultValues);
        bool TryReadValues(string key, out ulong[] output, ulong[] defaultValues);
        bool TryReadValues(string key, out float[] output, float[] defaultValues);
        bool TryReadValues(string key, out double[] output, double[] defaultValues);
        bool TryReadValues(string key, out string[] output, string[] defaultValues);
        bool TryReadValues(string key, out Vector2[] output, Vector2[] defaultValues);
        bool TryReadValues(string key, out Vector3[] output, Vector3[] defaultValues);
        bool TryReadValues(string key, out Vector4[] output, Vector4[] defaultValues);
        bool TryReadValues(string key, out Vector2Int[] output, Vector2Int[] defaultValues);
        bool TryReadValues(string key, out Vector3Int[] output, Vector3Int[] defaultValues);
        bool TryReadValues(string key, out Quaternion[] output, Quaternion[] defaultValues);
        bool TryReadValues(string key, out Color[] output, Color[] defaultValues);
        bool TryReadValues(string key, out Color32[] output, Color32[] defaultValues);
        bool TryReadValues(string key, out Guid[] output);
        bool TryReadValues(string key, List<bool> output);
        bool TryReadValues(string key, List<byte> output);
        bool TryReadValues(string key, List<sbyte> output);
        bool TryReadValues(string key, List<char> output);
        bool TryReadValues(string key, List<short> output);
        bool TryReadValues(string key, List<ushort> output);
        bool TryReadValues(string key, List<int> output);
        bool TryReadValues(string key, List<uint> output);
        bool TryReadValues(string key, List<long> output);
        bool TryReadValues(string key, List<ulong> output);
        bool TryReadValues(string key, List<float> output);
        bool TryReadValues(string key, List<double> output);
        bool TryReadValues(string key, List<string> output);
        bool TryReadValues(string key, List<Vector2> output);
        bool TryReadValues(string key, List<Vector3> output);
        bool TryReadValues(string key, List<Vector4> output);
        bool TryReadValues(string key, List<Vector2Int> output);
        bool TryReadValues(string key, List<Vector3Int> output);
        bool TryReadValues(string key, List<Quaternion> output);
        bool TryReadValues(string key, List<Color> output);
        bool TryReadValues(string key, List<Color32> output);
        bool TryReadValues(string key, List<Guid> output);
        bool TryReadSerializable<T>(string key, out T output, T defaultValue);
        bool TryReadSerializables<T>(string key, out T[] output, T[] defaultValues);
        bool TryReadSerializables<T>(string key, List<T> output);
        bool TryReadComponentReference<T>(string key, out T output, NeoSerializedGameObject pathFrom) where T : class;
        bool TryReadTransformReference(string key, out Transform output, NeoSerializedGameObject pathFrom);
        bool TryReadGameObjectReference(string key, out GameObject output, NeoSerializedGameObject pathFrom);
        bool TryReadNeoSerializedGameObjectReference(string key, out NeoSerializedGameObject output, NeoSerializedGameObject pathFrom);

        bool TryReadValue(int hash, out bool output, bool defaultValue);
        bool TryReadValue(int hash, out byte output, byte defaultValue);
        bool TryReadValue(int hash, out sbyte output, sbyte defaultValue);
        bool TryReadValue(int hash, out char output, char defaultValue);
        bool TryReadValue(int hash, out short output, short defaultValue);
        bool TryReadValue(int hash, out ushort output, ushort defaultValue);
        bool TryReadValue(int hash, out int output, int defaultValue);
        bool TryReadValue(int hash, out uint output, uint defaultValue);
        bool TryReadValue(int hash, out long output, long defaultValue);
        bool TryReadValue(int hash, out ulong output, ulong defaultValue);
        bool TryReadValue(int hash, out float output, float defaultValue);
        bool TryReadValue(int hash, out double output, double defaultValue);
        bool TryReadValue(int hash, out Vector2 output, Vector2 defaultValue);
        bool TryReadValue(int hash, out Vector3 output, Vector3 defaultValue);
        bool TryReadValue(int hash, out Vector4 output, Vector4 defaultValue);
        bool TryReadValue(int hash, out Vector2Int output, Vector2Int defaultValue);
        bool TryReadValue(int hash, out Vector3Int output, Vector3Int defaultValue);
        bool TryReadValue(int hash, out Quaternion output, Quaternion defaultValue);
        bool TryReadValue(int hash, out Color output, Color defaultValue);
        bool TryReadValue(int hash, out Color32 output, Color32 defaultValue);
        bool TryReadValue(int hash, out Guid output);
        bool TryReadValue(int hash, out DateTime output, DateTime defaultValue);
        bool TryReadValue(int hash, out string output, string defaultValue);
        bool TryReadValues(int hash, out bool[] output, bool[] defaultValues);
        bool TryReadValues(int hash, out byte[] output, byte[] defaultValues);
        bool TryReadValues(int hash, out sbyte[] output, sbyte[] defaultValues);
        bool TryReadValues(int hash, out char[] output, char[] defaultValues);
        bool TryReadValues(int hash, out short[] output, short[] defaultValues);
        bool TryReadValues(int hash, out ushort[] output, ushort[] defaultValues);
        bool TryReadValues(int hash, out int[] output, int[] defaultValues);
        bool TryReadValues(int hash, out uint[] output, uint[] defaultValues);
        bool TryReadValues(int hash, out long[] output, long[] defaultValues);
        bool TryReadValues(int hash, out ulong[] output, ulong[] defaultValues);
        bool TryReadValues(int hash, out float[] output, float[] defaultValues);
        bool TryReadValues(int hash, out double[] output, double[] defaultValues);
        bool TryReadValues(int hash, out string[] output, string[] defaultValues);
        bool TryReadValues(int hash, out Vector2[] output, Vector2[] defaultValues);
        bool TryReadValues(int hash, out Vector3[] output, Vector3[] defaultValues);
        bool TryReadValues(int hash, out Vector4[] output, Vector4[] defaultValues);
        bool TryReadValues(int hash, out Vector2Int[] output, Vector2Int[] defaultValues);
        bool TryReadValues(int hash, out Vector3Int[] output, Vector3Int[] defaultValues);
        bool TryReadValues(int hash, out Quaternion[] output, Quaternion[] defaultValues);
        bool TryReadValues(int hash, out Color[] output, Color[] defaultValues);
        bool TryReadValues(int hash, out Color32[] output, Color32[] defaultValues);
        bool TryReadValues(int hash, out Guid[] output);
        bool TryReadValues(int hash, List<bool> output);
        bool TryReadValues(int hash, List<byte> output);
        bool TryReadValues(int hash, List<sbyte> output);
        bool TryReadValues(int hash, List<char> output);
        bool TryReadValues(int hash, List<short> output);
        bool TryReadValues(int hash, List<ushort> output);
        bool TryReadValues(int hash, List<int> output);
        bool TryReadValues(int hash, List<uint> output);
        bool TryReadValues(int hash, List<long> output);
        bool TryReadValues(int hash, List<ulong> output);
        bool TryReadValues(int hash, List<float> output);
        bool TryReadValues(int hash, List<double> output);
        bool TryReadValues(int hash, List<string> output);
        bool TryReadValues(int hash, List<Vector2> output);
        bool TryReadValues(int hash, List<Vector3> output);
        bool TryReadValues(int hash, List<Vector4> output);
        bool TryReadValues(int hash, List<Vector2Int> output);
        bool TryReadValues(int hash, List<Vector3Int> output);
        bool TryReadValues(int hash, List<Quaternion> output);
        bool TryReadValues(int hash, List<Color> output);
        bool TryReadValues(int hash, List<Color32> output);
        bool TryReadValues(int hash, List<Guid> output);
        bool TryReadSerializable<T>(int hash, out T output, T defaultValue);
        bool TryReadSerializables<T>(int hash, out T[] output, T[] defaultValue);
        bool TryReadSerializables<T>(int hash, List<T> output);
        bool TryReadComponentReference<T>(int hash, out T output, NeoSerializedGameObject pathFrom) where T : class;
        bool TryReadTransformReference(int hash, out Transform output, NeoSerializedGameObject pathFrom);
        bool TryReadGameObjectReference(int hash, out GameObject output, NeoSerializedGameObject pathFrom);
        bool TryReadNeoSerializedGameObjectReference(int hash, out NeoSerializedGameObject output, NeoSerializedGameObject pathFrom);
    }
}