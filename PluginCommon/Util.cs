﻿/*
 * Copyright 2018 faddenSoft
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommonUtil;

namespace PluginCommon {
    /// <summary>
    /// Utility functions available for plugins to use.
    /// 
    /// The idea is to make CommonUtil functions available to plugins while isolating
    /// them from changes to the library.  Anything here is guaranteed to keep working,
    /// while other classes and functions in CommonUtil may change between releases.
    /// </summary>
    public static class Util {
        /// <summary>
        /// Extracts an integer from the data stream.  Integers shorter than 4 bytes are
        /// not sign-extended.
        /// </summary>
        /// <param name="data">Raw data stream.</param>
        /// <param name="offset">Start offset.</param>
        /// <param name="width">Word width, which may be 1-4 bytes.</param>
        /// <param name="isBigEndian">True if word is in big-endian order.</param>
        /// <returns>Value found.</returns>
        public static int GetWord(byte[] data, int offset, int width, bool isBigEndian) {
            return RawData.GetWord(data, offset, width, isBigEndian);
        }

        /// <summary>
        /// Determines whether the provided offset and length are valid for the array.
        /// </summary>
        /// <param name="data">Data array that to check against.</param>
        /// <param name="startOff">Start offset.</param>
        /// <param name="len">Number of bytes.</param>
        /// <returns>True if the specified range falls within the array bounds.</returns>
        public static bool IsInBounds(byte[] data, int startOff, int len) {
            return !(startOff < 0 || len < 0 || startOff >= data.Length || len > data.Length ||
                startOff + len > data.Length);
        }

        /// <summary>
        /// Computes a standard CRC-32 (polynomial 0xedb88320) on a buffer of data.
        /// </summary>
        /// <param name="data">Buffer to process.</param>
        /// <returns>CRC value.</returns>
        public static uint ComputeBufferCRC(byte[] data) {
            return CRC32.OnBuffer(0, data, 0, data.Length);
        }

        /// <summary>
        /// Formats the byte that follows a BRK instruction.  How we do this depends on
        /// whether the system is configured for two-byte BRKs.
        /// </summary>
        /// <remarks>
        /// We can actually apply the format both ways and let the app ignore the one it
        /// doesn't like, but this is cleaner.
        /// </remarks>
        /// <param name="appRef">Reference to application object.</param>
        /// <param name="twoByteBrk">True if BRKs are handled as two-byte instructions.</param>
        /// <param name="brkOffset">Offset of BRK instruction.</param>
        /// <param name="type">Data type to apply.</param>
        /// <param name="subType">Data sub-type to apply.</param>
        /// <param name="label">Label, for subType=Symbol.</param>
        public static void FormatBrkByte(IApplication appRef, bool twoByteBrk, int brkOffset,
                DataSubType subType, string label) {
            if (twoByteBrk) {
                // Two-byte BRK, so we want to apply the format to the instruction itself.
                appRef.SetOperandFormat(brkOffset, subType, label);
            } else {
                // Single-byte BRK, so we want to format the byte that follows the
                // instruction as inline data.
                appRef.SetInlineDataFormat(brkOffset + 1, 1, DataType.NumericLE, subType, label);
            }
        }

        /// <summary>
        /// Converts four 8-bit color values to a single 32-bit ARGB value.  Values should
        /// not be pre-multiplied.
        /// </summary>
        /// <param name="a">Alpha channel.</param>
        /// <param name="r">Red.</param>
        /// <param name="g">Green.</param>
        /// <param name="b">Blue.</param>
        /// <returns>Combined value.</returns>
        public static int MakeARGB(int a, int r, int g, int b) {
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Extracts a typed value from a dictionary with plain object values.
        /// </summary>
        /// <typeparam name="T">Type of value to retrieve.</typeparam>
        /// <param name="dict">Dictionary with values.</param>
        /// <param name="key">Entry to find.</param>
        /// <param name="defVal">Default value.</param>
        /// <returns>Value found, or the default if the key doesn't exist or the value has the
        ///   wrong type.</returns>
        public static T GetFromObjDict<T>(ReadOnlyDictionary<string, object> dict, string key,
                T defVal) {
            if (dict.TryGetValue(key, out object objVal)) {
                if (objVal is T) {
                    return (T)objVal;
                }
            }
            return defVal;
        }
    }
}
