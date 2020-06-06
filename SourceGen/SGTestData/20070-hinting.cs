﻿// Copyright 2018 faddenSoft. All Rights Reserved.
// See the LICENSE.txt file for distribution terms (Apache 2.0).

using System;
using System.Collections.Generic;

using PluginCommon;

namespace RuntimeData.Test2011 {
    public class Test2011 : MarshalByRefObject, IPlugin, IPlugin_InlineJsr {
        private IApplication mAppRef;
        private byte[] mFileData;

        public string Identifier {
            get {
                return "Test 2011-hinting";
            }
        }

        public void Prepare(IApplication appRef, byte[] fileData, AddressTranslate addrTrans) {
            mAppRef = appRef;
            mFileData = fileData;

            mAppRef.DebugLog("Test2011(id=" + AppDomain.CurrentDomain.Id + "): prepare()");
        }

        public void Unprepare() {
            mAppRef = null;
            mFileData = null;
        }

        public void CheckJsr(int offset, int operand, out bool noContinue) {
            int ADDR = 0x2456;

            noContinue = false;
            if (offset + 7 < mFileData.Length && operand == ADDR) {
                mAppRef.SetInlineDataFormat(offset + 3, 4, DataType.NumericLE,
                    DataSubType.None, null);
            }
        }
    }
}
