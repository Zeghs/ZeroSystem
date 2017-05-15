using System;
using Zeghs.Data;
using Mitake.Stock.Data;
using Mitake.Stock.Util;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Decode {
        /*
         *  成交資訊(上市:9999、上櫃:9998、興櫃:9994)：[type:1] = [33]16
         *  [data:N] = [時間:2] [format:1] [成交總額:n1] [成交張數:n2] [成交筆數:n3]
         */
        /// <summary>
        ///   大盤成交資訊
        /// </summary>
        internal sealed class Decode_S33 {
                internal static void Decode(MitakeIndex index, PacketBuffer Buffer) {
                        byte bMode = 0, bFlag = 0;
			MitakeIndexTick cTick = null;
                        
                        //移動至資料結構
                        Buffer.Position = 7;

			DateTime cTime = Time.GetTime(Buffer); //取得時間
			bool bHave = index.GetMitakeTick(cTime, ref cTick);
			cTick.SetFlag(2);

			MitakeIndexTick cPrevTick = null;
			if (index.ComplementStatus != ComplementStatus.NotComplement) {
				cPrevTick = index.GetPreviousTick(cTime, 2);
				if (cPrevTick != null) {
					cTick.Clone(cPrevTick);
				}
			}

                        //取得format
                        bFlag = Buffer[0];
                        ++Buffer.Position;

                        //取得成交總額
                        bMode = BitConvert.GetValue(bFlag, 6, 2);

			uint uVolume = Volumn.GetVolumn(bMode, Buffer);

			cTick.Volume = uVolume;
			if (uVolume > index.成交總額) {
				index.成交總額 = uVolume;
			}

                        //取得成交張數
                        bMode = BitConvert.GetValue(bFlag, 4, 2);
                        cTick.成交張數 = Volumn.GetVolumn(bMode, Buffer);

                        //取得成交筆數
                        bMode = BitConvert.GetValue(bFlag, 2, 2);
                        cTick.成交筆數 = Volumn.GetVolumn(bMode, Buffer);

			CalculateSingle(cTick, cPrevTick);
			++index.UpdateCount;
		}

		//計算單量
		internal static void CalculateSingle(MitakeIndexTick current, MitakeIndexTick previous) {
			if (previous == null) {
				current.Single = current.Volume;
				current.成交張數單量 = current.成交張數;
				current.成交筆數單量 = current.成交筆數;
			} else {
				current.Single = current.Volume - previous.Volume;
				current.成交張數單量 = current.成交張數 - previous.成交張數;
				current.成交筆數單量 = current.成交筆數 - previous.成交筆數;
			}
		}
        }
}